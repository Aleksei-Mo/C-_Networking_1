using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using ServerCode;



namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SentMessage(args[0], args[1]);
        }


        public static void SentMessage(string From, string ip)
        {
            UdpClient udpClient = new UdpClient();
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), 12345);
            while (true)
            {
                string messageText = System.String.Empty;
                do
                {
                    //Console.Clear();
                    Console.WriteLine("Введите сообщение.");
                    messageText = Console.ReadLine();
                }
                while (string.IsNullOrEmpty(messageText));
                Message message = new Message() { Text = messageText, NicknameFrom = From, NicknameTo = "Server", DateTime = DateTime.Now };
                string json = message.SerializeMessageToJson();
                byte[] data = Encoding.UTF8.GetBytes(json);
                int messageSize = udpClient.Send(data, data.Length, iPEndPoint);
                if (messageText.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("The chat session will be closed.");
                    return;
                }
                Console.WriteLine($"Client has sent the message with lenght {messageSize} to the Server! Waiting for the Server feedback.");
                //Console.WriteLine($"The message {message.Text} has been sent successfuly!");
                ServerFeedbackToClient(messageSize);
            }

        }

        public static void ServerFeedbackToClient(int msgSize)
        {
            UdpClient udpClient = new UdpClient(1234);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Console.WriteLine("Клиент ждет обратную связь от сервера.");
            while (true)
            {
                byte[] buffer = udpClient.Receive(ref iPEndPoint);
                if (buffer == null) break;
                var messageText = Encoding.UTF8.GetString(buffer);
                int messageSize = int.Parse(JsonSerializer.Deserialize<String>(messageText));
                if (msgSize == messageSize)
                {
                    Console.WriteLine("The message has been recieved by Server successfully!");
                    return;
                }
            }
        }
    }
}