using Microsoft.AspNetCore.Components;
using Rise.Shared.TimeSlots;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using static Rise.Shared.TimeSlots.CreateTimeSlotDto;
using Validator = Rise.Shared.TimeSlots.CreateTimeSlotDto.Validator;

namespace Rise.Client.Admins.CruisePeriods.TimeSlots
{
    public partial class Index : ComponentBase
    {
        [Inject]
        public required ICruisePeriodService CruisePeriodService { get; set; }

        [Inject]
        private ITimeSlotService TimeSlotService { get; set; } = default!;

        [Inject]
        public required ISnackbar Snackbar { get; set; }

        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        [Parameter]
        public int? Id { get; set; }

        private MudForm form = default!;
        private Validator validator = new();
        private TimeSpan? StartTime { get; set; }
        private TimeSpan? EndTime { get; set; }
        private CreateTimeSlotDto AllTimeSlotsDto { get; set; } = new CreateTimeSlotDto();
        private CruisePeriodDetailedDto? CruisePeriod { get; set; }

        private string FormatTimeSlot(TimeSlotRange timeSlot)
        {
            return $"{timeSlot.Start.ToString("HH:mm")} - {timeSlot.End.ToString("HH:mm")}";
        }

        protected override async Task OnInitializedAsync()
        {
            if (Id.HasValue)
            {
                try
                {
                    CruisePeriod = await CruisePeriodService.GetCruisePeriod(Id.Value);
                    if (CruisePeriod == null)
                    {
                        Snackbar.Add("Geen vaarperiode gevonden", Severity.Error);
                        NavigationManager.NavigateTo("/admin/cruise_period");
                        return;
                    }

                    AllTimeSlotsDto = new CreateTimeSlotDto
                    {
                        CruisePeriodId = Id.Value
                    };
                }
                catch (Exception)
                {
                    Snackbar.Add("Geen vaarperiode gevonden", Severity.Error);
                    NavigationManager.NavigateTo("/admin/cruise_period");
                    return;
                }
            }
        }

        public async Task<CruisePeriodDetailedDto> FetchCruisePeriod()
        {
            try
            {
                var cruisePeriod = await CruisePeriodService.GetCruisePeriod(Id ?? 1);
                if (cruisePeriod == null)
                {
                    NavigationManager.NavigateTo("/admin/cruise_period");
                    throw new Exception("Cruise period not found");
                }
                return cruisePeriod;
            }
            catch
            {
                NavigationManager.NavigateTo("/admin/cruise_period");
                throw;
            }
        }

        private async Task AddTimeSlot()
        {
            if (StartTime.HasValue && EndTime.HasValue)
            {
                var newTimeSlot = new TimeSlotRange
                {
                    Start = TimeOnly.FromTimeSpan(StartTime.Value),
                    End = TimeOnly.FromTimeSpan(EndTime.Value)
                };

                AllTimeSlotsDto.TimeSlots.Add(newTimeSlot);

                var validationResult = await validator.ValidateAsync(AllTimeSlotsDto);

                if (validationResult.IsValid)
                {
                    StartTime = null;
                    EndTime = null;
                    await form.ResetAsync();
                    StateHasChanged();
                }
                else
                {
                    AllTimeSlotsDto.TimeSlots.Remove(newTimeSlot);
                    foreach (var error in validationResult.Errors)
                    {
                        Snackbar.Add(error.ErrorMessage, Severity.Error);
                    }
                }
            }
        }

        private void RemoveTimeSlot(TimeSlotRange timeSlot)
        {
            AllTimeSlotsDto.TimeSlots.Remove(timeSlot);
            StateHasChanged();
        }

        private async Task SaveTimeSlots()
        {
            try
            {
                Console.WriteLine("Saving time slots");
                Console.WriteLine(AllTimeSlotsDto.TimeSlots.Count);
                await TimeSlotService.CreateTimeSlot(AllTimeSlotsDto);
                Snackbar.Add("Time slots saved successfully", Severity.Success);
                await form.ResetAsync();
                AllTimeSlotsDto.TimeSlots.Clear();
                StateHasChanged();
            }
            catch (Exception)
            {
                Snackbar.Add("Failed to save time slots", Severity.Error);
            }
        }

        private void HandleStartTimeChanged()
        {
            if (StartTime.HasValue)
            {
                EndTime = StartTime.Value.Add(TimeSpan.FromHours(3));
                StateHasChanged();
            }
        }
    }
}