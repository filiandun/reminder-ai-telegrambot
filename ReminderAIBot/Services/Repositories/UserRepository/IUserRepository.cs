using ReminderAIBot.Models.Database;

namespace ReminderAIBot.Services.Repositories.UserRepository
{
    public interface IUserRepository
    {
        public Task<User?> GetByTelegramId(long telegramId);

        public Task Add(User user);
        public Task Remove(User user);
        public Task Update(User user);
    }
}
