using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Task_1
{
    internal class Client
    {
      
        public static async Task SendtMess(string nikName)
        {
            IPEndPoint clientEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
            UdpClient udpClient = new UdpClient();

            while (true)
            {
                Console.WriteLine("\nВведите сообщение:");
                string text = Console.ReadLine();

                if (text != "Exit")
                {
                    try
                    {
                        Message newMessage = new Message(nikName, text);
                        string js = newMessage.ToJson();
                        byte[] bytes = Encoding.UTF8.GetBytes(js);
                        await udpClient.SendAsync(bytes, clientEP);

                        byte[] buffer = udpClient.Receive(ref clientEP); // Создаем буфер принимающий события от сервера                        
                        string str = Encoding.UTF8.GetString(buffer); // Переводим массив байтов в строку 
                        Message? msg = Message.FromJson(str); // Преобразуем строку из JSON в объект Message
                        Console.WriteLine($"\n{msg.ToString()}"); // Вывод в консоль сообщения от сервера
                    }
                    catch (Exception ex) { Console.WriteLine("Сервер не доступен");}
                    
                }
                else
                {
                    Console.WriteLine("\nРабота программы завершена");
                    return;
                }
            }


        }
    }
}
