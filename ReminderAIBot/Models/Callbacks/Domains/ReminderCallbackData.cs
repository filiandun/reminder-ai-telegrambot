using ReminderAIBot.Models.Callbacks.Enums;


namespace ReminderAIBot.Models.Callbacks.Domains
{
    public class ReminderCallbackData : CallbackData
    {
        public ReminderAction Action { get; set; }
        public int ReminderId { get; set; }
    }
}
