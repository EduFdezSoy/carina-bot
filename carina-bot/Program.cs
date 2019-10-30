using CoreRCON;
using System;
using System.IO;
using System.Linq;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using Microsoft.Extensions.Configuration;



namespace carina_bot
{
    class Program
    {
        //Configuration
        static readonly IConfiguration config = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json")
          .Build();
        // configuracion de telegram
        private static readonly TelegramBotClient Bot = new TelegramBotClient(config["BOT_TOKEN"]);

        // configuracion de mc 
        private static readonly string mc_ip = config["RCON_IP"];
        private static readonly ushort mc_port = ushort.Parse(config["RCON_PORT"]);
        private static readonly string mc_pw = config["RCON_PASSWORD"];
        private static readonly long mc_group = long.Parse(config["TELEGRAM_GROUP_ID"]);
        private static readonly string mc_log = config["PATH_LOG_FILE"];
        private static readonly string craftImgFolderPath = config["PATH_CRAFTING_FOLDER"];

        // Connect to mc server
        private static readonly RCON rcon = new RCON(IPAddress.Parse(mc_ip), mc_port, mc_pw);


        //Mover a clase constantes
        private const string Si = "Sí estoy seguro.";
        private const string No = "No, llevame atrás.";

        static void Main()
        {

            //Eventos del Bot de telegram
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            var me = Bot.GetMeAsync().Result;

            Console.Title = "@" + me.Username + " - Press Enter to exit";

            Bot.StartReceiving();
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.WriteLine();
            Console.WriteLine("Press Intro to exit.");
            Console.WriteLine();
            MineToTelegramChat(false);
            Console.ReadLine();
            Bot.StopReceiving();
        }

        /// <summary>
        /// Mandar mensaje del telegram al Minecraft
        /// </summary>
        /// <param name="user">Usuario del telegram</param>
        /// <param name="msg">Mensaje a enviar</param>
        private static async void MandarMsgMine(string user, string msg)
        {
            string tarea = await rcon.SendCommandAsync("say §9<" + user + "> " + msg);
            Console.WriteLine($"{tarea}");
        }

        /// <summary>
        /// Mandar mensaje del telegram al Minecraft
        /// </summary>
        /// <param name="user">Usuario del telegram</param>
        /// <param name="msg">Mensaje a enviar</param>
        private static async void MandarMsgMine(string comando)
        {
            string tarea = await rcon.SendCommandAsync(comando);
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

        
        /// <summary>
        /// Metodo de control de comandos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="messageEventArgs"></param>
        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.TextMessage) return;


            switch (message.Text.Split(' ').First())
            {
                //Fuerza a recargar el log del servidor (en caso de que algo no vaya bien)
                case "/reloadlog":
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Recargando el módulo de log del bot...");
                    MineToTelegramChat(true);
                    break;

                //Envia mensaje al minecraft
                case "/mc":
                    string texto = message.Text;
                    if (texto == "/mc")
                    {
                        await Bot.SendTextMessageAsync(message.Chat.Id, "manda algo con el /mc, vacío no me vale.");
                        await Bot.SendTextMessageAsync(message.Chat.Id, "ejemplo: /mc penes");
                    }
                    else
                    {
                        texto = texto.Substring(4, texto.Length - 4);
                        string usuario = message.From.Username;
                        MandarMsgMine(usuario, texto);
                    }
                    break;

                //Devuelve el ID del chat
                case "/chat":
                    await Bot.SendTextMessageAsync(message.Chat.Id, "El id de este chat es " + message.Chat.Id);
                    break;

                //Comprobar el estado del servidor
                case "/status":
                    await Bot.SendTextMessageAsync(mc_group, ServerPing.GetPing(mc_ip));
                    break;



                //Envia una foto del crafteo que le pidas
                case "/craft":
                    BuscarCrafteo(message.Text, message.MessageId);
                    break;

                //Envia una foto del crafteo que le pidas
                case "/command":
                    string comando = message.Text;
                    Console.WriteLine(comando);
                    if (comando == "/command")
                    {
                        await Bot.SendTextMessageAsync(message.Chat.Id, "manda algo con el /command, vacío no me vale.");
                        await Bot.SendTextMessageAsync(message.Chat.Id, "ejemplo: /command /give");
                        await Bot.SendTextMessageAsync(message.Chat.Id, "SOLO el admin del grupo puede crear comandos");
                    }
                    else
                    {
                        //MandarComandoServer(comando, messageEventArgs);
                        string[] comandos = comando.Split(' ');
                        string aux = string.Empty;
                        for (int i = 1; i < comandos.Length; i++)
                        {
                            Console.WriteLine("[" + i + "]: " + comandos[i]);
                            aux += comandos[i] + " ";
                        }
                        MandarMsgMine(aux.TrimEnd());
                        await Bot.SendTextMessageAsync(message.Chat.Id, "Se ha aplicado este comando al servidor:\n" + aux.TrimEnd(), ParseMode.Default, false, false, message.MessageId);
                    }
                    break;

                //Envia un texto con los comandos básicos
                case "/help":
                default:
                    const string usage = @"Usage:
/mc msg - send a mesage to Minecraft Server
/chat   - return the chat ID of this chat
/status - checks server estatus (online/offline)
/craft  - Send a photo with crafting recipe (ex: /craft planks)
/command- Send teminal commands on the server.

";

                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        usage,
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
            }
        }

        private static async void MandarComandoServer(string comando, MessageEventArgs e)
        {
            await Bot.SendTextMessageAsync(e.Message.Chat.Id, "¿Estás seguro de querer ejecutar un comando en el servidor? ¡Eso son trampas! ", replyMarkup: new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton(No),
                    new KeyboardButton(Si),
                }));

            switch (e.Message.Text)
            {
                case No:
                    await Bot.SendTextMessageAsync(e.Message.Chat.Id, "Envio de comando cancelado");
                    break;
                case Si:
                    string[] comandos = comando.Split(' ');
                    string aux = string.Empty;
                    for (int i = 1; i < comandos.Length; i++)
                    {
                        Console.WriteLine("[" + i + "]: " + comandos[i]);
                        aux += comandos[i] + " ";
                    }
                    MandarMsgMine(aux.TrimEnd());
                    await Bot.SendTextMessageAsync(e.Message.Chat.Id, "Se ha aplicado este comando al servidor: " + aux.TrimEnd(), ParseMode.Default, false, false, e.Message.MessageId);
                    break;
            }
        }

        /// <summary>
        /// Este método busca en el directorio de los crafteos, el pedido por el usuario y lo manda al telegram.
        /// </summary>
        /// <param name="craftName">Nombre del crafteo</param>
        /// <param name="id">Id del mensaje</param>
        private static async void BuscarCrafteo(string craftName, int id)
        {
            if (craftName == "/craft")
            {

                await Bot.SendTextMessageAsync(mc_group, "te enseña un crafteo /craft, vacío no me vale.");
                await Bot.SendTextMessageAsync(mc_group, "ejemplo: /craft planks");
            }
            else
            {

                Console.WriteLine(craftName);
                string separadas;

                separadas = craftName.Split(' ').Last();
                Console.WriteLine(separadas);

                try
                {
                    DirectoryInfo imgCraftFolder = new DirectoryInfo(craftImgFolderPath);
                    FileInfo[] filesInDir = imgCraftFolder.GetFiles(separadas + "*.*");

                    foreach (FileInfo foundFile in filesInDir)
                    {
                        craftName = foundFile.FullName;
                        Console.WriteLine("Buscando la imagen: " + craftName);
                    }


                    using (var stream = System.IO.File.Open(craftName, FileMode.Open))
                    {
                        FileToSend fts = new FileToSend();
                        fts.Content = stream;
                        fts.Filename = craftName.Split('\\').Last();
                        if (Path.GetExtension(craftName) == ".gif")
                            await Bot.SendVideoAsync(mc_group, fts, 10, 0, 0, null, true, id);
                        else
                            await Bot.SendPhotoAsync(mc_group, fts, null, true, id);
                    }
                }
                catch (FileNotFoundException)
                {
                    await Bot.SendTextMessageAsync(mc_group, "Ese crafteo no existe o no está implementado, anormal.");
                }
                catch (Exception)
                {
                    await Bot.SendTextMessageAsync(mc_group, "Algo ha ido mal.");
                }
            }
        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            await Bot.AnswerCallbackQueryAsync(
                callbackQueryEventArgs.CallbackQuery.Id,
                $"Received {callbackQueryEventArgs.CallbackQuery.Data}");
        }



        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message);
        }
    }
}
