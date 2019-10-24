using System;
using System.Net;
using System.IO;
using System.Linq;
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
            Console.WriteLine();
            MineToTelegramChat(false);
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static async void MandarMsgMine(string user, string msg)
        {
            // Send message
            string tarea = await rcon.SendCommandAsync("say §9<" + user + "> " + msg);
            Console.WriteLine($"{tarea}");
        }

 
		/// <summary>
        /// Mandar mensaje del Minecraft al Telegram
        /// </summary>
        private static async void MineToTelegramChat(bool forceReload)
        {
            do
            {
                try
                {
                    //Abro el fichero en modo lectura para que windows pueda leerlo.
                    using (var fs = new FileStream(mc_log, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var sr = new StreamReader(fs, System.Text.Encoding.UTF8))//Importante el UTF8 para que se pueda leer correctamente
                        {
                            sr.ReadToEnd();//Hace que el stream del flujo se vaya al final del archivo para que pueda leer las ultimas lineas
                            string ultimaLinea = sr.ReadLine();

                            string nuevaLinea = sr.ReadLine();

                            if (ultimaLinea != nuevaLinea && nuevaLinea != null && !nuevaLinea.Contains("[Rcon]"))//Cuando el stream termina de leer se pone a null y hace que escriba null en el telegram. Para ello si es null, no se escribe. Y si viene de RCON tampoco.
                            {
                                ultimaLinea = nuevaLinea;
                                await Bot.SendTextMessageAsync(mc_group, nuevaLinea, ParseMode.Default, false, true);
                            }
                            if (forceReload)
                            {
                                await Bot.SendTextMessageAsync(mc_group, "Se ha recargado el archivo log del servidor.");
                                forceReload = false;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    await Bot.SendTextMessageAsync(mc_group, "No se pudo acceder al archivo log, intentando volver a conectar. Manualmente puedes usar /reloadlog para intentar recargarlo.");
                }
            } while (true);
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
                case "/chatid":
                    await Bot.SendTextMessageAsync(message.Chat.Id, "El id de este chat es " + message.Chat.Id);
                    break;

                //Fuerza a recargar el log del servidor (en caso de que algo no vaya bien)
                case "/reloadlog":
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Recargando el módulo de log del bot...");
                    MineToTelegramChat(true);
                    break;
                case "/help":
                default:
                    const string usage = @"Usage:
/mc        - send a mesage to Minecraft Server
/chat      - return the chat ID of this chat
/status    - checks server estatus (online/offline)
/craft     - Send a photo with crafting recipe (ex: /craft planks)
/command   - Send teminal commands on the server.
/reloadlog - Manuelly reload the log file server.
/chatid    - Get this chat id.
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
