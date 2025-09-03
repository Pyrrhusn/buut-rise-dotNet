using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Rise.Client.Common
{
  public partial class RedirectAlert
  {
    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? AdditionalAttributes { get; set; }

    [Parameter]
    public required Severity AlertSeverity { get; set; } = Severity.Normal;
  }
}