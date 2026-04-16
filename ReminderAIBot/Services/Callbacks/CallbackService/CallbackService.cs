using ReminderAIBot.Models.UI;
using ReminderAIBot.Models.Database;
using ReminderAIBot.Models.Messages;

using ReminderAIBot.Services.ReminderService;
using ReminderAIBot.Services.Messenger.SenderService;
using ReminderAIBot.Services.Messages.MessageBuilder;
using ReminderAIBot.Services.Callbacks.CallbackDataParser;
using ReminderAIBot.Models.Callback.Enums;
using ReminderAIBot.Models.Callbacks;
using ReminderAIBot.Models.Callbacks.Enums;
using ReminderAIBot.Models.Callbacks.Domains;


namespace ReminderAIBot.Services.Callbacks.CallbackService
{
    public class CallbackService : ICallbackService
    {
        private readonly ILogger<CallbackService> _logger;

        private readonly ICallbackDataParser _callbackDataParser;
        private readonly ISenderService _senderService;
        private readonly IReminderService _reminderService;

        private readonly IMessageBuilder _messageBuilder;


        public CallbackService(ILogger<CallbackService> logger, ICallbackDataParser callbackDataParser, ISenderService senderService, IReminderService reminderService, IMessageBuilder messageBuilder)
        {
            this._logger = logger;

            this._callbackDataParser = callbackDataParser;
            this._senderService = senderService;
            this._reminderService = reminderService;

            this._messageBuilder = messageBuilder;
        }


        public async Task HandleAsync(long chatId, int messageId, string? data)
        {
            if (data is null)
            {
                this._logger.LogWarning("handle: callbackData is null");
                return;
            }

            CallbackData? callbackData = this._callbackDataParser.Parse((string)data);

            switch (callbackData)
            {
                case ChangePageCallbackData navigationCallbackData: await this.HandleNavigationAsync(navigationCallbackData, chatId, messageId); break;
                case ReminderCallbackData reminderCallbackData: await this.HandleReminderAsync(reminderCallbackData, chatId, messageId); break;

                default: this._logger.LogWarning($"handle: unknown type callback data: ({callbackData?.GetType() ?? null})"); break;
            }
        }


        private async Task HandleReminderAsync(ReminderCallbackData reminderCallbackData, long chatId, int messageId)
        {
            switch (reminderCallbackData.Action)
            {
                case ReminderAction.Add:
                    //await this._senderService.SendMessageAsync(chatId, new BotMessage() { Text = "Напоминание успешно создано!" });
                    //await this._senderService.EditMessageAsync(chatId, callback.Message.MessageId, new BotMessage() { Text = "Напоминание создано!" }); 

                    break;

                case ReminderAction.Delete:
                    await this._reminderService.RemoveReminder(chatId, reminderCallbackData.ReminderId);

                    //await this._senderService.SendMessageAsync(chatId, new BotMessage() { Text = "Напоминание удалено." });
                    //await this._senderService.EditMessageAsync(chatId, callback.Message.MessageId, new BotMessage() { Text = "Напоминание удалено" });

                    break;

                case ReminderAction.Edit: break;

                default: this._logger.LogWarning($"handle reminder: unknown callback action: {reminderCallbackData.Action}"); break;
            }
        }

        private async Task HandleNavigationAsync(ChangePageCallbackData navigationCallbackData, long chatId, int messageId)
        {
            switch (navigationCallbackData.Action)
            {
                case UiScreen.Move:
                    // TODO надо подумать, как сделать нормально, так как тут костыли, так ещё и уйти можно за максимум страниц
                    // (благодаря PagedList, пробелм не будет, он будет просто возвращать полседнюю доступную страницу, хоть 99999 ему передай,
                    // однако проблема возникает дальше у builder, который к page + 1 делает, поэтому там до бесконечности дойти можно, а потом долго придётся возвращаться назад)
                    PagedList<Reminder> reminders = new PagedList<Reminder>(await this._reminderService.GetUserReminders(chatId), 5);

                    BotMessage botMessage = this._messageBuilder.RemindersList(reminders.GetPage(navigationCallbackData.Page).ToList(), navigationCallbackData.Page);

                    await this._senderService.EditMessageAsync(chatId, messageId, botMessage);

                    break;

                default: this._logger.LogWarning($"handle navigation: unknown callback action: {navigationCallbackData.Action}"); break;
            }
        }

        private async Task HandleTimeZoneAsync(TimeZoneCallbackData timeZoneCallbackData, long chatId, int messageId)
        {
            switch (timeZoneCallbackData.Action)
            {
                case TimeZoneAction.Set:
                    PagedList<TimeZoneInfo> reminders = new PagedList<TimeZoneInfo>(TimeZoneInfo.GetSystemTimeZones().ToList(), 5);

                    BotMessage botMessage = this._messageBuilder.RemindersList(reminders.GetPage(timeZoneCallbackData.Page).ToList(), timeZoneCallbackData.Page);

                    await this._senderService.EditMessageAsync(chatId, messageId, botMessage);

                    break;

                default: this._logger.LogWarning($"handle navigation: unknown callback action: {timeZoneCallbackData.Action}"); break;
            }
        }
    }
}
