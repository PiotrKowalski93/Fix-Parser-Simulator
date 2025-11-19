using QuickFix;
using QuickFix.Fields;

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

        public void SimulateResend()
        {
            if (_app.SessionID == null)
            {
                Console.WriteLine("No session established yet.");
                return;
            }

            int gapFrom = 3;
            int gapTo = 5;

            Message rr = new Message();
            rr.Header.SetField(new MsgType("2"));
            rr.SetField(new BeginSeqNo(gapFrom));
            rr.SetField(new EndSeqNo(gapTo));

            Session.SendToTarget(rr, _app.SessionID);

            Console.WriteLine("[FixExchange] Resend message sent to session " + _app.SessionID);
        }
    }
}
