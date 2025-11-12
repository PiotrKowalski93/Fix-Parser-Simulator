using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Broker
{
    public class FixSession
    {
        private readonly NetworkStream _stream;
        private readonly CancellationTokenSource _cts;

        //public Action<string>? OnRawMessage;
        public Action<string, Dictionary<string, string>>? OnMessageReceived;

        public FixSession(NetworkStream stream)
        {
            _stream = stream;
            _cts = new CancellationTokenSource();
        }

        public async Task StartListeningAsync()
        {
            var buffer = new byte[4096];
            var receivedData = new StringBuilder();

            while (!_cts.Token.IsCancellationRequested)
            {
                if (!_stream.DataAvailable)
                {
                    await Task.Delay(10);
                    continue;
                }

                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, _cts.Token);
                if (bytesRead == 0)
                {
                    Console.WriteLine("Connection closed.");
                    break;
                }

                string chunk = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                receivedData.Append(chunk);

                // Możliwe, że mamy kilka wiadomości FIX w jednym streamie
                string[] messages = receivedData.ToString().Split("8=FIX.4.4");
                for (int i = 1; i < messages.Length; i++)
                {
                    string msg = "8=FIX.4.4" + messages[i];
                    msg = msg.Replace('\x01', '|'); // dla czytelności

                    //OnRawMessage?.Invoke(msg);

                    var fields = ParseFixMessage(msg);
                    if (fields.TryGetValue("35", out var msgType))
                    {
                        OnMessageReceived?.Invoke(msgType, fields);
                    }
                }

                receivedData.Clear(); // wyczyść bufor po przetworzeniu
            }
        }

        private Dictionary<string, string> ParseFixMessage(string msg)
        {
            var fields = new Dictionary<string, string>();
            var parts = msg.Split('|');
            foreach (var p in parts)
            {
                var kv = p.Split('=');
                if (kv.Length == 2)
                    fields[kv[0]] = kv[1];
            }
            return fields;
        }

        public void StopListening() => _cts.Cancel();
    }

}
