using ReminderAIBot.Models.Callback.Enums;
using ReminderAIBot.Models.Callbacks;
using ReminderAIBot.Models.Callbacks.Domains;
using ReminderAIBot.Models.Callbacks.Enums;


namespace ReminderAIBot.Services.Callbacks.CallbackDataParser
{
    public class CallbackDataParser : ICallbackDataParser
    {
        private readonly ILogger<CallbackDataParser> _logger;


        public CallbackDataParser(ILogger<CallbackDataParser> logger)
        {
            this._logger = logger;
        }

        // TODO пиздец костыли, надо переделать
        public CallbackData? Parse(string data)
        {
            CallbackData? result = null;

            result = this.ParseReminder(data);
            if (result is not null) return result;

            result = this.ParsePage(data);
            if (result is not null) return result;

            this._logger.LogWarning($"parse: unknown callbackdata ({data})");
            return null;
        }


        private ReminderCallbackData? ParseReminder(string data)
        {
            var parts = data.Split(':');

            if (parts.Length != 3 || parts[0] != "rem") return null;

            if (!int.TryParse(parts[1], out var action)) return null;

            if (!int.TryParse(parts[2], out var reminderId)) return null;

            return new ReminderCallbackData { Action = (ReminderAction)action, ReminderId = reminderId };
        }


        private ChangePageCallbackData? ParsePage(string data)
        {
            var parts = data.Split(':');

            if (parts.Length != 4 || parts[0] != "nav") return null;

            if (!int.TryParse(parts[1], out var action)) return null;

            if (!int.TryParse(parts[2], out var type)) return null;

            if (!int.TryParse(parts[3], out var page)) return null;

            return new ChangePageCallbackData { Action = (UiScreen)action, Item = (NavigationItem)type, Page = page };
        }
    }
}
