using System.Text.Json;
using ReminderAIBot.Models.Database;


namespace ReminderAIBot.Services.Repositories.ReminderRepository
{
    public class LocalReminderRepository : IReminderRepository
    {
        private readonly ILogger<LocalReminderRepository> _logger;

        private readonly string _fileDb;
        private List<Reminder> _db;


        public LocalReminderRepository(ILogger<LocalReminderRepository> logger)
        {
            this._logger = logger;

            this._fileDb = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reminderdb.json");
            this.Load();
        }


        public async Task<List<Reminder>> GetByUserId(long userId) => this._db.Where(r => r.UserId == userId).ToList();

        public async Task Add(Reminder reminder)
        {
            this._db.Add(reminder);

            this.Save();
        }

        public Task Update(Reminder reminder)
        {
            throw new NotImplementedException();
        }

        public async Task Remove(int reminderId)
        {
            Reminder reminder = this._db.FirstOrDefault(r => r.Id == reminderId) ?? throw new Exception("remove: reminder not found");

            this._db.Remove(reminder);

            this.Save();
        }


        private void Save()
        {
            string jsonDb = JsonSerializer.Serialize(this._db);

            File.WriteAllText(this._fileDb, jsonDb);
        }

        private void Load()
        {
            if (!File.Exists(this._fileDb))
            {
                this._logger.LogDebug($"fileDb not found {this._fileDb}");
                this._db = new List<Reminder>();

                return;
            }

            try
            {
                var json = File.ReadAllText(this._fileDb);
                var data = JsonSerializer.Deserialize<List<Reminder>>(json);

                this._db = data ?? new List<Reminder>();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
            }
        }
    }
}
