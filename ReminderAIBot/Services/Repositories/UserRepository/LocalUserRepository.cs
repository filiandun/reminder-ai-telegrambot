using System.Text.Json;
using ReminderAIBot.Models.Database;


namespace ReminderAIBot.Services.Repositories.UserRepository
{
    public class LocalUserRepository : IUserRepository
    {
        private readonly ILogger<LocalUserRepository> _logger;

        private readonly string _fileDb;
        private List<User> _db;


        public LocalUserRepository(ILogger<LocalUserRepository> logger)
        {
            this._logger = logger;

            this._fileDb = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "userdb.json");
            this.Load();
        }


        public async Task<User?> GetByTelegramId(long telegramId) => this._db.FirstOrDefault(u => u.TelegramId == telegramId);

        public async Task Add(User user)
        {
            if (this._db.Contains(user)) throw new Exception("add: user already on db");

            this._db.Add(user);

            this.Save();
        }

        public async Task Remove(User user)
        {
            if (!this._db.Remove(user)) throw new Exception("remove: user not found on db");

            this.Save();
        }

        public async Task Update(User user)
        {
            User? originalUser = this._db.FirstOrDefault(user);

            if (originalUser is null) throw new Exception("update: user not found on db");

            originalUser.TelegramId = user.TelegramId;
            originalUser.TimeZoneId = user.TimeZoneId;

            this.Save();
        }


        private void Save()
        {
            try
            {
                string jsonDb = JsonSerializer.Serialize(this._db);

                File.WriteAllText(this._fileDb, jsonDb);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
            }
        }

        private void Load()
        {
            if (!File.Exists(this._fileDb))
            {
                this._logger.LogDebug($"load: fileDb not found {this._fileDb}");
                this._db = new List<User>();

                return;
            }

            try
            {
                var json = File.ReadAllText(this._fileDb);
                var data = JsonSerializer.Deserialize<List<User>> (json);

                this._db = data ?? new List<User>();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);

                this._db = new List<User>();
            }
        }
    }
}
