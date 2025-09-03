using Microsoft.AspNetCore.Components;
using Rise.Client.Common;
using Rise.Shared.Reservations;
using Rise.Shared.Users;
using Rise.Shared;

namespace Rise.Client.Admins
{
    public partial class Index
    {
        [Inject]
        private IReservationService ReservationService { get; set; } = default!;

        [Inject]
        private IUserAdminService UserService { get; set; } = default!;

        [Inject]
        private IBoatService BoatService { get; set; } = default!;

        private AsyncData<int>? AsyncBoatsRef;
        private AsyncData<int>? AsyncReservationsRef;
        private AsyncData<int>? AsyncUsersRef;

        private int _boatsCount;
        private int _reservationsCount;
        private int _usersCount;

        private Task<int> FetchBoatsCount()
        {
            return BoatService.GetActiveBoatsCountAsync();
        }

        private Task<int> FetchReservationsCount()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return ReservationService.GetReservationsCountAsync(today);
        }

        private Task<int> FetchUsersCount()
        {
            return UserService.GetActiveUsersCountAsync();
        }
    }
}