namespace ReminderAIBot.Models.Messages
{
    public class InlineButton
    {
        public required string Text { get; set; }
        public string? CallbackData { get; set; }
    }
}
