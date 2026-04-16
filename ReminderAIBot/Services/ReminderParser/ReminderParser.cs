using System.Text.Json;

using IAIChatServiceLib;
using ReminderAIBot.Models;
using IAIChatServiceLib.Models;
using ReminderAIBot.Models.Database;
using System.Linq.Expressions;


namespace ReminderAIBot.Services.ReminderParser
{
    public class ReminderParser : IReminderParser
    {
        private readonly ILogger<ReminderParser> _logger;

        private readonly IAIChatService _AIChatService;

        private string _template = "Ты — сервис для парсинга напоминаний." +
            "Твоя задача: преобразовать текст пользователя в структурированный JSON." +
            "Правила:" +
            "1. Отвечай ТОЛЬКО валидным JSON, без пояснений, без текста до и после, без комментариев //." +
            "2. Если данных недостаточно — заполняй поля null." +
            "3. Дата и время должны быть в ISO 8601 формате (YYYY-MM-DDTHH:mm:ss)." +
            "4. Если пользователь не указал дату, но указал время — используй ближайшую будущую дату." +
            "5. Если указано \"завтра\", \"сегодня\", \"через X минут/часов\" — корректно интерпретируй." +
            "6. Текущая дата и время: {{CURRENT_DATETIME}}" +
            "Формат ответа:\r\n{\r\n  \"Text\": \"string\",          " +
            "// текст напоминания\r\n  \"RemindAt\": \"string|null\", " +
            "// дата и время\r\n  \"IsValid\": true|false      " +
            "// удалось ли распарсить\r\n}\r\n\r\n" +
            "Примеры:" +
            "\r\n\r\n" +
            "Вход:\r\n\"Напомни купить молоко завтра в 16:00\"\r\n\r\n" +
            "Выход:\r\n{\r\n  \"text\": \"купить молоко\",\r\n  \"datetime\": \"2026-03-28T16:00:00\",\r\n  \"isValid\": true\r\n}\r\n\r\n" +
            "Вход:\r\n\"Позвонить маме\"\r\n\r\n" +
            "Выход:\r\n{\r\n  \"text\": \"позвонить маме\",\r\n  \"datetime\": null,\r\n  \"isValid\": false\r\n}\r\n\r\n" +
            "Теперь обработай сообщение пользователя:\r\n\"{{USER_INPUT}}\"";


        public ReminderParser(ILogger<ReminderParser> logger, IAIChatService AIChatService)
        {
            this._logger = logger;

            this._AIChatService = AIChatService;
        }

        // TODO добавить базовый парсинг, чтобы не дёргать AI, кодгда сообщение приходят уровня "как дела?".
        public async Task<Reminder> ParseAsync(string rawText)
        {
            int balance = await this._AIChatService.GetBalanceTokenAsync();
            this._logger.LogInformation($"balance - {balance}");

            AIRequest AIRequest = this.MapToAIRequest("user", rawText);

            AIResponse AIResponse = await this._AIChatService.GetResponseAsync(AIRequest);

            return this.MapToReminder(AIResponse, rawText);
        }


        private AIRequest MapToAIRequest(string role, string text)
        {
            AIRequest AIRequest = new AIRequest()
            {
                Messages = new List<AIMessage>() 
                { 
                    new AIMessage() { Role = "system", Content = this._template.Replace("{{USER_INPUT}}", text).Replace("{{CURRENT_DATETIME}}", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")) },
                }
            };

            return AIRequest;
        }

        // TODO очень важно проверять валидность JSON и требовать заново сгенерировать от ИИ:
        // очень часто он любит добавить комментарий или какие-нибудь ещё символы добавить
        // пример метода для очистки в самом низу (CleanJson)
        private Reminder MapToReminder(AIResponse AIResponse, string rawText)
        {
            // TODO добавить try catch
            ReminderDraft? reminderDraft = JsonSerializer.Deserialize<ReminderDraft>(AIResponse.Message.Content);

            if (reminderDraft is null)
            {
                this._logger.LogCritical("Уёбище на GigaChat вставил свой комментарий в JSON БЛЯТЬ, 99.9%");
                return new Reminder();
            }

            Reminder reminder = new Reminder()
            {
                RemindAt = reminderDraft.RemindAt,
                CreatedAt = DateTime.Now,
                RawText = rawText,
                Text = reminderDraft.Text,
            };

            return reminder;
        }


        private string CleanJson(string input)
        {
            input = input.Replace("```json", "").Replace("```", "");

            int start = input.IndexOf('{');
            if (start >= 0)
                input = input[start..];

            int end = input.LastIndexOf('}');
            if (end >= 0)
                input = input[..(end + 1)];

            return input.Trim();
        }
    }
}
