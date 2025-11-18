using QuickFix;
using QuickFix.Transport;

namespace BrokerQuickFix
{
    public class FixClient
    {
        private readonly FixClientApp _app;
        private readonly SocketInitiator _initiator;
        public SessionID? SessionId { get; private set; }

        public FixClient(string configPath)
        {
            _app = new FixClientApp();

            var settings = new SessionSettings(configPath);
            var storeFactory = new FileStoreFactory(settings);
            var logFactory = new FileLogFactory(settings);

            _initiator = new SocketInitiator(_app, storeFactory, settings, logFactory);

            // One session
            foreach (var session in settings.GetSessions())
                SessionId = session;
        }

        public void Start()
        {
            Console.WriteLine("[FixClient] Starting...");
            _initiator.Start();
        }

        public void Stop()
        {
            Console.WriteLine("[FixClient] Stopping...");
            _initiator.Stop();
        }

        public FixClientApp App => _app;
    }
}
