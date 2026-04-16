using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using ReminderAIBot.Services.Callbacks.CallbackService;
using ReminderAIBot.Services.Messages.MessageService;


namespace ReminderAIBot.Services.Messages.MessageHandler
{
    public class MessageHandler : IMessageHandler
    {
        private readonly ILogger<MessageHandler> _logger;

        private readonly IMessageService _messageService;
        private readonly ICallbackService _callbackService;


        public MessageHandler(ILogger<MessageHandler> logger, IMessageService messageService, ICallbackService callbackService)
        {
            this._logger = logger;

            this._messageService = messageService;
            this._callbackService = callbackService;
        }


        public async Task HandleAsync(Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message: await this.HandleMessageAsync(update.Message); break;
                case UpdateType.CallbackQuery: await this.HandleCallbackAsync(update.CallbackQuery); break;

                //case UpdateType.ChatMember: break;
                //case UpdateType.EditedMessage: break;
                //case UpdateType.Unknown: break;

                default: this._logger.LogWarning($"pre handle: unhandled update type: {update.Type}"); break;
            }
        }


        private async Task HandleMessageAsync(Message? message)
        {
            if (message is null)
            {
                this._logger.LogWarning("pre handle message: message is null");
                return;
            }

            long chatId = message.Chat.Id;
            string? messageText = message.Text;
            string? command = null;

            MessageEntity? commandEntity = message.Entities?.FirstOrDefault(e => e.Type == MessageEntityType.BotCommand) ?? null;
            if (commandEntity is not null)
            {
                command = message.Text?.Substring(commandEntity.Offset, commandEntity.Length) ?? null;
            }

            await this._messageService.HandleAsync(chatId, messageText, command);
        }

        private async Task HandleCallbackAsync(CallbackQuery? callbackQuery)
        {
            if (callbackQuery is null)
            {
                this._logger.LogWarning("pre handle callback: callbackQuery is null");
                return;
            }

            if (callbackQuery.Message is null)
            {
                this._logger.LogWarning("pre handle callback: callbackQuery.Message is null");
                return;
            }

            long chatId = callbackQuery.Message.Chat.Id;
            int messageId = callbackQuery.Message.MessageId;
            string? callbackData = callbackQuery.Data;

            await this._callbackService.HandleAsync(chatId, messageId, callbackData);
        }
    }
}