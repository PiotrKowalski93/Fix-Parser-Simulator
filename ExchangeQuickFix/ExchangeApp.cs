using QuickFix;
using QuickFix.Fields;
using Utils;

namespace ExchangeQuickFix
{
    public class ExchangeApp : MessageCracker, IApplication
    {
        public SessionID SessionID { get; private set; }

        public void OnCreate(SessionID sessionID)
        {
            Console.WriteLine($"[Exchange] Session created: {sessionID}");
            SessionID = sessionID;
        }

        public void OnLogon(SessionID sessionID)
        {
            Console.WriteLine($"[Exchange] LOGON: {sessionID}");
            SessionID = sessionID;
        }

        public void OnLogout(SessionID sessionID)
        {
            Console.WriteLine($"[Exchange] LOGOUT: {sessionID}");
        }

        // Broker is sending those
        // 1) We can add user and password i.e
        // 2) Add custom tags

        // We can manipulate msg before sending to exchange and right afer reciving, before logic
        public void ToAdmin(Message msg, SessionID sessionID)
        {
            var seq = msg.Header.GetInt(Tags.MsgSeqNum);
            var type = msg.Header.GetString(Tags.MsgType);

            Console.WriteLine($"[ToAdmin] SeqNum: {seq} | Type: {MessageUtils.MsgType(type)}");

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
            var seq = msg.Header.GetInt(Tags.MsgSeqNum);
            var type = msg.Header.GetString(Tags.MsgType);

            Console.WriteLine($"[FromAdmin] SeqNum: {seq} | Type: {MessageUtils.MsgType(type)}");

            //if (msgType == MsgType.LOGOUT)
            //{
            //    Console.WriteLine("Logout received: " +
            //        msg.GetString(Tags.Text));
            //}

            //if (msgType == MsgType.REJECT || msgType == MsgType.BUSINESS_MESSAGE_REJECT)
            //{
            //    Console.WriteLine("Reject: " + msg.ToString());
            //}

            if (type == MsgType.RESEND_REQUEST)
            {
                int begin = msg.GetInt(Tags.BeginSeqNo);
                int end = msg.GetInt(Tags.EndSeqNo);

                Console.WriteLine($"[EXCHANGE] ResendRequest BeginSeqNum: {begin} | EndSeqNum: {end}");
                SendGapFill(sessionID, end + 1);
            }
        }
        // --------------------------------------------

        public void ToApp(Message msg, SessionID sessionID)
        {
            var seq = msg.Header.GetInt(Tags.MsgSeqNum);
            var type = MessageUtils.MsgType(msg.Header.GetString(Tags.MsgType));

            Console.WriteLine($"[Exchange -> Client] SeqNum: {seq} | Type: {type}");
        }

        public void FromApp(Message msg, SessionID sessionID)
        {
            var seq = msg.Header.GetInt(Tags.MsgSeqNum);
            var type = MessageUtils.MsgType(msg.Header.GetString(Tags.MsgType));

            Console.WriteLine($"[Client -> Exchange] SeqNum: {seq} | Type: {type}");
            Crack(msg, sessionID);
        }

        // ---------------------------------------
        //       MESSAGE HANDLERS
        // ---------------------------------------
        public void OnMessage(QuickFix.FIX44.NewOrderSingle order, SessionID sessionID)
        {
            var orderId = Guid.NewGuid().ToString();
            var clOrdId = order.GetField(Tags.ClOrdID);

            var symbol = order.Symbol.getValue();
            var side = order.Side.getValue();
            var qty = order.OrderQty.getValue();

            // 1) PENDING NEW - Exchange got order
            var pendingNew = new QuickFix.FIX44.ExecutionReport(
                new OrderID(orderId),
                new ExecID(Guid.NewGuid().ToString()),
                new ExecType(ExecType.PENDING_NEW),
                new OrdStatus(OrdStatus.PENDING_NEW),
                new Symbol(symbol),
                new Side(side),
                new LeavesQty(qty),
                new CumQty(0),
                new AvgPx(0)
            );

            pendingNew.SetField(new ClOrdID(clOrdId));

            Session.SendToTarget(pendingNew, sessionID);

            // Simulate Exchange latency
            Thread.Sleep(2000);

            // 2) NEW — Order is Accepted and active
            var newReport = new QuickFix.FIX44.ExecutionReport(
                new OrderID(orderId),
                new ExecID(Guid.NewGuid().ToString()),
                new ExecType(ExecType.NEW),
                new OrdStatus(OrdStatus.NEW),
                new Symbol(symbol),
                new Side(side),
                new LeavesQty(qty),
                new CumQty(0),
                new AvgPx(0)
            );

            newReport.SetField(new ClOrdID(clOrdId));

            Session.SendToTarget(newReport, sessionID);

            Thread.Sleep(2000);

            // 3) FILLED — Full order is filled
            decimal filledPrice = 100.25m; // Sample price

            var filled = new QuickFix.FIX44.ExecutionReport(
                new OrderID(orderId),
                new ExecID(Guid.NewGuid().ToString()),
                new ExecType(ExecType.FILL),
                new OrdStatus(OrdStatus.FILLED),
                new Symbol(symbol),
                new Side(side),
                new LeavesQty(0),    // Nothing left
                new CumQty(qty),     // All consumed
                new AvgPx(filledPrice)
            );

            filled.SetField(new ClOrdID(clOrdId));
            filled.SetField(new LastPx(filledPrice));
            filled.SetField(new LastQty(qty)); // Last volume 0- all

            Session.SendToTarget(filled, sessionID);
        }

        public void OnMessage(QuickFix.FIX44.OrderCancelRequest cancelRequest, SessionID sessionID)
        {
            var exec = new QuickFix.FIX44.ExecutionReport(
                new OrderID(Guid.NewGuid().ToString()),   // Echange given ID
                new ExecID(Guid.NewGuid().ToString()),    // Unique ExecId
                new ExecType(ExecType.CANCELED),
                new OrdStatus(OrdStatus.CANCELED),
                cancelRequest.Symbol,
                cancelRequest.Side,
                new LeavesQty(0),
                new CumQty(0),
                new AvgPx(0)
            );

            exec.Set(cancelRequest.ClOrdID);
            exec.Set(cancelRequest.OrigClOrdID);
            exec.Set(cancelRequest.Symbol);
            exec.Set(cancelRequest.OrderQty);

            Session.SendToTarget(exec, sessionID);
        }
        public void OnMessage(QuickFix.FIX44.OrderCancelReplaceRequest replaceRequest, SessionID sessionID)
        {
            var exec = new QuickFix.FIX44.ExecutionReport(
                new OrderID(Guid.NewGuid().ToString()),
                new ExecID(Guid.NewGuid().ToString()),
                new ExecType(ExecType.REPLACE),
                new OrdStatus(OrdStatus.REPLACED),
                replaceRequest.Symbol,
                replaceRequest.Side,
                new LeavesQty(replaceRequest.OrderQty.Obj),
                new CumQty(0),
                new AvgPx(0)
            );

            exec.Set(replaceRequest.ClOrdID);      // New ID
            exec.Set(replaceRequest.OrigClOrdID);  // Previous ID
            exec.Set(replaceRequest.Symbol);
            exec.Set(replaceRequest.OrderQty);
            exec.Set(replaceRequest.Price);

            Session.SendToTarget(exec, sessionID);
        }

        // ---------------------------------------


        private void SendGapFill(SessionID sessionID, int newSeq)
        {
            var seqReset = new QuickFix.FIX44.SequenceReset();

            seqReset.Set(new GapFillFlag(true));  // 123=Y
            seqReset.Set(new NewSeqNo(newSeq));   // next seqnum

            Console.WriteLine($"[EXCHANGE] Sending GapFill, NewSeqNo={newSeq}");

            Session.SendToTarget(seqReset, sessionID);
        }
    }

}
