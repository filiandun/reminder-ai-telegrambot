using ReminderAIBot.Models.Messages;

namespace ReminderAIBot.Services.Messenger.SenderService
{
    public interface ISenderService
    {
        public Task SendMessageAsync(long chatId, RenderedMessage message);

        public Task EditMessageAsync(long chatId, int messageId, RenderedMessage message);

        public Task DeleteMessageAsync(long chatId, int messageId);

        public Task AnswerCallbackQuery(string callbackQueryId, string text);

    }
}
