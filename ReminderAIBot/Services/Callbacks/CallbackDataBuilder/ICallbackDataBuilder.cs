using ReminderAIBot.Models.Callback.Enums;
using ReminderAIBot.Models.Callbacks.Enums;


namespace ReminderAIBot.Services.Callbacks.CallbackDataBuilder
{
    public interface ICallbackDataBuilder
    {
        public string Navigation(UiScreen action, NavigationItem type, int page);

        public string Reminder(ReminderAction action, int reminderId);
        public string TimeZone(TimeZoneAction action, string timeZoneId);

    }
}
