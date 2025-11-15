namespace ExchangeQuickFix
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var exchange = new FixExchange("Configuration/exchange.cfg");
            exchange.Start();

            Console.WriteLine("FIX Exchange running. Press ENTER to exit.");
            Console.ReadLine();

            exchange.Stop();
        }
    }
}
