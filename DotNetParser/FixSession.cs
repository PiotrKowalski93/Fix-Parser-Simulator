using System.Net.Sockets;
using System.Text;

namespace Broker
{
    public class FixSession
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private readonly CancellationTokenSource _cts;

        // Session 
        public bool Connected;
        public DateTime _lastReceived;

        private int _heartbeatIntervalSec;
        private int _seqNumber;
        private string _senderCompId;
        private string _targetCompId;

        // Events for incoming messages
        //public event EventHandler<string>? MessageReceived;

        // Internal session handling
        private event EventHandler? HeartbeatSent;
        private event EventHandler? TestRequestSent;
        private event EventHandler? HeartbeatTimeout;
        private event EventHandler? Disconnected;
        private event EventHandler? LogonReceived;
        private event EventHandler? LogoutReceived;

        public FixSession(int heartbeatIntervalSec = 10,
            string senderCompId = "BROKER", 
            string targetCompId = "EXCHANGE")
        {
            _heartbeatIntervalSec = heartbeatIntervalSec;
            _cts = new CancellationTokenSource();
            _senderCompId = senderCompId;
            _targetCompId = targetCompId;
        }

        public async Task OpenSessionAsync(string host = "127.0.0.1", int port = 9000)
        {
            try
            {
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(host, port);
                Console.WriteLine($"[Broker] Connected to Exchange at {host}:{port}");
                _stream = _tcpClient.GetStream();

                _seqNumber = 1;
                // Send Logon
                Console.WriteLine($"[Broker] Send Logon");
                await SendAsync(FixMessageCreator.GenerateLogonMsg(_senderCompId, _targetCompId, _seqNumber++));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Broker] Exception during establishing connectopmn to Exchange at {host}:{port}");
                Console.WriteLine($"{ex.Message}");
                return;
            }

            //Run Listener
            Task.Run(() => StartListeningAsync());

            //Run heartbeat monitor
            Task.Run(() => HeartbeatMonitor());
        }

        private async Task StartListeningAsync()
        {
            var buffer = new byte[4096];
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, _cts.Token);
                    if (bytesRead == 0)
                    {
                        Disconnected?.Invoke(this, EventArgs.Empty);
                        break;
                    }

                    var message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    _lastReceived = DateTime.UtcNow;

                    MessageReceived(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Session] Error: {ex.Message}");
                Disconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        private void MessageReceived(string message)
        {
            Console.WriteLine($"[Exchange] {message}");

            // TODO: Add mapping Field => Event
            //if (message.Contains("35=A")) LogonReceived?.Invoke(this, EventArgs.Empty);
            //if (message.Contains("35=5")) LogoutReceived?.Invoke(this, EventArgs.Empty);
        }

        // This loop keeps Session alive
        private async Task HeartbeatMonitor()
        {
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    await Task.Delay(_heartbeatIntervalSec * 1000, _cts.Token);

                    var diff = DateTime.UtcNow - _lastReceived;

                    if (diff.TotalSeconds > _heartbeatIntervalSec * 1.5)
                    {
                        HeartbeatTimeout?.Invoke(this, EventArgs.Empty);

                        Console.WriteLine($"[Broker] Sent TestRequest");
                        await SendAsync(FixMessageCreator.GenerateTestRequest(_senderCompId, _targetCompId, _seqNumber, "TestReqId"));

                        TestRequestSent?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        Console.WriteLine($"[Broker] Sent Heartbeat");
                        await SendAsync(FixMessageCreator.GenerateHeartbeat(_senderCompId, _targetCompId, _seqNumber));

                        HeartbeatSent?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // TODO
            }
        }

        public async Task SendAsync(string fixMessage)
        {
            //var msg = fixMessage.Replace("|", "\x01");
            byte[] bytes = Encoding.UTF8.GetBytes(fixMessage);
            await _stream.WriteAsync(bytes, 0, bytes.Length);
        }

        public void StopListening()
        {
            _cts.Cancel();
            _tcpClient.Dispose();
        } 
    }
}
