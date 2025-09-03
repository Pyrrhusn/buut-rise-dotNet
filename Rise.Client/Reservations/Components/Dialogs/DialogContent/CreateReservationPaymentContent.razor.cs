namespace Rise.Client.Reservations.Components.Dialogs.DialogContent
{
    public partial class CreateReservationPaymentContent
    {
        private string LoadingDots = ".";
        private Timer? timer;

        protected override void OnInitialized()
        {
            timer = new Timer(
            _ =>
            {
                LoadingDots = LoadingDots.Length >= 3 ? "." : LoadingDots + ".";
                InvokeAsync(StateHasChanged);
            },
            null,
            0,
            500
            );
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}