using Microsoft.Extensions.Options;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;

using ReminderAIBot.Models;
using ReminderAIBot.Services.Messages.MessageHandler;


namespace ReminderAIBot.Services.Messenger.RecieverService
{
    public class TelegramReceiverService : BackgroundService, IReceiverService
    {
        private readonly ILogger<TelegramReceiverService> _logger;
        private readonly IHostEnvironment _hostEnvironment;

        private readonly TelegramBotConfig _telegramBotConfig;
        private readonly ITelegramBotClient _telegramBotClient;

        private readonly IMessageHandler _messageHandler;


        public TelegramReceiverService(ILogger<TelegramReceiverService> logger, IHostEnvironment hostEnvironment, IOptions<TelegramBotConfig> options, ITelegramBotClient telegramBotClient, IMessageHandler messageHandler)
        {
            this._logger = logger;
            this._hostEnvironment = hostEnvironment;

            this._telegramBotConfig = options.Value;
            this._telegramBotClient = telegramBotClient;

            this._messageHandler = messageHandler;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (this._hostEnvironment.IsDevelopment())
            {
                await this._telegramBotClient.DeleteWebhook();
                this._telegramBotClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, cancellationToken: stoppingToken);

                this._logger.LogInformation("polling mode");
            }
            else
            {
                await this._telegramBotClient.SetWebhook(this._telegramBotConfig.WebhookUrl, cancellationToken: stoppingToken);

                this._logger.LogInformation("webhook mode");
            }
        }


        private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            await this._messageHandler.HandleAsync(update);

            this._logger.LogInformation($"update: ID [{update.Message?.Chat.Id ?? update.CallbackQuery?.Message?.Chat.Id ?? -1}] \n\tmessage: {update.Message?.Text ?? "empty"} \n\tcallback: {update.CallbackQuery?.Data ?? "empty"}");
        }

        private async Task HandleErrorAsync(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            this._logger.LogError(exception.Message);
        }
    }
}
