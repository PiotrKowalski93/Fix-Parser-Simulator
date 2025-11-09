namespace DotNetParser.Broker
{
    public interface IBrokerMessages
    {
        // ---- Session Layer
        string GenerateLogonMsg();
        string GenerateLogoffMsg();
        string GenerateTestRequest();
        string GenerateHeartbeat();

        // ---- Application Layer
        string GenerateNewOrderSingle(string clOrdId, string symbol, Sides side, int qty, float price, string orderId, string execId, string client);
        string GenerateOrderCancelRequest();
        string GenerateMarketDataSnapshot();
        string GenerateResendRequest(int beginSeqNo, int endSeqNo);
        string GenerateOrderReplaceRequest();
    }
}
