using System.Text;

using ReminderAIBot.Models.Database;
using ReminderAIBot.Models.Messages;
using ReminderAIBot.Models.UI.ScreenModel;
using ReminderAIBot.Models.CallbackCommands;

using ReminderAIBot.Services.Callbacks.CallbackDataCodec;


namespace ReminderAIBot.Services.Messenger.ScreenRenderer
{
    public class ScreenRenderer : IScreenRenderer
    {
        public ScreenRenderer()
        {

        }


        public RenderedMessage Render(ScreenModel model)
        {
            return model switch
            {
                HomeScreenModel homeScreenModel => this.RenderHome(homeScreenModel),
                RemindersListScreenModel remindersListScreenModel => this.RenderRemindersList(remindersListScreenModel),
                ReminderScreenModel reminderScreenModel => this.RenderReminder(reminderScreenModel),

                _ => throw new NotSupportedException($"render: unsupported screen model type: {model.GetType().Name}")
            };
        }

        private RenderedMessage RenderHome(HomeScreenModel model)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(model.Title);
            stringBuilder.AppendLine(model.Text);


            List<InlineButtonRow> buttons = new List<InlineButtonRow>()
            {
                new InlineButtonRow() 
                { 
                    InlineButtons = new List<InlineButton>()
                    {
                        new InlineButton() { Text = $"Список напоминаний [{model.RemindersCount}]", CallbackData = CallbackDataCodec.Encode(new OpenRemindersListCommand(0)) } 
                    }
                },
            };


            RenderedMessage botMessage = new RenderedMessage()
            {
                Text = stringBuilder.ToString(),
                InlineButtonRows = buttons
            };

            return botMessage;
        }

        private RenderedMessage RenderRemindersList(RemindersListScreenModel model)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(model.Title);
            stringBuilder.AppendLine(model.Text);


            List<InlineButtonRow> buttons = new();

            if (model.Reminders is not null)
            {
                foreach (Reminder reminder in model.Reminders)
                {
                    buttons.Add
                    (
                        new InlineButtonRow()
                        {
                            InlineButtons = new List<InlineButton>()
                            {
                                new InlineButton() { Text = $"[{reminder.RemindAt.ToString("g")}]{reminder.Text}", CallbackData = CallbackDataCodec.Encode(new OpenReminderDetailsCommand(reminder.Id)) }
                            }
                        }
                    );
                }
            }

            //
            List<InlineButton> paginationButtons = new();

            if (model.HasPrevPage) paginationButtons.Add(new InlineButton() { Text = "<<", CallbackData = CallbackDataCodec.Encode(new OpenRemindersListCommand(model.CurrentPage - 1)) });

            paginationButtons.Add(new InlineButton() { Text = $"{model.CurrentPage + 1} из {model.TotalPages + 1}", CallbackData = "-" });

            if (model.HasNextPage) paginationButtons.Add(new InlineButton() { Text = ">>", CallbackData = CallbackDataCodec.Encode(new OpenRemindersListCommand(model.CurrentPage + 1)) });


            InlineButtonRow paginationButtonRow = new()
            {
                InlineButtons = paginationButtons
            };
                
            buttons.Add(paginationButtonRow);

            //
            buttons.Add(new InlineButtonRow()
            {
                InlineButtons = new List<InlineButton>()
                {
                    new InlineButton() { Text = $"Назад", CallbackData = CallbackDataCodec.Encode(new OpenHomeCommand()) }
                }
            });

            RenderedMessage botMessage = new RenderedMessage()
            {
                Text = stringBuilder.ToString(),
                InlineButtonRows = buttons
            };

            return botMessage;
        }

        private RenderedMessage RenderReminder(ReminderScreenModel model)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(model.Title);
            stringBuilder.AppendLine(model.Text);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"\"{model.ReminderText}\"");
            stringBuilder.AppendLine($"Напомнить: {model.RemindAt}");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"Создано: {model.CreatedAt}");
            stringBuilder.AppendLine($"Изначальный запрос: {model.RawText}");


            List<InlineButtonRow> buttons = new List<InlineButtonRow>()
            {
                new InlineButtonRow() 
                { 
                    InlineButtons = new List<InlineButton>() 
                    {
                        new InlineButton() { Text = $"Редактировать", CallbackData = CallbackDataCodec.Encode(new EditReminderCommand(model.ReminderId)) },
                        new InlineButton() { Text = $"Удалить", CallbackData = CallbackDataCodec.Encode(new DeleteReminderCommand(model.ReminderId)) },
                        new InlineButton() { Text = $"Выключить", CallbackData = "-"},
                    },
                },
            };

            //
            buttons.Add(new InlineButtonRow()
            {
                InlineButtons = new List<InlineButton>()
                {
                    new InlineButton() { Text = $"Назад", CallbackData = CallbackDataCodec.Encode(new OpenRemindersListCommand(0)) }
                }
            });

            RenderedMessage botMessage = new RenderedMessage()
            {
                Text = stringBuilder.ToString(),
                InlineButtonRows = buttons
            };

            return botMessage;
        }
    }
}
