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

            Console.WriteLine($"[ToAdmin] SeqNum: {seq} | Type: {MessageUtils.ToReadable(type)}");

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

            Console.WriteLine($"[FromAdmin] SeqNum: {seq} | Type: {MessageUtils.ToReadable(type)}");

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
            var type = MessageUtils.ToReadable(msg.Header.GetString(Tags.MsgType));

            Console.WriteLine($"[Exchange -> Client] SeqNum: {seq} | Type: {type}+");
        }

        public void FromApp(Message msg, SessionID sessionID)
        {
            var seq = msg.Header.GetInt(Tags.MsgSeqNum);
            var type = MessageUtils.ToReadable(msg.Header.GetString(Tags.MsgType));

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
        }

        public void OnMessage(QuickFix.FIX44.OrderCancelRequest cancelRequest, SessionID sessionID)
        {
            //TODO: Implement Response
        }
        public void OnMessage(QuickFix.FIX44.OrderCancelReplaceRequest replaceRequest, SessionID sessionID)
        {
            //TODO: Implement Response
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
