using System.Text.RegularExpressions;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Home
{
    public class HomePageTestAuthenticated : CustomAuthenticatedPageTest
    {

        [Test]
        public async Task ClickReservationButton_ShouldNavigateToReservations()
        {
            UserRole role = UserRole.Guest;
            //Arrange
            await LoginAsync(role);
            await NavigateToUrl("/home");

            //Act
            var reservationButton = Page.GetByTestId("reservation-button");
            await reservationButton.ClickAsync();

            //Assert
            await Expect(Page).ToHaveURLAsync(new Regex("/reservations\\?.*$"));

            await LogoutAsync(role);
        }
    }
}