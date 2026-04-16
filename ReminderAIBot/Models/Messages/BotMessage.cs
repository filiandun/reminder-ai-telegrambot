namespace ReminderAIBot.Models.Messages
{
    public class BotMessage
    {
        public required string Text { get; set; }
        public List<List<MessageButton>> Buttons { get; set; } = [];

        //public static implicit operator BotMessage(string text) => new BotMessage() { Text = text };  // круто, но вообще не нравится, так как в коде оно очень неочевидно выглядит, вот пример
                                                                                                        // await this._senderService.SendMessageAsync(update.Message.Chat.Id, this._messageBuilder.NewReminder(newReminder));
                                                                                                        //                                                                          *вот тут неявное преобразование*
    }
}
