using DotNetEnv;

using Telegram.Bot;

using IAIChatServiceLib;
using GigaChatServiceLib;
using GigaChatServiceLib.Models.Config;

using ReminderAIBot.Models;
using ReminderAIBot.Services.ReminderParser;
using ReminderAIBot.Services.ReminderService;
using ReminderAIBot.Services.OnboardingService;
using ReminderAIBot.Services.Messenger.SenderService;
using ReminderAIBot.Services.Messenger.RecieverService;
using ReminderAIBot.Services.Repositories.ReminderRepository;
using ReminderAIBot.Services.Repositories.UserRepository;
using ReminderAIBot.Services.Messages.MessageHandler;
using ReminderAIBot.Services.Messages.MessageBuilder;
using ReminderAIBot.Services.Callbacks.CallbackDataParser;
using ReminderAIBot.Services.Callbacks.CallbackDataBuilder;
using ReminderAIBot.Services.Messages.MessageService;
using ReminderAIBot.Services.Callbacks.CallbackService;


namespace ReminderAIBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load("secret.env"); // PS NuGET для подтягивания переменных окружения из файла

            var builder = WebApplication.CreateBuilder(args);

            // Logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));


            // Add TG bot config
            builder.Services.Configure<TelegramBotConfig>(config =>
            {
                config.Token = builder.Configuration["TELEGRAM_BOT_TOKEN"] ?? throw new InvalidOperationException("Env TELEGRAM_BOT_TOKEN is not set");
                config.WebhookUrl = builder.Configuration["Telegram:WebhookUrl"] ?? throw new InvalidOperationException("Opt Telegram.WebhookUrl is not set");
            });


            // Add services to the container.
            builder.Services.AddSingleton<IUserRepository, LocalUserRepository>();
            builder.Services.AddSingleton<IReminderRepository, LocalReminderRepository>();

            builder.Services.AddSingleton<IMessageHandler, MessageHandler>();

            builder.Services.AddSingleton<IReminderParser, ReminderParser>();
            builder.Services.AddSingleton<IReminderService, ReminderService>();

            builder.Services.AddSingleton<IMessageBuilder, MessageBuilder>();

            builder.Services.AddSingleton<ICallbackDataBuilder, CallbackDataBuilder>();
            builder.Services.AddSingleton<ICallbackDataParser, CallbackDataParser>();

            builder.Services.AddSingleton<IOnboardingService, OnboardingService>();

            builder.Services.AddSingleton<IMessageService, MessageService>();
            builder.Services.AddSingleton<ICallbackService, CallbackService>();


            builder.Services.AddSingleton<GigaChatConfig>();

            builder.Services.AddHttpClient<IAIChatService, GigaChatService>().ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator // TODO: remove in prod
            });


            // add TG bot client
            builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>(sp =>
            {
                IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
                string token = configuration["TELEGRAM_BOT_TOKEN"] ?? throw new InvalidOperationException("Env TELEGRAM_BOT_TOKEN is not set"); ;
                return new TelegramBotClient(token);
            });

            builder.Services.AddSingleton<ISenderService, TelegramSenderService>();

            builder.Services.AddHostedService<TelegramReceiverService>();
            //builder.Services.AddHostedService<ReminderWorker>();

            //
            builder.Services.AddControllers();


            var app = builder.Build();

            app.UseHttpsRedirection();
            app.MapControllers();

            app.Run();
        }
    }
}