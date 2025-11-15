using QuickFix;
using QuickFix.Fields;

namespace ExchangeQuickFix
{
    public class ExchangeApp : MessageCracker, IApplication
    {
        public void OnCreate(SessionID sessionID)
        {
            Console.WriteLine($"[Exchange] Session created: {sessionID}");
        }

        public void OnLogon(SessionID sessionID)
        {
            Console.WriteLine($"[Exchange] LOGON: {sessionID}");
        }

        public void OnLogout(SessionID sessionID)
        {
            Console.WriteLine($"[Exchange] LOGOUT: {sessionID}");
        }

        // -------------- Admin Msg
        // Logon (35=A)
        // Logout(35=5)
        // Messages i.e:
        // Heartbeat(35=0)
        // TestRequest(35=1)
        // ResendRequest(35=2)
        // SequenceReset(35=4)
        // Reject(35=3)
        // BusinessMessageReject(35=j)

        // Broker is sending those
        // 1) We can add user and password i.e
        // 2) Add custom tags

        // We can manipulate msg before sending to exchange and right afer reciving, before logic
        public void ToAdmin(Message msg, SessionID sessionID)
        {
            Console.WriteLine($"[Exchange -> Admin] {msg}");

            // Prod Scenario?
            //var msgType = message.Header.GetString(Tags.MsgType);

            //if (msgType == MsgType.LOGON)
            //{
            //    message.SetField(new Username("myUser"));
            //    message.SetField(new Password("myPass"));
            //    message.SetField(new ResetSeqNumFlag(true));
            //}
        }

        // Coming from Exchange
        // 1) Forced logout with a reason “Invalid Credentials”, “Too many logons”, “Session not allowed”, “SeqNum too low” (-> resend request)
        // 2) Session monitoring for Metrics and Alerts
        public void FromAdmin(Message msg, SessionID sessionID)
        {
            Console.WriteLine($"[Admin -> Exchange] {msg}");

            var msgType = msg.Header.GetString(Tags.MsgType);

            //if (msgType == MsgType.LOGOUT)
            //{
            //    Console.WriteLine("Logout received: " +
            //        msg.GetString(Tags.Text));
            //}

            //if (msgType == MsgType.REJECT || msgType == MsgType.BUSINESS_MESSAGE_REJECT)
            //{
            //    Console.WriteLine("Reject: " + msg.ToString());
            //}
        }
        // --------------------------------------------


        public void ToApp(Message msg, SessionID sessionID)
        {
            Console.WriteLine($"[Exchange -> Client] {msg}");
        }

        public void FromApp(Message msg, SessionID sessionID)
        {
            Console.WriteLine($"[Client -> Exchange] {msg}");
            Crack(msg, sessionID);
        }

        // ---------------------------------------
        //       MESSAGE HANDLERS
        // ---------------------------------------
        public void OnMessage(QuickFix.FIX44.NewOrderSingle order, SessionID sessionID)
        {
            Console.WriteLine("[Exchange] Received NewOrderSingle");

            var orderId = Guid.NewGuid().ToString();
            var clOrdId = order.GetField(Tags.ClOrdID);

            // STEP 1: Order Acknowledgement (NEW)
            var ack = new QuickFix.FIX44.ExecutionReport(
                new OrderID(orderId),
                new ExecID(Guid.NewGuid().ToString()),
                new ExecType(ExecType.NEW),
                new OrdStatus(OrdStatus.NEW),
                new Symbol(order.Symbol.getValue()),
                new Side(order.Side.getValue()),
                new LeavesQty(order.OrderQty.getValue()),
                new CumQty(0),
                new AvgPx(0)
            );

            ack.SetField(new ClOrdID(clOrdId));
            ack.SetField(new Symbol(order.Symbol.getValue()));
            ack.SetField(new AvgPx(0));

            Session.SendToTarget(ack, sessionID);

            // STEP 2: Fill the order (FILLED)
            var fill = new QuickFix.FIX44.ExecutionReport(
                new OrderID(orderId),
                new ExecID(Guid.NewGuid().ToString()),
                new ExecType(ExecType.FILL),
                new OrdStatus(OrdStatus.FILLED),
                new Symbol(order.Symbol.getValue()),
                new Side(order.Side.getValue()),
                new LeavesQty(0),
                new CumQty(order.OrderQty.getValue()),
                new AvgPx(0)
            );

            fill.SetField(new ClOrdID(clOrdId));
            fill.SetField(new Symbol(order.Symbol.getValue()));
            fill.SetField(new LastQty(order.OrderQty.getValue()));
            fill.SetField(new LastPx(order.Price.getValue()));
            fill.SetField(new AvgPx(order.Price.getValue()));

            Session.SendToTarget(fill, sessionID);
        }
    }

}
