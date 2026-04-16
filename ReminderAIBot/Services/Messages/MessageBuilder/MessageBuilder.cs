using Microsoft.AspNetCore.Mvc.RazorPages;
using ReminderAIBot.Models.Callback.Enums;
using ReminderAIBot.Models.Callbacks.Enums;
using ReminderAIBot.Models.Database;
using ReminderAIBot.Models.Messages;
using ReminderAIBot.Services.Callbacks.CallbackDataBuilder;
using System.Formats.Tar;
using System.Text;


namespace ReminderAIBot.Services.Messages.MessageBuilder
{
    public class MessageBuilder : IMessageBuilder
    {
        private readonly ICallbackDataBuilder _callbackDataBuilder;


        public MessageBuilder(ICallbackDataBuilder callbackDataBuilder)
        {
            this._callbackDataBuilder = callbackDataBuilder;
        }


        public BotMessage NewReminderMessage(Reminder reminder)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Новое напоминание:");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"\"{reminder.Text}\"");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Напомнить");
            stringBuilder.AppendLine($"{reminder.RemindAt.Value.ToString("f")}");

            BotMessage botMessage = new BotMessage()
            {
                Text = stringBuilder.ToString(),
                Buttons = new List<List<MessageButton>>()
                {
                    new List<MessageButton>() 
                    { 
                        new MessageButton() { Text = "Создать", CallbackData = this._callbackDataBuilder.Reminder(ReminderAction.Add, reminder.Id) } 
                    },
                    new List<MessageButton>()
                    {
                        new MessageButton() { Text = "Удалить", CallbackData = this._callbackDataBuilder.Reminder(ReminderAction.Delete, reminder.Id) },
                    }
                }
            };

            return botMessage;
        }


        public BotMessage RemindersList(List<Reminder> reminders, int page)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Напоминаний: {reminders.Count}\n");

            BotMessage botMessage = new BotMessage()
            {
                Text = stringBuilder.ToString(),
                Buttons = new List<List<MessageButton>>()
            };

            foreach (Reminder reminder in reminders)
            {
                botMessage.Buttons.Add(new List<MessageButton>() { new MessageButton() { Text = $"[{reminder.RemindAt.ToString()}] {reminder.Text}" } });
            }

            int prevPage = page <= 0 ? 0 : page - 1;
            int nextPage = page <= 0 ? 1 : page + 1;

            botMessage.Buttons.Add(new List<MessageButton>() 
            { 
                new MessageButton() { Text = "<<", CallbackData = this._callbackDataBuilder.Navigation(UiScreen.Move, prevPage) },
                new MessageButton() { Text = ">>", CallbackData = this._callbackDataBuilder.Navigation(UiScreen.Move, nextPage) }
            });

            return botMessage;
        }

        public BotMessage TimeZonesList(List<TimeZoneInfo> timeZones, int page)
        {
            StringBuilder stringBuilder = new StringBuilder();

            BotMessage botMessage = new BotMessage()
            {
                Text = stringBuilder.ToString(),
                Buttons = new List<List<MessageButton>>()
            };

            foreach (TimeZoneInfo timeZone in timeZones)
            {
                botMessage.Buttons.Add(new List<MessageButton>() { new MessageButton() { Text = timeZone.DisplayName, CallbackData = this._callbackDataBuilder.TimeZone(TimeZoneAction.Set, timeZone.Id) } });
            }

            int prevPage = page <= 0 ? 0 : page - 1;
            int nextPage = page <= 0 ? 1 : page + 1;

            botMessage.Buttons.Add(new List<MessageButton>()
            {
                new MessageButton() { Text = "<<", CallbackData = this._callbackDataBuilder.Navigation(UiScreen.Move, prevPage) },
                new MessageButton() { Text = ">>", CallbackData = this._callbackDataBuilder.Navigation(UiScreen.Move, nextPage) }
            });

            return botMessage;
        }

    }
}
