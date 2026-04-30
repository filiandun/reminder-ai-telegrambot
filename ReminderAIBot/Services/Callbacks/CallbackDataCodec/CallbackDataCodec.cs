using ReminderAIBot.Models;
using ReminderAIBot.Models.CallbackCommands;


namespace ReminderAIBot.Services.Callbacks.CallbackDataCodec
{
    public static class CallbackDataCodec
    {
        public static string Encode(CallbackCommand command)
        {
            return command switch
            {
                OpenHomeCommand => CallbackRoutes.OpenHome,

                OpenRemindersListCommand openRemindersListCommand => CallbackRoutes.OpenRemindersList + $":{openRemindersListCommand.Page}",
                OpenReminderDetailsCommand openReminderDetailsCommand => CallbackRoutes.OpenReminderDetails + $":{openReminderDetailsCommand.ReminderId}",

                CreateReminderCommand => CallbackRoutes.CreateReminder,
                EditReminderCommand  editReminderCommand => CallbackRoutes.EditReminder + $":{editReminderCommand.ReminderId}",
                DeleteReminderCommand deleteReminderCommand => CallbackRoutes.DeleteReminder + $":{deleteReminderCommand.ReminderId}",

                OpenTimeZoneListCommand openTimeZoneListCommand => CallbackRoutes.OpenTimeZonesList + $":{openTimeZoneListCommand.Page}",
                SetTimeZoneCommand setTimeZoneCommand => CallbackRoutes.SetTimeZone + $":{setTimeZoneCommand.TimeZoneId}",

                _ => throw new ArgumentOutOfRangeException(nameof(command), command, null)
            };
        }

        // TODO сделать безопаснее, сейчас int.Parse - вообще небезопасно
        public static CallbackCommand? Decode(string data)
        {
            string[] parts = data.Split(':');

            if (parts.Length == 0) return null;

            return parts[0] switch
            {
                CallbackRoutes.OpenHome => new OpenHomeCommand(),
                CallbackRoutes.OpenRemindersList => new OpenRemindersListCommand(int.Parse(parts[1])),
                CallbackRoutes.OpenReminderDetails => new OpenReminderDetailsCommand(int.Parse(parts[1])),
                CallbackRoutes.OpenTimeZonesList => new OpenTimeZoneListCommand(int.Parse(parts[1])),

                CallbackRoutes.CreateReminder => new CreateReminderCommand(),
                CallbackRoutes.EditReminder => new EditReminderCommand(int.Parse(parts[1])),
                CallbackRoutes.DeleteReminder => new DeleteReminderCommand(int.Parse(parts[1])),

                CallbackRoutes.SetTimeZone => new SetTimeZoneCommand(parts[2]),

                _ => null
            };
        }
    }
}