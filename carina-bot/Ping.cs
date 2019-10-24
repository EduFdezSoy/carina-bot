using System.Net.NetworkInformation;

namespace carina_bot
{
    public class ServerPing
    {
        public static string GetPing(string ip)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(ip);
                pingable = reply.Status == IPStatus.Success;

                if (pingable)
                {
                    return DisplayReply(reply);
                }
                else
                    return "Servidor no disponible en estos momentos, ¿Seguro que está online?";
            }
            catch (PingException)
            {
                return "Ping fallido, seguro pusiste mal la IP, ¿tienes el bachiller?";
            }
            catch (System.ArgumentNullException)
            {
                return "La IP no puede ser null, idiota.";
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }
        }



        private static string DisplayReply(PingReply reply)
        {
            string aux = string.Empty;
            if (reply == null)
                aux = "El objeto PingReply está vacio. Muero en silencio sin explotar.";
            else
            {
                try
                {
                    aux += string.Format("El servidor está {0}", (reply.Status == IPStatus.Success) ? "Online, deberias estar picando." : "Caido, better call Edu") + "\n";
                }
                catch (System.Exception)
                {
                    System.Console.WriteLine("He muerto");
                }
            }

            return aux;
        }


    }
}
