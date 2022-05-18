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
using System.IO;

namespace VIOrientemBot
{
    struct BotUpdate
    {
        public string text;
        public long id;
        public string? username;
    }
    class Person
    {
        public string Name { get; }
        public int Age { get; set; }
        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }
    internal class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("5383320170:AAG-6lKyg1tTJvYNvn9oYC_jnoUKYIszpKQ");       
        static string fileName = "updates.json";
        static bool logging = false;
      
        static List<BotUpdate> botUpdates = new List<BotUpdate>();       
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // string path = @"C:\Games\Тренировка.txt";
            // StreamWriter sw = new StreamWriter(path, true);
            //if (System.IO.File.Exists(path))
            //{
            //    // Если - да, то сообщаем об этом
            //    Console.WriteLine("\nФайл OutputTime.txt существует\n");
            //    Console.ReadKey();
            //    return
            //}
            //else
            //{
            //    // Если - нет, тоже сообщаем 
            //    Console.WriteLine("\nФайл Output.txt отсутствует!\nПриложение автоматически создаст его!\n");
            //    return
            //}
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                if (logging)
                {
                    Log(update);
                    logging = false;
                }
                var message = update.Message;
                if (message.Text.ToLower() == "/start")//нижний регистор ToLower
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать на борт, добрый путник!");
                    return;
                }
                if (message.Text == "Привет")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Привет-привет!!");
                    return;
                }
                if (message.Text.ToLower() == "/add")
                {
                    message = update.Message;
                    await botClient.SendTextMessageAsync(message.Chat, "А теперь отправь свою тренировку");
                    await botClient.SendTextMessageAsync(message.Chat, "Для отмены напиши /back");
                    if (message.Text.ToLower() != "/back")
                    { try

                        {
                            logging = true;
                            //sw.WriteLine(botUpdatesString);
                            // System.IO.File.AppendAllText(path, botUpdatesString);
                            await botClient.SendTextMessageAsync(message.Chat, "Тренировка записана");
                            return;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error reading or deserializing {ex}");
                        }
                    }

                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Запись отменена");
                        return;
                    }
                   
                    var receiverOptions = new ReceiverOptions
                    {
                        AllowedUpdates = new UpdateType[]
                {
                    UpdateType.Message,
                    UpdateType.EditedMessage,
                }
                    };                  
                }
                if(message.Text.ToLower() == "/read")      //получения списка тренировак
                {
                    StreamReader sr = new StreamReader("updates.json");                   
                    string line;                       
                    while ((line = sr.ReadLine()) != null)
                    await botClient.SendTextMessageAsync(message.Chat, line);
                    return;                    
                }                               
                if(message.Text.ToLower() == "/help")     //помощь 
                {
                    await botClient.SendTextMessageAsync(message.Chat, "/add - для записи тренировки");
                    await botClient.SendTextMessageAsync(message.Chat, "/help - для получения списка команд");
                    await botClient.SendTextMessageAsync(message.Chat, "/read - для получения списка всех тренировак");
                    return;
                }                
                await botClient.SendTextMessageAsync(message.Chat, "Попробуй другие команды");
            }
        }

        private static void Log(Update update)
        {
            Message? message = update.Message;
            System.IO.File.AppendAllText(fileName, $"\n{message.Chat.FirstName}: {message.Text}({DateTime.Now}) ");            
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {            
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
    }
}