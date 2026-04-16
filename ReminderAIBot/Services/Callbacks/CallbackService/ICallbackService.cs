
namespace ReminderAIBot.Services.Callbacks.CallbackService
{
    public interface ICallbackService
    {
        public Task HandleAsync(long chatId, int messageId, string? data);
    }
}
