using ReminderAIBot.Models.Messages;
using ReminderAIBot.Models.UI.ScreenModel;


namespace ReminderAIBot.Services.Messenger.ScreenRenderer
{
    public interface IScreenRenderer
    {
        public RenderedMessage Render(ScreenModel model);
    }
}
