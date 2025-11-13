using QuickFix;
using QuickFix.Fields;

namespace BrokerQuickFix
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            SessionSettings settings = new SessionSettings(args[0]);

            IApplication myApp = new MyQuickFixApp();

            IMessageStoreFactory storeFactory = new FileStoreFactory(settings);

            ILogFactory logFactory = new FileLogFactory(settings);

            ThreadedSocketAcceptor acceptor = new ThreadedSocketAcceptor(
                myApp,
                storeFactory,
                settings,
                logFactory);

            //acceptor.Start();
            //while (true)
            //{
            //    System.Console.WriteLine("o hai");
            //    System.Threading.Thread.Sleep(1000);
            //}
            //acceptor.Stop();

            // Creating new order
            var order = new QuickFix.FIX44.NewOrderSingle(
                new ClOrdID("1234"),
                new Symbol("AAPL"),
                new Side(Side.BUY),
                new TransactTime(DateTime.Now),
                new OrdType(OrdType.MARKET));

            order.Price = new Price(new decimal(22.4));
            order.Account = new Account("18861112");

            Session.SendToTarget(order, sessionID);
        }
    }

    public class MyQuickFixApp : IApplication
    {
        public void FromApp(Message msg, SessionID sessionID) { }
        public void OnCreate(SessionID sessionID) { }
        public void OnLogout(SessionID sessionID) { }
        public void OnLogon(SessionID sessionID) { }
        public void FromAdmin(Message msg, SessionID sessionID) { }
        public void ToAdmin(Message msg, SessionID sessionID) { }
        public void ToApp(Message msg, SessionID sessionID) { }
    }


}
