using ReminderAIBot.Models.Database;
using ReminderAIBot.Services.Repositories.ReminderRepository;
using ReminderAIBot.Services.Repositories.UserRepository;

namespace ReminderAIBot.Services.ReminderService
{
    public class ReminderService : IReminderService
    {
        private readonly ILogger<ReminderService> _logger;

        private readonly IUserRepository _userRepository;
        private readonly IReminderRepository _reminderRepository;


        public ReminderService(ILogger<ReminderService> logger, IUserRepository userRepository, IReminderRepository reminderRepository)
        {
            this._logger = logger;

            this._reminderRepository = reminderRepository;
            this._userRepository = userRepository;
        }


        public async Task<List<Reminder>> GetUserReminders(long telegramId)
        {
            User? user = await this._userRepository.GetByTelegramId(telegramId);
            if (user is null) return new List<Reminder>();

            this._logger.LogInformation($"user {telegramId} get all reminders");

            return await this._reminderRepository.GetByUserId(user.Id);
        }

        public async Task AddReminder(long userId, Reminder reminder)
        {
            User? user = await this._userRepository.GetByTelegramId(userId);
            if (user is null)
            {
                user = new User { Id = new Random().Next(), TelegramId = userId, TimeZoneId = TimeZoneInfo.Local.ToString() };
                await this._userRepository.Add(user);
            }

            reminder.Id = new Random().Next();
            reminder.UserId = user.Id;

            this._logger.LogInformation($"add: user [{userId}] add new reminder {reminder.Text}");

            await this._reminderRepository.Add(reminder);
        }

        public async Task RemoveReminder(long userId, int reminderId)
        {
            User? user = await this._userRepository.GetByTelegramId(userId);
            if (user is null) throw new Exception("remove reminder: user by telegram id not found");

            if (user.TelegramId != userId) throw new Exception("try remove reminder other user");

            this._logger.LogInformation($"remove reminder: user [{userId}] remove reminder [{reminderId}]");

            await this._reminderRepository.Remove(reminderId);
        }
    }
}
