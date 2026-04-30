using ReminderAIBot.Models.Database;

namespace ReminderAIBot.Services.Repositories.ReminderRepository
{
    public interface IReminderRepository
    {
        public Task<Reminder?> GetReminder(int reminderId);
        public Task<List<Reminder>> GetRemindersList(long userId);

        public Task Add(Reminder reminder);
        public Task Update(Reminder reminder);
        public Task Remove(int reminderId);
    }
}
