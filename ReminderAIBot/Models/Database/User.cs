using System.Text.Json.Serialization;

namespace ReminderAIBot.Models.Database
{
    public record User
    {
        public int Id { get; set; }

        public long TelegramId { get; set; }

        public string? TimeZoneId { get; set; }


        [JsonIgnore]
        public TimeZoneInfo? TimeZone => this.TimeZoneId is null ? null : TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
    }
}
