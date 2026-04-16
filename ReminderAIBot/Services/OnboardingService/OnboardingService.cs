using ReminderAIBot.Models;
using ReminderAIBot.Models.Messages;
using ReminderAIBot.Services.Messenger.SenderService;
using System.Text;

namespace ReminderAIBot.Services.OnboardingService
{
    public class OnboardingService : IOnboardingService
    {
        private readonly ILogger<OnboardingService> _logger;

        private readonly ISenderService _senderService;


        public OnboardingService(ILogger<OnboardingService> logger, ISenderService senderService)
        {
            this._logger = logger;

            this._senderService = senderService;
        }


        public async Task Greeting(long chatId)
        {
            StringBuilder greetingSb = new StringBuilder();
            greetingSb.AppendLine("Привет!");
            greetingSb.AppendLine("Я бот для создания напоминаний.");
            greetingSb.AppendLine("Внутри себя я использую ИИ для лучшего распознавания: росто напиши мне \"послезавтра в 7 вечера запись к барберу\"");
            greetingSb.AppendLine();
            greetingSb.AppendLine("Однако перед использованием выбери свой часовой пояс из списка ниже, это необходимо для корретных и своевременных напоминаниях.");

            BotMessage message = new BotMessage
            {
                Text = greetingSb.ToString(),
                Buttons = 
            }

            await this._senderService.SendMessageAsync(chatId, );
        }

        public async Task AskTimeZone(long chatId)
        {
            List<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones().ToList();

            throw new NotImplementedException();
        }
    }
}