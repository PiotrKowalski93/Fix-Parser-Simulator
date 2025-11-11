using System.Net.Sockets;
using System.Text;

namespace Broker
{
    public class BrokerClient
    {
        private readonly string _host;
        private readonly int _port;
        private readonly BrokerFixMessageGenerator _messageGenerator;

        // Session
        bool _loggedIn = false;

        public BrokerClient(string host = "127.0.0.1", int port = 9000)
        {
            _host = host;
            _port = port;
            _messageGenerator = new BrokerFixMessageGenerator();
        }

        // TODO: Implement session handling, wait for respones
        public async Task StartAsync()
        {
            using var client = new TcpClient();
            await client.ConnectAsync(_host, _port);
            Console.WriteLine($"[Broker] Connected to Exchange at {_host}:{_port}");

            var stream = client.GetStream();

            // 1️) Logon
            string logon = _messageGenerator.GenerateLogonMsg();
            await SendAsync(stream, logon);
            //await WaitForLogonAck();

            // 2️) New Order
            string newOrder = _messageGenerator.GenerateNewOrderSingle(
                "ORD123", 
                "AAPL", 
                "1", 
                100, 
                180.50f,
                "ExecId",
                "ExDestinationSample");
            await SendAsync(stream, newOrder);
            //await WaitForExecutionReportAccepted();

            // 3️) Cancel Order
            string cancel = _messageGenerator.GenerateOrderCancelRequest(
                "ORD124", 
                "ORD123", 
                "AAPL",
                "1");
            await SendAsync(stream, cancel);
            //await WaitForCancelAck();

            // 4) Send Logout
            string logout = _messageGenerator.GenerateLogoffMsg();
            await SendAsync(stream, cancel);
            //await WaitForLogoutAck();
        }

        private async Task SendAsync(NetworkStream stream, string msg)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            await stream.WriteAsync(bytes, 0, bytes.Length);
            Console.WriteLine($"[Broker] Sent: {msg}");
        }

        private async Task WaitForLogonAck(NetworkStream stream)
        {
            var buffer = new byte[4096];
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                msg = msg.Replace('\x01', '|'); // Only for console logging!

                Console.WriteLine($"[Broker] Received: {msg}");
            }
        }
    }
}
