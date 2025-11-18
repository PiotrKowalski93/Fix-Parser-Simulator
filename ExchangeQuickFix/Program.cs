namespace ExchangeQuickFix
{
    internal class Program
    {
        static FixExchange _exchange;

        static void Main(string[] args)
        {
            _exchange = new FixExchange("Configuration/exchange.cfg");
            _exchange.Start();

            Console.WriteLine("FIX Exchange running. Press ENTER to exit.");

            bool shouldContinue;

            do
            {
                var command = Console.ReadLine();
                shouldContinue = HandleCommand(command);
            } while (shouldContinue);
            
            _exchange.Stop();
        }

        private static bool HandleCommand(string? command) 
        {
            switch (command)
            {
                case "Resend":
                    _exchange.SimulateResend();
                    return true;
                default:
                    return false;
            }
        }
    }
}
