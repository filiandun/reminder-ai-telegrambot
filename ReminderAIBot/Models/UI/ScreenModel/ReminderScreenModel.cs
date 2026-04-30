
namespace ReminderAIBot.Models.UI.ScreenModel
{
    public class ReminderScreenModel : ScreenModel
    {
        public int ReminderId { get; set; }

        public bool IsEnabled { get; set; }

        public string ReminderText { get; set; }
        public string RawText { get; set; }

        public string RemindAt { get; set; }
        public string CreatedAt { get; set; }
    }
}
