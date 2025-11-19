using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace BrokerQuickFix
{
    public class OrderService
    {
        private readonly SessionID _sessionId;

        public OrderService(SessionID sessionId)
        {
            _sessionId = sessionId;
        }

        public void SendNewOrderSingle(string symbol, decimal qty, decimal price)
        {
            var msg = new NewOrderSingle(
                new ClOrdID(Guid.NewGuid().ToString()),
                new Symbol(symbol),
                new Side(Side.BUY),
                new TransactTime(DateTime.UtcNow),
                new OrdType(OrdType.LIMIT)
            );

            msg.SetField(new Symbol(symbol));
            msg.SetField(new OrderQty(qty));
            msg.SetField(new Price(price));
            msg.SetField(new TimeInForce(TimeInForce.DAY));

            Session.SendToTarget(msg, _sessionId);
            Console.WriteLine("[OrderService] Sent NewOrderSingle");
        }

        public void SendResend(int beginSeqNo, int endSeqNo)
        {
            var msg = new ResendRequest(new BeginSeqNo(beginSeqNo), new EndSeqNo(endSeqNo));
            Session.SendToTarget(msg, _sessionId);

            Console.WriteLine($"[FixClient] Resend message Session: {_sessionId} | BeginSeqNum: {beginSeqNo} | EndSeqNo: {endSeqNo}");
        }
    }
}
