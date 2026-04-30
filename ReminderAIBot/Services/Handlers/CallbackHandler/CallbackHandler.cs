using ReminderAIBot.Models.Messages;
using ReminderAIBot.Models.UI.ScreenModel;
using ReminderAIBot.Models.CallbackCommands;

using ReminderAIBot.Services.Messenger.SenderService;
using ReminderAIBot.Services.Messenger.ScreenRenderer;
using ReminderAIBot.Services.Callbacks.CallbackDataCodec;
using ReminderAIBot.Services.Applications.HomeApplicationService;
using ReminderAIBot.Services.Applications.ReminderApplicationService;


namespace ReminderAIBot.Services.Handlers.CallbackHandler
{
    public class CallbackHandler : ICallbackHandler
    {
        private readonly ILogger<CallbackHandler> _logger;

        private readonly IHomeApplicationService _homeApplicationService;
        private readonly IReminderApplicationService _reminderApplicationService;

        private readonly IScreenRenderer _screenRenderer;
        private readonly ISenderService _senderService;


        public CallbackHandler(ILogger<CallbackHandler> logger, IHomeApplicationService homeApplicationService, IReminderApplicationService reminderApplicationService,  IScreenRenderer messageBuilder, ISenderService senderService)
        {
            this._logger = logger;

            this._homeApplicationService = homeApplicationService;
            this._reminderApplicationService = reminderApplicationService;

            this._senderService = senderService;
            this._screenRenderer = messageBuilder;
        }


        public async Task HandleAsync(long chatId, int messageId, string callbackQueryId, string? data)
        {
            if (data is null)
            {
                this._logger.LogWarning("handle: callback data is null");
                return;
            }

            CallbackCommand? callbackCommand = CallbackDataCodec.Decode(data);

            if (callbackCommand is null)
            {
                this._logger.LogWarning("handle: callback command is null");
                return;
            }

            switch (callbackCommand)
            {
                case OpenHomeCommand: await this.HandleOpenAsync(await this._homeApplicationService.BuildHomeScreenModelAsync(chatId), chatId, messageId); break;

                case OpenRemindersListCommand command: await this.HandleOpenAsync(await this._reminderApplicationService.BuildRemindersListScreenModelAsync(chatId, command.Page), chatId, messageId); break;
                case OpenReminderDetailsCommand command: await this.HandleOpenAsync(await this._reminderApplicationService.BuildReminderScreenModelAsync(chatId, command.ReminderId), chatId, messageId); break;

                //case CreateReminderCommand createReminderCommand: await this.HandleCreateReminderAsync(createReminderCommand, chatId, messageId); break;
                case EditReminderCommand editReminderCommand: await this._senderService.AnswerCallbackQuery(callbackQueryId, "TODO"); break;
                case DeleteReminderCommand deleteReminderCommand: await this._senderService.AnswerCallbackQuery(callbackQueryId, "Напоминание удалено"); break;

                //case OpenTimeZoneListCommand openTimeZoneListCommand: await this.HandleOpenAsync(await this._settingsApplicationService.Build(), chatId, messageId); break;
                //case SetTimeZoneCommand setTimeZoneCommand: await this.HandleSetTimeZoneAsync(setTimeZoneCommand, chatId, messageId); break;

                default: this._logger.LogWarning($"handle: unknown type callback data: ({callbackCommand.GetType()})"); break;
            }
        }

        private async Task HandleOpenAsync(ScreenModel screenModel, long chatId, int messageId)
        {
            RenderedMessage renderedMessage = this._screenRenderer.Render(screenModel);

            await this._senderService.EditMessageAsync(chatId, messageId, renderedMessage);
        }
    }
}
