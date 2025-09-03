using Rise.Shared.Pagination;

namespace Rise.Shared.Reservations
{
    public interface IReservationService
    {
        Task<ReservationsRangeDto> GetAllReservationsInRangeByCurrentUser(DateOnly startDate, DateOnly endDate);

        /// <summary>
        /// Gets all the upcoming reservations for the current user
        /// </summary>
        /// <returns>All the reservations</returns>

        Task<ItemsPageDto<ReservationDto>> GetUserReservations(int? cursor, bool? isNextPage, bool getPast = false, int pageSize = 5);


        /// <summary>
        /// Creates a reservation with given details
        /// </summary>
        /// <param name="reservationDto">Dto with details of to be created reservation</param>
        /// <returns>The created reservation details</returns>
        Task<int> CreateReservation(CreateReservationDto reservationDto);


        /// <summary>
        /// Gets the details of a specific reservation
        /// </summary>
        /// <param name="reservationId">The ID of the reservation</param>
        /// <returns>The reservation details</returns>
        Task<ReservationDetailsDto> GetReservationDetailsAsync(int reservationId);

        /// <summary>
        /// Cancels a reservation
        /// </summary>
        /// <param name="reservationId">The ID of the reservation to be cancelled</param>
        Task CancelReservationAsync(int reservationId);

        /// <summary>
        /// Gets the count of reservations for a given date
        /// </summary>
        /// <param name="date">Date to get the count of reservations for</param>
        /// <returns>
        /// The count of reservations for the given date
        /// </returns>
        Task<int> GetReservationsCountAsync(DateOnly date);

        /// <summary>
        /// Gets all reservations for admin overview
        /// </summary>
        /// <param name="cursor">Pagination cursor for the current page</param>
        /// <param name="isNextPage">Specifies if the next page or previous page is needed</param>
        /// <param name="pageSize">The number of reservations per page</param>
        /// <returns>A paginated list of all reservations</returns>
        Task<ItemsPageDto<ReservationDto>> GetAllReservations(int? cursor, bool? isNextPage, int pageSize = 10, bool showPastReservations = false);
    }

}

