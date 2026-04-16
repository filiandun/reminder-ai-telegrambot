using ReminderAIBot.Models.Callbacks;


namespace ReminderAIBot.Services.Callbacks.CallbackDataParser
{
    public interface ICallbackDataParser
    {
        public CallbackData? Parse(string data);
    }
}
