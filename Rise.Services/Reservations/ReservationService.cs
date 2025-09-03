using Microsoft.EntityFrameworkCore;
using Npgsql;
using Rise.Domain.Boats;
using Rise.Domain.Exceptions;
using Rise.Domain.Reservations;
using Rise.Domain.TimeSlots;
using Rise.Domain.Users;
using Rise.Persistence;
using Rise.Services.Constants;
using Rise.Services.Auth;
using Rise.Services.Pagination;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;
using Rise.Shared.Notifications;

namespace Rise.Services.Reservations
{
    public class ReservationService(ApplicationDbContext dbContext, IAuthContextProvider authContextProvider, IInternalNotificationService internalNotificationService)
        : AuthenticatedService(dbContext, authContextProvider), IReservationService
    {


        /// <summary>
        /// Gets all reservations in the given date range by the current user
        /// </summary>
        internal class ReservationTemp
        {
            public DateOnly Date { get; set; }
        }
        public async Task<ReservationsRangeDto> GetAllReservationsInRangeByCurrentUser(DateOnly startDate, DateOnly endDate)
        {
            int userId = (int)_authContextProvider.GetUserId()!;

            ISet<DateOnly> reservations = new HashSet<DateOnly>();

            List<ReservationTemp> allReservationsDuringRange = await _dbContext.Reservations.Where(
                reservation =>
                reservation.TimeSlot.Date >= startDate &&
                reservation.TimeSlot.Date <= endDate &&
                reservation.UserId == userId
                )
                .Select(reservation => new ReservationTemp()
                {
                    Date = reservation.TimeSlot.Date,
                }
                )
                .ToListAsync();

            return new ReservationsRangeDto(allReservationsDuringRange.Select(reservation => reservation.Date));
        }

        public async Task<ItemsPageDto<ReservationDto>> GetUserReservations(int? cursor, bool? isNextPage, bool getPast = false, int pageSize = 5)
        {
            int userId = (int)_authContextProvider.GetUserId()!;
            return await PaginationService.GetPaginatedResultsAsync<Reservation, ReservationDto>(
                queryableDbSet: _dbContext.Reservations.AsQueryable(),
                filterLambda: r => (r.UserId == userId) && (getPast ?
                    r.TimeSlot.Date < DateOnly.FromDateTime(DateTime.Now) :
                    r.TimeSlot.Date >= DateOnly.FromDateTime(DateTime.Now)),
                orderingExpressions: [
                    new OrderingExpression<Reservation, object>
                    {
                        OrderLambda = r => r.TimeSlot.Date,
                        IsDescending = getPast
                    },
                    new OrderingExpression<Reservation, object>
                    {
                        OrderLambda = r => r.Id,
                        IsDescending = getPast
                    }
                ],
                projection: r => new ReservationDto
                {
                    Id = r.Id,
                    Start = r.TimeSlot.Start,
                    End = r.TimeSlot.End,
                    Date = r.TimeSlot.Date,
                    BoatId = r.BoatId,
                    IsDeleted = r.IsDeleted,
                    BoatPersonalName = r.Boat.PersonalName
                },
                cursor: cursor,
                isNextPage: isNextPage,
                pageSize: pageSize
            );
        }

        /// <summary>
        /// Creates a reservation for the current user
        /// </summary>
        /// <param name="timeSlotId"></param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException"></exception>
        /// <exception cref="NoEntityAvailableException"></exception>
        /// <exception cref="EntityAlreadyExistsException"></exception>
        /// <exception cref="ReservationCreationFailedException"></exception>
        public async Task<int> CreateReservation(CreateReservationDto reservationDto)
        {
            int userId = (int)_authContextProvider.GetUserId()!;


            User user = await _dbContext.Users.FindAsync(userId) ?? throw new EntityNotFoundException(nameof(User), userId);

            TimeSlot timeSlot = await _dbContext.TimeSlots.FindAsync(reservationDto.TimeSlotId) ?? throw new EntityNotFoundException(nameof(TimeSlot), reservationDto.TimeSlotId);

            //get a boat that is available for that time slot
            //we just assign the first boat that is available
            //user can't choose a boat
            Boat boat = await _dbContext.Boats.Where(b => b.Reservations.All(r => r.TimeSlotId != timeSlot.Id)).FirstOrDefaultAsync() ?? throw new NoBoatAvailableException(timeSlot.Id);
            int boatId = boat.Id;

            bool hasReservationForTimeSlot = await _dbContext.Reservations.AnyAsync(r => r.TimeSlotId == timeSlot.Id && r.UserId == userId);

            Reservation reservation = new()
            {
                UserId = userId,
                User = user,
                TimeSlot = timeSlot,
                TimeSlotId = timeSlot.Id,
                BoatId = boatId,
                Boat = boat
            };

            try
            {
                _dbContext.Reservations.Add(reservation);
                await _dbContext.SaveChangesAsync();

                await internalNotificationService.SendNotificationToUser(
                userId,
                "Reservation Confirmed",
                $"Your reservation on {timeSlot.Date.ToLongDateString()} {timeSlot.Start:HH:mm} - {timeSlot.End:HH:mm} has been confirmed with boat {boat.PersonalName}. Please arrive on time.",
                SeverityEnum.Success
                );

                return reservation.Id;
            }
            catch (DbUpdateException ex)
            {
                HandleDbUpdateException(ex);
                throw new ReservationCreationFailedException(ErrorMessages.Reservation.UnexpectedError);
            }
        }

        /// <summary>
        /// Handles the exceptions thrown by the database when creating a reservation
        /// </summary>
        /// <param name="ex"></param>
        /// <exception cref="UniqueConstraintViolationException"></exception>
        /// <exception cref="ReservationCreationFailedException"></exception>
        private static void HandleDbUpdateException(DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException pgEx)
            {
                string message = pgEx.ConstraintName switch
                {
                    DatabaseConstraints.UniqueBoatTimeSlot => ErrorMessages.Reservation.BoatAlreadyReserved,
                    DatabaseConstraints.UniqueUserTimeSlot => ErrorMessages.Reservation.UserAlreadyBooked,
                    _ => ErrorMessages.Reservation.UnexpectedError //default message for unexpected errors
                };

                throw new UniqueConstraintViolationException(message);
            }
        }

        public async Task<ReservationDetailsDto> GetReservationDetailsAsync(int reservationId)
        {
            var reservation = await _dbContext.Reservations
                .Include(r => r.Boat)
                .Include(r => r.TimeSlot)
                .Include(r => r.PreviousBatteryHolder)
                .Include(r => r.Battery)
                    .ThenInclude(b => b!.Mentor)
                .FirstOrDefaultAsync(r => r.Id == reservationId)
                ?? throw new EntityNotFoundException(nameof(Reservation), reservationId);

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            DateOnly date = reservation.TimeSlot.Date;
            DateOnly beforeBuffer = date.AddDays(-Reservation.MinDaysBetweenReservation);
            User? previousBatteryHolder = beforeBuffer <= today && today <= date ? reservation.PreviousBatteryHolder : null;

            return new ReservationDetailsDto
            {
                Id = reservation.Id,
                Start = reservation.TimeSlot.Start,
                End = reservation.TimeSlot.End,
                Date = reservation.TimeSlot.Date,
                IsDeleted = reservation.IsDeleted,
                BoatPersonalName = reservation.Boat.PersonalName,
                MentorName = reservation.Battery?.Mentor?.FamilyName,
                BatteryId = reservation.Battery?.Id,
                CurrentBatteryUserName = previousBatteryHolder?.FamilyName,
                CurrentBatteryUserId = previousBatteryHolder?.Id,
                CurrentHolderPhoneNumber = previousBatteryHolder?.PhoneNumber,
                CurrentHolderEmail = previousBatteryHolder?.Email,
                CurrentHolderStreet = previousBatteryHolder?.Address.Street,
                CurrentHolderNumber = previousBatteryHolder?.Address.Number,
                CurrentHolderCity = previousBatteryHolder?.Address.City,
                CurrentHolderPostalCode = previousBatteryHolder?.Address.PostalCode
            };
        }
        public async Task CancelReservationAsync(int reservationId)
        {
            bool isAdmin = _authContextProvider.IsAdmin();

            int userId = (int)_authContextProvider.GetUserId()!;

            var query = _dbContext.Reservations
                .Include(r => r.TimeSlot)
                .Include(r => r.User)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(r => r.UserId == userId);
            }

            var reservation = await query.FirstOrDefaultAsync(r => r.Id == reservationId)
                ?? throw new EntityNotFoundException(nameof(Reservation), reservationId);

            reservation.Cancel(isAdmin);

            await _dbContext.SaveChangesAsync();
            try
            {
                await internalNotificationService.SendNotificationToUser(
                    userId,
                    "Reservation Cancelled",
                    $"Your reservation on {reservation.TimeSlot.Date.ToLongDateString()} {reservation.TimeSlot.Start:HH:mm} - {reservation.TimeSlot.End:HH:mm} has been cancelled.",
                    SeverityEnum.Info
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> GetReservationsCountAsync(DateOnly date)
        {
            return await _dbContext.Reservations
                .CountAsync(r => r.TimeSlot.Date == date && !r.IsDeleted);
        }

        public async Task<ItemsPageDto<ReservationDto>> GetAllReservations(int? cursor, bool? isNextPage, int pageSize = 10, bool showPastReservations = false)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            return await PaginationService.GetPaginatedResultsAsync<Reservation, ReservationDto>(
                queryableDbSet: _dbContext.Reservations
                    .Include(r => r.User)
                    .Include(r => r.TimeSlot)
                    .Include(r => r.Boat)
                    .Where(r => showPastReservations ? r.TimeSlot.Date < today : r.TimeSlot.Date >= today)
                    .AsQueryable(),
                filterLambda: r => true,
                orderingExpressions: new List<OrderingExpression<Reservation, object>>
                {
            new() { OrderLambda = r => r.TimeSlot.Date, IsDescending = false },
            new() { OrderLambda = r => r.Id, IsDescending = false }
                },
                projection: r => new ReservationDto
                {
                    Id = r.Id,
                    Start = r.TimeSlot.Start,
                    End = r.TimeSlot.End,
                    Date = r.TimeSlot.Date,
                    BoatId = r.BoatId,
                    IsDeleted = r.IsDeleted,
                    BoatPersonalName = r.Boat.PersonalName,
                    UserName = r.User.FamilyName
                },
                cursor: cursor,
                isNextPage: isNextPage,
                pageSize: pageSize
            );
        }





    }
}




