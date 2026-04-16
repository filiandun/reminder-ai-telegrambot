using ReminderAIBot.Models.Database;
using ReminderAIBot.Models.Messages;


namespace ReminderAIBot.Services.Messages.MessageBuilder
{
    public interface IMessageBuilder
    {

        public BotMessage NewReminderMessage(Reminder reminder);
        public BotMessage RemindersList(List<Reminder> reminders, int page);
        public BotMessage TimeZonesList(List<TimeZoneInfo> timeZones, int page);

    }
}
