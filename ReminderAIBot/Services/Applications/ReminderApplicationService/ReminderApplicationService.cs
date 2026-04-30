using ReminderAIBot.Models.Database;
using ReminderAIBot.Models.UI.ScreenModel;

using ReminderAIBot.Services.ReminderManager;


namespace ReminderAIBot.Services.Applications.ReminderApplicationService
{
    public class ReminderApplicationService : IReminderApplicationService
    {
        private readonly ILogger<ReminderApplicationService> _logger;

        private readonly IReminderManager _reminderManager;


        public ReminderApplicationService(ILogger<ReminderApplicationService> logger, IReminderManager reminderManager)
        {
            this._logger = logger;
            this._reminderManager = reminderManager;
        }


        public async Task<RemindersListScreenModel> BuildRemindersListScreenModelAsync(long chatId, int page = 0, int pageSize = 5)
        {
            List<Reminder> reminders = (await this._reminderManager.GetRemindersList(chatId)).Skip(page * pageSize).Take(pageSize).ToList();
            int remindersCount = (await this._reminderManager.GetRemindersList(chatId)).Count();

            return new RemindersListScreenModel()
            {
                Title = "Список ваших напоминаний",
                Text = $"всего напоминаний: {remindersCount}",

                Reminders = reminders,

                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)remindersCount / pageSize)
            };
        }


        public async Task<ReminderScreenModel> BuildReminderScreenModelAsync(long chatId, int reminderId)
        {
            Reminder reminder = (await this._reminderManager.GetReminder(chatId, reminderId));

            return new ReminderScreenModel()
            {
                ReminderId = reminder.Id,

                Title = "Просмотр напоминания",
                Text = "тут вы можете управлять напоминанием",

                IsEnabled = true,

                ReminderText = reminder.Text,
                RawText = reminder.RawText,

                RemindAt = reminder.RemindAt.DateTime.ToString("U"),
                CreatedAt = reminder.CreatedAt.DateTime.ToString("U"),
            };
        }
    }
}
