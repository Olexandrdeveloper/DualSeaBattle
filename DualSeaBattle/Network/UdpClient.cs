using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using DualSeaBattle.Model;

namespace DualSeaBattle.Network
{    
    public class ClientUdp : IDisposable
    {
        private readonly UdpClient Client;
        private readonly IPEndPoint ServerEndPoint;
           
        public ClientUdp(string serverIp, int port)
        {
            ServerEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), port);
            Client = new UdpClient(0);  
            Console.WriteLine($" Подключение к {serverIp}:{port}");

            //var network = new ClientUdp ("127.0.0.1", 5000);
        }

        public void SendMessage(Message msg)
        {
            var json = JsonSerializer.Serialize(msg);
            var bytes = Encoding.UTF8.GetBytes(json);

            Console.WriteLine($"[CLIENT] Отправка: {json}");

            Client.Send(bytes, bytes.Length, ServerEndPoint);
        }
        //***********************
        public Message? TryReceiveMessage()
        {            
            try
            {
                if (Client == null)
                {
                    Console.WriteLine("[CLIENT] ERROR (Ошибка)");
                    return null;
                }
                if (Client.Available <= 0)
                {
                    return null;
                }

                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] result = Client.Receive(ref remoteEP);

                Console.WriteLine($"[CLIENT] Получено {remoteEP}, размер: {result.Length} байт");

                if (!remoteEP.Equals(ServerEndPoint))
                {
                    Console.WriteLine("[CLIENT] Пакет не принят: неизвестный отправитель");
                    return null;
                }

                string json = Encoding.UTF8.GetString(result);
                Console.WriteLine($"[CLIENT] JSON: {json}");

                return JsonSerializer.Deserialize<Message>(json);
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine($"[CLIENT] Сокет закрыт: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[CLIENT] Ошибка JSON: {ex.Message}");
                return null;
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"[CLIENT] Ошибка сокета: {ex.Message}");
                return null;
            }
        }
        public void Dispose()
        {
            Client?.Close();
        }
    }    
}
