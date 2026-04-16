using ReminderAIBot.Models.Callback.Enums;
using ReminderAIBot.Models.Callbacks.Enums;


namespace ReminderAIBot.Services.Callbacks.CallbackDataBuilder
{
    public class CallbackDataBuilder : ICallbackDataBuilder
    {
        public string Navigation(UiScreen action, NavigationItem type, int page) => $"nav:{(int)action}:{(int)type}:{page}";

        public string Reminder(ReminderAction action, int reminderId) => $"rem:{(int)action}:{reminderId}";
        public string TimeZone(TimeZoneAction action, string timeZoneId) => $"tmz:{(int)action}:{timeZoneId}";
    }
}
