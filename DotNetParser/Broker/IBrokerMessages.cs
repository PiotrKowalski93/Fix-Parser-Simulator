namespace DotNetParser.Broker
{
    public interface IBrokerMessages
    {
        // ---- Session Layer
        string GenerateLogonMsg();
        string GenerateLogoffMsg();
        string GenerateTestRequest(string testRequestId);
        string GenerateHeartbeat();

        // ---- Application Layer
        string GenerateNewOrderSingle(string clOrdId, string symbol, string side, int qty, float price, string execId, string client);
        string GenerateOrderCancelRequest(string clOrdId, string orderNumber, string symbol, string side);
        string GenerateMarketDataSnapshot(string msgRequestId, string symbol);
        string GenerateResendRequest(int beginSeqNo, int endSeqNo);
        string GenerateOrderReplaceRequest(string originalClOrdId, string newClOrdId, string symbol, string side, string newQty, string newPrice);
    }
}
