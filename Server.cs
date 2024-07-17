using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Task_1
{
    internal class Server
    {
        static private CancellationTokenSource cts = new CancellationTokenSource();
        static private CancellationToken ct = cts.Token;

        private static bool IsRunning = true;

        public static async Task AcceptMess()
        {
            UdpClient udpClient = new UdpClient(12345);
            IPEndPoint serverEp = new IPEndPoint(IPAddress.Any, 0);

            Console.WriteLine("Сервер ожидает сообщение от клиента.");
            Console.WriteLine("Для завершения работы нажмите \"Esc\"");
            Task programmClose = Task.Run(() =>
            {
                try
                {
                    while (!ct.IsCancellationRequested)
                    {
                        var key = Console.ReadKey(intercept: true);
                        if (key.Key == ConsoleKey.Escape)
                        {
                            cts.Cancel();                            
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Работа сервера остановлена ");
                }
               
            });


            while (!ct.IsCancellationRequested)
            {
                try
                {
                    if (udpClient.Available > 0)
                    {
                        var data = udpClient.Receive(ref serverEp);
                        string dataB = Encoding.UTF8.GetString(data);

                        await Task.Run(async () =>
                        {
                            Message? msg = Message.FromJson(dataB);
                            if (msg != null)
                            {
                                Console.WriteLine($"{msg.ToString()}\n"); // Вывод в консоль сообщения от клиента

                                Message servResp = new Message("Server", "Сообщение получено"); // Формируем ответ клиенту, о доставке сообщения
                                string strServResp = servResp.ToJson(); // Конвертируем наше сообщение в JSON
                                byte[] byteServResp = Encoding.UTF8.GetBytes(strServResp); // Кодируем JSON в массив байтов
                                await udpClient.SendAsync(byteServResp, serverEp); // Отправляем пакет клиенту

                            }
                            else { Console.WriteLine("Некорректное сообщение"); }
                        });
                    }
                    else
                    {
                        await Task.Delay(100);
                    }
                    ct.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Работа сервера остановлена");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка {e.Message}");
                }
            }
        }
    }
}
