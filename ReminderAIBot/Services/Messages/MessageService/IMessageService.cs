
namespace ReminderAIBot.Services.Messages.MessageService
{
    public interface IMessageService
    {
        public Task HandleAsync(long chatId, string? messageText, string? command);
    }
}
