namespace BrokerQuickFix
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var client = new FixClient("Configuration/client.cfg");
            client.Start();

            if (client.SessionId == null)
            {
                Console.WriteLine("No FIX session found.");
                return;
            }

            var orderService = new OrderService(client.SessionId);

            // Poczekaj na logon
            Console.WriteLine("Press ENTER to send a test order...");
            Console.ReadLine();
            

            
            orderService.SendNewOrderSingle("AAPL", 10, 150.25m);

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();

            client.Stop();
        }
    }
}
