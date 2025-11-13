using System.Net.Sockets;
using System.Text;

namespace Broker
{
    public class BrokerClient
    {
        private readonly string _host;
        private readonly int _port;
        //private readonly FixMessageCreator _messageGenerator;

        private Thread tcpListenerThread;

        // Session
        bool _loggedIn = false;

        public BrokerClient(string host = "127.0.0.1", int port = 9000)
        {
            _host = host;
            _port = port;
            //_messageGenerator = new FixMessageCreator();
        }

        // TODO: Implement session handling, wait for respones
        public async Task StartAsync()
        {
            //using var client = new TcpClient();
            //await client.ConnectAsync(_host, _port);
            //Console.WriteLine($"[Broker] Connected to Exchange at {_host}:{_port}");

            //var stream = client.GetStream();
            var _fixSession = new FixSession();
            await _fixSession.OpenSessionAsync();

            //_fixSession.OnMessageReceived += (msgType, fields) =>
            //{
            //    switch (msgType)
            //    {
            //        case "A": Console.WriteLine("Logon Ack received"); break;
            //        case "0": Console.WriteLine("Heartbeat"); break;
            //        case "8": Console.WriteLine("Execution Report"); break;
            //        case "5": Console.WriteLine("Logout"); break;
            //        default: Console.WriteLine($"Unknown message {msgType}"); break;
            //    }
            //};

            // Listener thread
            //tcpListenerThread = new Thread(async () => await _fixSession.StartListeningAsync());
            //tcpListenerThread.IsBackground = true;
            //tcpListenerThread.Start();

            try
            {
                await Task.Delay(1000);

                // 2️) New Order
                //string newOrder = FixMessageCreator.GenerateNewOrderSingle(
                //    "ORD123",
                //    "AAPL",
                //    "1",
                //    100,
                //    180.50f,
                //    "ExecId",
                //    "ExDestinationSample");
                //await SendAsync(stream, newOrder);

                await Task.Delay(1000);

                // 3️) Cancel Order
                //string cancel = FixMessageCreator.GenerateOrderCancelRequest(
                //    "ORD124",
                //    "ORD123",
                //    "AAPL",
                //    "1");
                //await SendAsync(stream, cancel);

                await Task.Delay(1000);

                // 4) Send Logout
                //string logout = FixMessageCreator.GenerateLogoffMsg();
                //await SendAsync(stream, logout);
            }
            catch (Exception)
            {
                _fixSession.StopListening();
                throw;
            }
        }
    }
}
