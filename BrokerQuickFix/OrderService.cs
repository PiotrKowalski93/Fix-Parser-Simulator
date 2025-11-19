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
            var reqId = Guid.NewGuid().ToString();

            var newOrderRequest = new NewOrderSingle(
                new ClOrdID(reqId),
                new Symbol(symbol),
                new Side(Side.BUY),
                new TransactTime(DateTime.UtcNow),
                new OrdType(OrdType.LIMIT)
            );

            newOrderRequest.SetField(new Symbol(symbol));
            newOrderRequest.SetField(new OrderQty(qty));
            newOrderRequest.SetField(new Price(price));
            newOrderRequest.SetField(new TimeInForce(TimeInForce.DAY));

            Session.SendToTarget(newOrderRequest, _sessionId);
            Console.WriteLine($"[FixClient] Sent NewOrderSingle {reqId}");
        }

        public void SendResend(int beginSeqNo, int endSeqNo)
        {
            var msg = new ResendRequest(new BeginSeqNo(beginSeqNo), new EndSeqNo(endSeqNo));
            Session.SendToTarget(msg, _sessionId);
            Console.WriteLine($"[FixClient] Resend message Session: {_sessionId} | BeginSeqNum: {beginSeqNo} | EndSeqNo: {endSeqNo}");
        }

        public void SendCancel(string origClOrdID, 
            string symbol, 
            char side, 
            decimal qty)
        {
            var reqId = Guid.NewGuid().ToString();
            var cancelRequest = new OrderCancelRequest(
                new OrigClOrdID(origClOrdID),
                new ClOrdID(reqId),
                new Symbol(symbol),
                new Side(side),
                new TransactTime(DateTime.UtcNow));

            cancelRequest.Set(new OrderQty(qty));
            Session.SendToTarget(cancelRequest, _sessionId);
            Console.WriteLine($"[FixClient] Sent OrderCancelRequest {reqId}");
        }

        public void SendReplace(string origClOrdID,
            string symbol, 
            char side, 
            decimal newQty,
            decimal? newPrice = null)
        {
            var reqId = Guid.NewGuid().ToString();
            var replaceRequest = new OrderCancelReplaceRequest(
                new OrigClOrdID(origClOrdID),
                new ClOrdID(reqId),
                new Symbol(symbol),
                new Side(side),
                new TransactTime(DateTime.UtcNow),
                new OrdType(OrdType.LIMIT));
            replaceRequest.Set(new OrderQty(newQty));

            if (newPrice.HasValue)
            {
                replaceRequest.Set(new Price(newPrice.Value));
            }

            Session.SendToTarget(replaceRequest, _sessionId);
            Console.WriteLine($"[FixClient] Sent OrderCancelReplaceRequest {reqId}");
        }
    }
}
