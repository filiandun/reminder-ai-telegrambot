using ReminderAIBot.Models.CallbackData;
using ReminderAIBot.Models.Database;
using ReminderAIBot.Models.Messages;
using ReminderAIBot.Models.UI;
using ReminderAIBot.Services.Messages.MessageBuilder;
using ReminderAIBot.Services.Messenger.SenderService;
using ReminderAIBot.Services.OnboardingService;
using ReminderAIBot.Services.ReminderParser;
using ReminderAIBot.Services.ReminderService;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace ReminderAIBot.Services.Messages.MessageService
{
    public class MessageService : IMessageService
    {
        private readonly ILogger<MessageService> _logger;

        private readonly ISenderService _senderService;

        private readonly IReminderService _reminderService;
        private readonly IReminderParser _reminderParser;

        private readonly IMessageBuilder _messageBuilder;

        private readonly IOnboardingService _onboardingService;


        public MessageService(ILogger<MessageService> logger, ISenderService senderService, IReminderService reminderService, IReminderParser reminderParser, IMessageBuilder messageBuilder, IOnboardingService onboardingService)
        {
            this._logger = logger;

            this._senderService = senderService;

            this._reminderService = reminderService;
            this._reminderParser = reminderParser;

            this._messageBuilder = messageBuilder;           

            this._onboardingService = onboardingService;
        }


        public async Task HandleAsync(long chatId, string? messageText, string? command)
        {
            if (messageText is null)
            {
                this._logger.LogWarning("handle: messageText is null");
                return;
            }

            if (command is not null)
            {
                await this.HandleCommandAsync(chatId, command);
                return;
            }

            Reminder newReminder = await this._reminderParser.ParseAsync(messageText);
            await this._reminderService.AddReminder(chatId, newReminder);

            BotMessage newReminderMessage = this._messageBuilder.NewReminderMessage(newReminder);
            await this._senderService.SendMessageAsync(chatId, newReminderMessage);
        }


        private async Task HandleCommandAsync(long chatId, string command)
        {
            switch (command)
            {
                case "/start":
                    await this._onboardingService.Greeting(chatId);

                    break;

                case "/help":
                    await this._senderService.SendMessageAsync(chatId, new BotMessage() { Text = "Я бот на основе Искусственного Интеллекта для создания напоминаний. Пример использования: напиши \"сегодня в 12 дня покормить кота\"" });

                    break;

                case "/list":
                    PagedList<Reminder> reminders = new PagedList<Reminder>(await this._reminderService.GetUserReminders(chatId), 2);

                    BotMessage botMessage = this._messageBuilder.RemindersList(reminders.GetPage(0).ToList(), 0);

                    await this._senderService.SendMessageAsync(chatId, botMessage);

                    break;

                default:
                    await this._senderService.SendMessageAsync(chatId, new BotMessage() { Text = "К сожалению, такой команды я не знаю" });

                    break;
            }
        }
    }
}
