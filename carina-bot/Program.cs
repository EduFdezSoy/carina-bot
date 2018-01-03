using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;
using CoreRCON;

namespace carina_bot
{
    class Program
    {
        // configuracion de telegram
        private static readonly TelegramBotClient Bot = new TelegramBotClient("98764321:qwertyuiop");

        // configuracion de mc 
        private static readonly string mc_ip = "123.123.123.123";
        private static readonly ushort mc_port = 25575;
        private static readonly string mc_pw = "asdfghjkl";
        private static readonly long mc_group = -123456;
        private static readonly string mc_log = "C:/Users/edufd/Desktop/latest.log";
           // "/home/ubuntu/minecraft/logs/latest.log";

        // Connect to mc server
        private static readonly RCON rcon = new RCON(IPAddress.Parse(mc_ip), mc_port, mc_pw);

        
        static void Main(string[] args)
        {
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnInlineQuery += BotOnInlineQueryReceived;
            Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            var me = Bot.GetMeAsync().Result;

            Console.Title = "@" + me.Username + " - Press Intro to exit";
            
            Bot.StartReceiving();
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.WriteLine();
            Console.WriteLine("Press Intro to exit.");
            LeerMsgMine();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static async void MandarMsgMine(string user, string msg)
        {
            // Send message
            string tarea = await rcon.SendCommandAsync("say §9<" + user + "> " + msg);
            Console.WriteLine($"{tarea}");
        }

        private static async void LeerMsgMine()
        {
            // pillamos el archivito (revisemos esto mas tarde, quizás nos conviene más hacerlo en el main o fuera del programa una sola vez)
            using (FileStream logFile = System.IO.File.Open(mc_log, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                long position = logFile.Length;

                // vamos a la posición que tenemos guardada
                logFile.Position = position;

                using (StreamReader reader = new StreamReader(logFile))
                {
                    string line;

                    do
                    {
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            if (line == "")
                                continue;

                            // si la linea no es de Chat, la ignoramos tambien
                            // tenemos que añadir el try catch por ciertas lineas salvajes que hagan petar
                            try
                            {
                                if (line.Substring(12, 17) != "Async Chat Thread")
                                    continue;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error: " + e.Message);
                                continue;
                            }

                            string texto = line.Substring(42, line.Length - 42);

                            Console.WriteLine(texto);
                            await Bot.SendTextMessageAsync(mc_group, texto);
                        }
                        position = logFile.Position; // guardamos la posicion donde nos encontramos en nuestra variable 

                        // a las 00:00:00 se resetea el log, y nosotros reseteamos el contador
                        if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                        {
                            Console.WriteLine("[{0}:{1}] - El log se ha reseteado, reiniciemos el contador.", DateTime.Now.Hour, DateTime.Now.Minute);
                            logFile.Position = 0;

                            // esperamos 1 segundo para que no vuelva a entrar aqui hasta el dia siguiente
                            await Task.Delay(1000);
                        }

                        await Task.Delay(250);

                    } while (true);
                }
            }
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.TextMessage) return;

            IReplyMarkup keyboard = new ReplyKeyboardRemove();

            switch (message.Text.Split(' ').First())
            {
                // send message to minecraft server
                case "/mc":
                    string texto = message.Text;
                    if (texto == "/mc")
                    {
                        await Bot.SendTextMessageAsync(message.Chat.Id, "manda algo con el /mc, vacío no me vale.");
                        await Task.Delay(250); // simulate longer running task
                        await Bot.SendTextMessageAsync(message.Chat.Id, "ejemplo: /mc penes");
                    }
                    else
                    { 
                        texto = texto.Substring(4, texto.Length - 4);
                        string usuario = message.From.Username;
                        MandarMsgMine(usuario, texto);
                    }
                    break;

                // return chat id
                case "/chat":
                    await Bot.SendTextMessageAsync(message.Chat.Id, "El id de este chat es " + message.Chat.Id);
                    break;

                // send inline keyboard
                case "/inline":
                    await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                    await Task.Delay(500); // simulate longer running task

                    keyboard = new InlineKeyboardMarkup(new[]
                    {
                        new [] // first row
                        {
                            InlineKeyboardButton.WithCallbackData("1.1"),
                            InlineKeyboardButton.WithCallbackData("1.2"),
                        },
                        new [] // second row
                        {
                            InlineKeyboardButton.WithCallbackData("2.1"),
                            InlineKeyboardButton.WithCallbackData("2.2"),
                        }
                    });

                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Choose",
                        replyMarkup: keyboard);
                    break;

                // send custom keyboard
                case "/keyboard":
                    keyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new [] // first row
                        {
                            new KeyboardButton("1.1"),
                            new KeyboardButton("1.2"),
                        },
                        new [] // last row
                        {
                            new KeyboardButton("2.1"),
                            new KeyboardButton("2.2"),
                        }
                    });

                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Choose",
                        replyMarkup: keyboard);
                    break;

                // send a photo
                case "/photo":
                    await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                    const string file = @"Files/tux.png";

                    var fileName = file.Split(Path.DirectorySeparatorChar).Last();

                    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var fts = new FileToSend(fileName, fileStream);

                        await Bot.SendPhotoAsync(
                            message.Chat.Id,
                            fts,
                            "Nice Picture");
                    }
                    break;

                // request location or contact
                case "/request":
                    keyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton("Location")
                        {
                            RequestLocation = true
                        },
                        new KeyboardButton("Contact")
                        {
                            RequestContact = true
                        },
                    });

                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Who or Where are you?",
                        replyMarkup: keyboard);
                    break;
                case "/help":
                default:
                    const string usage = @"Usage:
/mc msg - send a mesage to Minecraft Server
/chat   - return the chat ID of this chat

Test Commands:
/inline   - send inline keyboard
/keyboard - send custom keyboard
/photo    - send a photo
/request  - request location or contact
";

                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        usage,
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
            }
        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            await Bot.AnswerCallbackQueryAsync(
                callbackQueryEventArgs.CallbackQuery.Id,
                $"Received {callbackQueryEventArgs.CallbackQuery.Data}");
        }

        private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            Console.WriteLine($"Received inline query from: {inlineQueryEventArgs.InlineQuery.From.Id}");

            InlineQueryResult[] results = {
                new InlineQueryResultLocation
                {
                    Id = "1",
                    Latitude = 40.7058316f, // displayed result
                    Longitude = -74.2581888f,
                    Title = "New York",
                    InputMessageContent = new InputLocationMessageContent // message if result is selected
                    {
                        Latitude = 40.7058316f,
                        Longitude = -74.2581888f,
                    }
                },

                new InlineQueryResultLocation
                {
                    Id = "2",
                    Longitude = 52.507629f, // displayed result
                    Latitude = 13.1449577f,
                    Title = "Berlin",
                    InputMessageContent = new InputLocationMessageContent // message if result is selected
                    {
                        Longitude = 52.507629f,
                        Latitude = 13.1449577f
                    }
                }
            };

            await Bot.AnswerInlineQueryAsync(
                inlineQueryEventArgs.InlineQuery.Id,
                results,
                isPersonal: true,
                cacheTime: 0);
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message);
        }
    }
}
