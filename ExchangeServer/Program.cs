using System.Threading.Tasks;

namespace Exchange
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var exchange = new ExchangeServer();

            CancellationToken cancellationToken = new CancellationToken();

            await exchange.StartAsync(cancellationToken);

            Console.ReadKey();
        }

        private static void PrintMsgs()
        {
            Console.WriteLine("NEW");
            Console.WriteLine(ExchangeFixMessageGenerator.BuildExecutionReportNew("ORD123", "EX001", "AAPL", "1", 100, 180.5m, "1").Replace('\x01', '|'));
            Console.WriteLine();

            Console.WriteLine("PARTIAL FILL");
            Console.WriteLine(ExchangeFixMessageGenerator.BuildExecutionReportPartialFill("ORD123", "EX002", "AAPL", "1", 100, 40, 180.55m, "1").Replace('\x01', '|'));
            Console.WriteLine();

            Console.WriteLine("FULL FILL");
            Console.WriteLine(ExchangeFixMessageGenerator.BuildExecutionReportFill("ORD123", "EX003", "AAPL", "1", 100, 180.60m, "1").Replace('\x01', '|'));
            Console.WriteLine();

            Console.WriteLine("CANCEL");
            Console.WriteLine(ExchangeFixMessageGenerator.BuildExecutionReportCancel("ORD124", "EX004", "ORD123", "AAPL", "1", 100, "1").Replace('\x01', '|'));
            Console.WriteLine();

            Console.WriteLine("REJECT");
            Console.WriteLine(ExchangeFixMessageGenerator.BuildExecutionReportReject("ORD125", "EX005", "AAPL", "1", 100, "Invalid Price", "1").Replace('\x01', '|'));
        }
    }
}
