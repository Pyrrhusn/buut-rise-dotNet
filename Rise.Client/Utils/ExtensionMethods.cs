using MudBlazor;
using Rise.Shared.Notifications;

namespace Rise.Client.Utils
{
    public static class ExtensionMethods
    {
        private const string universalDateFormat = "yyyy-MM-dd";
        public static string ToUniversalStringDate(this DateOnly date)
        {
            return date.ToString(universalDateFormat);
        }

        public static string ToRelativeDateString(this DateTime date)
        {
            //If the timestamp is today, return the time
            if (date.Date == DateTime.Today)
            {
                return date.ToShortTimeString();
            }

            //If the timestamp was yesterday, return "Yesterday" + time
            if (date.Date == DateTime.Today.AddDays(-1))
            {
                return "Gisteren om " + date.ToShortTimeString();
            }

            //If the timestamp is within the last 7 days, return the day of the week and the time
            if (date.Date >= DateTime.Today.AddDays(-7))
            {
                //return the day of the week and the time
                return date.ToString("dddd HH:mm");
            }

            //Otherwise, return the date
            return date.ToString("g");
        }

        public static MudBlazor.Color Color(this SeverityEnum severity)
        {
            return severity switch
            {
                SeverityEnum.Error => MudBlazor.Color.Error,
                SeverityEnum.Info => MudBlazor.Color.Info,
                SeverityEnum.Success => MudBlazor.Color.Success,
                SeverityEnum.Warning => MudBlazor.Color.Warning,
                _ => MudBlazor.Color.Default
            };
        }

        public static string Icon(this SeverityEnum severity)
        {
            return severity switch
            {
                SeverityEnum.Error => Icons.Material.Outlined.Error,
                SeverityEnum.Info => Icons.Material.Outlined.Info,
                SeverityEnum.Success => Icons.Material.Outlined.CheckCircle,
                SeverityEnum.Warning => Icons.Material.Outlined.Warning,
                _ => Icons.Material.Outlined.Info
            };
        }
    }
}

