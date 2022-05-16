using Newtonsoft.Json;
using System;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VIOrientemBot
{
    struct BotUpdate
    {
        public string text;
        public long id;
        public string? username;
    }
    internal class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("5383320170:AAG-6lKyg1tTJvYNvn9oYC_jnoUKYIszpKQ");       
        static string fileName = "updates.json";
        static List<BotUpdate> botUpdates = new List<BotUpdate>();       
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            
            // Некоторые действия
             Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text.ToLower() == "/start")//нижний регистор ToLower
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать на борт, добрый путник!");
                    return;
                }
                if (message.Text.ToLower() == "Привет")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Привет-привет!!");
                    return;
                }
                if (message.Text.ToLower() == "/тренировка")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "А теперь отправь свою тренировку");
                    try
                    {                       
                        var botUpdatesString = System.IO.File.ReadAllText(fileName);
                        botUpdates = JsonConvert.DeserializeObject<List<BotUpdate>>(botUpdatesString) ?? botUpdates;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading or deserializing {ex}");
                    }
                    var receiverOptions = new ReceiverOptions
                    {
                        AllowedUpdates = new UpdateType[]
                {
                    UpdateType.Message,
                    UpdateType.EditedMessage,
                }
                    };
                    //message = update.Message;
                    //string workout = message.Text;
                    //sw.WriteLine(workout);

                }
                if(message.Text.ToLower() == "/help")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "/тренировка - для записи тренировки");
                    await botClient.SendTextMessageAsync(message.Chat, "/help - для получения списка команд");
                }                
                await botClient.SendTextMessageAsync(message.Chat, "Попробуй другие команды");
            }
        }
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
        static void Main(string[] args)
        {            
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[]
                {
                    UpdateType.Message,
                    UpdateType.EditedMessage,
                }
            };

            bot.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions);            
            //
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
             receiverOptions = new ReceiverOptions
            {
               AllowedUpdates = { }
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
        private static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken arg3)
        {
            if (update.Type == UpdateType.Message)
            {
                if (update.Message.Type == MessageType.Text)
                {
                    var _botUpdate = new BotUpdate
                    {
                        text = update.Message.Text,
                        id = update.Message.Chat.Id,
                        username = update.Message.Chat.Username
                    };

                    botUpdates.Add(_botUpdate);

                    var botUpdatesString = JsonConvert.SerializeObject(botUpdates);

                    System.IO.File.WriteAllText(fileName, botUpdatesString);
                }
            }
        }
        private static Task ErrorHandler(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}