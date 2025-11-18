namespace BrokerQuickFix
{
    internal class Program
    {
        static FixClient _client;
        static OrderService _orderService;

        static void Main(string[] args)
        {
            _client = new FixClient("Configuration/client.cfg");
            _client.Start();

            if (_client.SessionId == null)
            {
                Console.WriteLine("No FIX session found.");
                return;
            }

            _orderService = new OrderService(_client.SessionId);

            Console.WriteLine("Wait for logon...");

            bool shouldContinue;
            do
            {
                var command = Console.ReadLine();
                shouldContinue = HandleCommand(command);
            } while (shouldContinue);

            _client.Stop();
        }

        private static bool HandleCommand(string? command)
        {
            switch (command)
            {
                case "Send NewOrderSingle":
                    _orderService.SendNewOrderSingle("AAPL", 10, 150.25m);
                    return true;
                default:
                    return false;
            }
        }
    }
}
