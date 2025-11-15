using QuickFix;

namespace ExchangeQuickFix
{
    public class FixExchange
    {
        private readonly ThreadedSocketAcceptor _acceptor;
        private readonly ExchangeApp _app;

        public FixExchange(string configPath)
        {
            _app = new ExchangeApp();

            var settings = new SessionSettings(configPath);
            var storeFactory = new FileStoreFactory(settings);
            var logFactory = new FileLogFactory(settings);

            _acceptor = new ThreadedSocketAcceptor(_app, storeFactory, settings, logFactory);
        }

        public void Start()
        {
            Console.WriteLine("[FixExchange] Starting FIX server...");
            _acceptor.Start();
        }

        public void Stop()
        {
            Console.WriteLine("[FixExchange] Stopping...");
            _acceptor.Stop();
        }
    }
}
