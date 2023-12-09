using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;


namespace ServerCode
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server();
        }

        /* public void task1()
         {
             Message msg = new Message() { Text = "Hello", DateTime = DateTime.Now, NicknameFrom = "Aleksei", NicknameTo = "All" };
             string json = msg.SerializeMessageToJson();
             Console.WriteLine(json);
             Message? msgDeserialized = Message.DeserializeFromJson(json);
         }*/

        public static void Server()
        {
            UdpClient udpClient = new UdpClient(12345);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Console.WriteLine("Сервер ждет сообщение от клиента");
            while (true)
            {
                byte[] buffer = udpClient.Receive(ref iPEndPoint);
                if (buffer == null) break;
                Console.WriteLine($"Size of message is {buffer.Length}");
                var messageText = Encoding.UTF8.GetString(buffer);
                Message message = Message.DeserializeFromJson(messageText);
                message.Print();
                ServerFeedbackToClient(buffer.Length);
            }
        }

        public static void ServerFeedbackToClient(int messageSize)
        {
            UdpClient udpClient = new UdpClient();
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.2"), 1234);
            string messageText = messageSize.ToString();//send to client the recieved message size
            string json = JsonSerializer.Serialize(messageText);
            byte[] data = Encoding.UTF8.GetBytes(json);
            udpClient.Send(data, data.Length, iPEndPoint);
            
        }

    }
}
