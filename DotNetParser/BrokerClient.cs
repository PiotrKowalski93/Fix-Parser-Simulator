namespace Broker
{
    public class BrokerClient
    {
        private readonly string _host;
        private readonly int _port;

        public BrokerClient(string host = "127.0.0.1", int port = 9000)
        {
            _host = host;
            _port = port;
        }

        public async Task StartAsync()
        {
            var _fixSession = new FixSession();
            await _fixSession.OpenSessionAsync();

            try
            {
                await Task.Delay(1000);
            }
            catch (Exception)
            {
                _fixSession.StopListening();
                throw;
            }
        }
    }
}
