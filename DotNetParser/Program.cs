namespace Broker
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("[Broker] Press any key to start broker...");
            Console.ReadKey();

            var broker = new BrokerClient();
            await broker.StartAsync();

            Console.ReadKey();
        }
    }
}
