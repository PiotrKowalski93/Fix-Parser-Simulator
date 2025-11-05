namespace DotNetParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("NEW");
            Console.WriteLine(ExchangeFixMessageGenerator.BuildExecutionReportNew("ORD123", "EX001", "AAPL", Sides.Buy, 100, 180.5m, "1").Replace('\x01', '|'));
            Console.WriteLine();

            Console.WriteLine("PARTIAL FILL");
            Console.WriteLine(ExchangeFixMessageGenerator.BuildExecutionReportPartialFill("ORD123", "EX002", "AAPL", Sides.Buy, 100, 40, 180.55m, "1").Replace('\x01', '|'));
            Console.WriteLine();

            Console.WriteLine("FULL FILL");
            Console.WriteLine(ExchangeFixMessageGenerator.BuildExecutionReportFill("ORD123", "EX003", "AAPL", Sides.Buy, 100, 180.60m, "1").Replace('\x01', '|'));
            Console.WriteLine();

            Console.WriteLine("CANCEL");
            Console.WriteLine(ExchangeFixMessageGenerator.BuildExecutionReportCancel("ORD124", "EX004", "ORD123", "AAPL", Sides.Buy, 100, "1").Replace('\x01', '|'));
            Console.WriteLine();

            Console.WriteLine("REJECT");
            Console.WriteLine(ExchangeFixMessageGenerator.BuildExecutionReportReject("ORD125", "EX005", "AAPL", Sides.Buy, 100, "Invalid Price", "1").Replace('\x01', '|'));

            Console.ReadKey();
        }
    }
}
