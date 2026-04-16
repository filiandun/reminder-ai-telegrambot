using Telegram.Bot.Types;

namespace ReminderAIBot.Services.Messages.MessageHandler
{
    public interface IMessageHandler
    {
        public Task HandleAsync(Update update);
    }
}
