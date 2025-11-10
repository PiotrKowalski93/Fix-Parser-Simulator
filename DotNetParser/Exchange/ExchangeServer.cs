using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DotNetParser.Exchange
{
    public class ExchangeServer
    {
        private bool _running;
        private int _port;
        private TcpListener? _listener;
        private Random _randomizer;

        public ExchangeServer(int port = 98781)
        {
            _port = port;
            _randomizer = new Random();
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _listener = new TcpListener(IPAddress.Loopback, _port);
            _listener.Start();
            _running = true;

            Console.WriteLine($"FIX Exchange Simulator started on port: {_port}");

            while (cancellationToken.IsCancellationRequested) 
            {
                var client = await _listener.AcceptTcpClientAsync();
                await HandleClientAsync(client, cancellationToken);
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
        {
            Console.WriteLine("Client connected.");

            using var stream = client.GetStream();
            var buffer = new byte[4096];

            while (_running && !cancellationToken.IsCancellationRequested)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                if (bytesRead <= 0) break;

                string fixMsg = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                fixMsg = fixMsg.Replace('\x01', '|'); // Only for console logging!

                var sections = fixMsg.Split('|');
                var tags = PrepareTags(sections);

                Console.WriteLine($"Received: {fixMsg}");

                // Recognize message type
                string response = await BuildExchangeResponse(tags["35"]);

                if (!string.IsNullOrEmpty(response))
                {
                    await SendAsync(stream, response);
                }
            }
        }

        private async Task<string> BuildExchangeResponse(string msgType)
        {
            // Small delay simulation
            await Task.Delay(500);

            if (msgType == "A") // Logon
                return "8=FIX.4.4|9=65|35=A|34=1|49=EXCHANGE|56=BROKER|108=30|10=128|";

            if (msgType == "D") // New Order
                return "8=FIX.4.4|9=120|35=8|49=EXCHANGE|56=BROKER|150=0|39=0|55=AAPL|54=1|38=100|44=180.50|37=EX123|17=1|10=220|";

            if (msgType == "F") // Cancel
                return "8=FIX.4.4|9=110|35=8|49=EXCHANGE|56=BROKER|150=4|39=4|41=ORD123|37=EX123|11=ORD123|17=2|10=155|";

            return null;
        }

        private async Task RespondeWithPartialFill(Dictionary<string, string> tags, NetworkStream stream)
        {
            // Small delay simulation
            await Task.Delay(500);

            string clOrdId = tags["11"];
            string symbol = tags["55"];
            string side = tags["54"];
            decimal qty = decimal.Parse(tags["38"]);
            decimal price = decimal.Parse(tags["44"]);

            // 1) NEW
            string ack = ExchangeFixMessageGenerator.BuildExecutionReportNew(
                clOrdId,
                _randomizer.Next(1, 9999).ToString(), // Must be unique in a trading day
                symbol,
                side,
                qty,
                price,
                _randomizer.Next(1, 9999).ToString()); // In is unique for each message in session, random for now

            await SendAsync(stream, ack);

            // 2️) Partial Fill
            await Task.Delay(1000);
            string partial = ExchangeFixMessageGenerator.BuildExecutionReportPartialFill(
                clOrdId,
                _randomizer.Next(1, 9999).ToString(),
                symbol,
                side,
                qty,
                qty / 2,
                price,
                _randomizer.Next(1, 9999).ToString());
            await SendAsync(stream, partial);

            // 3️) Full Fill
            await Task.Delay(1000);
            string fill = ExchangeFixMessageGenerator.BuildExecutionReportFill(
                clOrdId,
                _randomizer.Next(1, 9999).ToString(),
                symbol,
                side,
                qty,
                price,
                _randomizer.Next(1, 9999).ToString());
            await SendAsync(stream, fill);
        }

        public async Task SendAsync(NetworkStream stream, string msg)
        {
            string rawFix = msg.Replace('|', '\x01');
            byte[] data = Encoding.ASCII.GetBytes(rawFix);
            await stream.WriteAsync(data, 0, data.Length);
            Console.WriteLine($"Sent: {msg}");
        }

        public Dictionary<string, string> PrepareTags(string[] sections)
        {
            var dict = new Dictionary<string, string>();

            foreach (var section in sections)
            {
                var tag = section.Split("=");
                dict.Add(tag[0], tag[1]);
            }
            return dict;
        }
    }
}
