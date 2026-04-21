using System.Collections;

namespace ReminderAIBot.Models.Messages
{
    public class InlineButtonRow : IEnumerable<InlineButton>
    {
        public List<InlineButton> InlineButtons { get; set; } = new();


        public IEnumerator<InlineButton> GetEnumerator()
        {
            foreach (InlineButton inlineButton in this.InlineButtons)
            {
                yield return inlineButton;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
