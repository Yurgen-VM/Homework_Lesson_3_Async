namespace Task_1
{
    internal class Program
    {
        

        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                await Server.AcceptMess();
            }
            else
            {
                await Client.SendtMess($"Yura");
            }

            await Console.Out.WriteLineAsync("Нажмите Enter для выхода");
            Console.ReadLine();

        }
    }
}
