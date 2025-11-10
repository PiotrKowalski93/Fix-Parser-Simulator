using DotNetParser;
using DotNetParser.Broker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace FixSimulator.Broker
{
    public class BrokerClient
    {
        private readonly string _host;
        private readonly int _port;
        private readonly BrokerFixMessageGenerator _messageGenerator;

        public BrokerClient(string host = "127.0.0.1", int port = 5001)
        {
            _host = host;
            _port = port;
            _messageGenerator = new BrokerFixMessageGenerator();
        }

        public async Task StartAsync()
        {
            using var client = new TcpClient();
            await client.ConnectAsync(_host, _port);
            Console.WriteLine($"[Broker] Connected to Exchange at {_host}:{_port}");

            var stream = client.GetStream();

            // 1️) Logon
            string logon = _messageGenerator.GenerateLogonMsg();
            await SendAsync(stream, logon);

            // 2️) New Order
            string newOrder = _messageGenerator.GenerateNewOrderSingle(
                "ORD123", 
                "AAPL", 
                "1", 
                100, 
                180.50f,
                "ExecId",
                "ExDestinationSample");  // ????
            await SendAsync(stream, newOrder);

            // 3️) Cancel Order
            string cancel = _messageGenerator.GenerateOrderCancelRequest(
                "ORD124", 
                "ORD123", 
                "AAPL",
                "1");
            await SendAsync(stream, cancel);

            // 4️) Listen for responses
            await ListenAsync(stream);
        }

        private async Task SendAsync(NetworkStream stream, string msg)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            await stream.WriteAsync(bytes, 0, bytes.Length);
            Console.WriteLine($"[Broker] Sent: {msg}");
        }

        private async Task ListenAsync(NetworkStream stream)
        {
            var buffer = new byte[4096];
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"[Broker] Received: {msg}");
            }
        }
    }
}
