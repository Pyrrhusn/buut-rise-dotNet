using Microsoft.AspNetCore.Components;
using Rise.Client.Common;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;
using Microsoft.JSInterop;
using Microsoft.Extensions.Localization;
using Rise.Client.Localization.Reservations;


namespace Rise.Client.Reservations.Components;

public class UserReservationsBase : ComponentBase
{
    public required AsyncData<ItemsPageDto<ReservationDto>> AsyncDataRef { get; set; }
    protected ItemsPageDto<ReservationDto>? ReservationPage { get; set; }

    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    [Inject]
    public required IReservationService ReservationService { get; set; }

    [Inject]
    public required IJSRuntime JS { get; set; }

    [Parameter]
    public bool IsNextPage { get; set; } = true;
    private Stack<int?> _previousCursors = new();
    private bool IsFirstPage;

    [Parameter]
    [SupplyParameterFromQuery]
    public bool Past { get; set; }

    protected bool ShowPastReservations
    {
        get;
        set;
    }

    protected override async Task OnInitializedAsync()
    {
        ShowPastReservations = Past;
        await base.OnInitializedAsync();
    }

    protected Task<ItemsPageDto<ReservationDto>> LoadReservations()
    {
        int? cursor = IsNextPage ? ReservationPage?.NextId : ReservationPage?.PreviousId;

        if (IsNextPage && cursor != null)
        {
            _previousCursors.Push(ReservationPage?.PreviousId);
        }
        else if (!IsNextPage && _previousCursors.Count > 0)
        {
            cursor = _previousCursors.Pop();
        }

        IsFirstPage = cursor == null;

        return ReservationService.GetUserReservations(
            cursor,
            cursor != null ? IsNextPage : null,
            ShowPastReservations,
            pageSize: 5
        );
    }

    protected async Task FetchAndResetScroll()
    {
        await AsyncDataRef.FetchData();
        await ScrollToTop();
    }

    protected async Task LoadNextPage()
    {
        IsNextPage = true;
        await FetchAndResetScroll();
    }

    protected async Task LoadPreviousPage()
    {
        if (ReservationPage?.PreviousId is null || ReservationPage?.IsFirstPage == true)
            return;

        IsNextPage = false;
        await FetchAndResetScroll();
    }

    protected async Task ScrollToTop()
    {
        await JS.InvokeVoidAsync("window.scrollTo", 0, 0);
    }

    protected async Task TogglePastReservations()
    {
        await TogglePastReservations(!ShowPastReservations);
    }

    protected void NavigateTo(int reservationId)
    {
        NavigationManager.NavigateTo($"/reservations/{reservationId}");
    }

    protected async Task TogglePastReservations(bool enable)
    {
        if (ShowPastReservations != enable)
        {
            ShowPastReservations = enable;
            ReservationPage = null;
            _previousCursors.Clear();
            IsFirstPage = true;
            IsNextPage = true;

            Dictionary<string, object?> queries = new()
            {
                ["CurrentTab"] = "reservations",
                ["Past"] = enable
            };
            NavigationManager.NavigateTo(NavigationManager.GetUriWithQueryParameters(queries), forceLoad: false, replace: true);

            await FetchAndResetScroll();
        }
    }



}
