using ReminderAIBot.Models.Messages;

namespace ReminderAIBot.Services.Messenger.SenderService
{
    public interface ISenderService
    {
        public Task SendMessageAsync(long chatId, BotMessage message);

        public Task EditMessageAsync(long chatId, int messageId, BotMessage message);

        public Task DeleteMessageAsync(long chatId, int messageId);
    }
}
