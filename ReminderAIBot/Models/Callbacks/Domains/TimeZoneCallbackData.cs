using ReminderAIBot.Models.Callbacks.Enums;

namespace ReminderAIBot.Models.Callbacks.Domains
{
    public class TimeZoneCallbackData : CallbackData
    {
        public TimeZoneAction Action { get; set; }
        public string TimeZoneId { get; set; }
    }
}
