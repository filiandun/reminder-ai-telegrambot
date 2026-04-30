using IAIChatServiceLib.Models;
using ReminderAIBot.Models.Messages;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace ReminderAIBot.Services.Messenger.SenderService
{
    public class TelegramSenderService : ISenderService
    {
        private readonly ILogger<TelegramSenderService> _logger;

        private readonly ITelegramBotClient _telegramBotClient;


        public TelegramSenderService(ILogger<TelegramSenderService> logger, ITelegramBotClient telegramBotClient)
        {
            this._logger = logger;
            this._telegramBotClient = telegramBotClient;
        }


        public async Task SendMessageAsync(long chatId, RenderedMessage message)
        {
            if (message.InlineButtonRows is null)
            {
                await this._telegramBotClient.SendMessage(chatId: new ChatId(chatId), text: message.Text, protectContent: true);
            }
            else
            {
                InlineKeyboardMarkup inlineKeyboardMarkup = this.CreateInlineKeyboard(message.InlineButtonRows);

                await this._telegramBotClient.SendMessage(chatId: new ChatId(chatId), text: message.Text, protectContent: true, replyMarkup: inlineKeyboardMarkup);
            }

            this._logger.LogInformation($"send: chatId [{chatId}] buttons count [{message.InlineButtonRows?.Count ?? 0}] text \"{message.Text.Take(40)}..\"");
        }


        public async Task EditMessageAsync(long chatId, int messageId, RenderedMessage message)
        {
            await this._telegramBotClient.EditMessageText(chatId: new ChatId(chatId), messageId: messageId, message.Text);

            InlineKeyboardMarkup inlineKeyboardMarkup = this.CreateInlineKeyboard(message.InlineButtonRows);
            await this._telegramBotClient.EditMessageReplyMarkup(chatId: new ChatId(chatId), messageId: messageId, replyMarkup: inlineKeyboardMarkup);

            this._logger.LogInformation($"edit: chatId [{chatId}] messageId [{messageId}] message text \"{message.Text.Take(40)}..\" buttons count [{message.InlineButtonRows?.Count ?? 0}]");
        }


        public async Task DeleteMessageAsync(long chatId, int messageId)
        {
            await this._telegramBotClient.DeleteMessage(chatId: new ChatId(chatId), messageId: messageId);

            this._logger.LogInformation($"delete: chatId [{chatId}] messageId [{messageId}]");
        }


        public async Task AnswerCallbackQuery(string callbackQueryId, string text)
        {
            await this._telegramBotClient.AnswerCallbackQuery(callbackQueryId, text);
        }

        // TODO подумать над тем, как cделать многоуровневые кнопки (это больше вопрос к моделям)
        private InlineKeyboardMarkup CreateInlineKeyboard(List<InlineButtonRow> inlineButtonRows)
        {
            InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup();

            foreach (InlineButtonRow inlineButtonRow in inlineButtonRows)
            {
                foreach (InlineButton button in inlineButtonRow)
                {
                    inlineKeyboardMarkup.AddButton(button.Text, button.CallbackData ?? "unknown");
                }
                inlineKeyboardMarkup.AddNewRow();
            }

            return inlineKeyboardMarkup;
        }
    }
}
