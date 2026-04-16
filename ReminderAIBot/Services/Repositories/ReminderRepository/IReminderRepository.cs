using ReminderAIBot.Models.Database;

namespace ReminderAIBot.Services.Repositories.ReminderRepository
{
    public interface IReminderRepository
    {
        public Task<List<Reminder>> GetByUserId(long userId);

        public Task Add(Reminder reminder);
        public Task Update(Reminder reminder);
        public Task Remove(int reminderId);
    }
}
