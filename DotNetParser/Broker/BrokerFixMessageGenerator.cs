namespace DotNetParser.Broker
{
    //| FIX tag 35 | Name                                    
    //| ---------- | ---------------------------------------
    //| `A`        | ** Logon **                             
    //| `5`        | ** Logout **
    //| `0`        | ** Heartbeat **
    //| `1`        | ** Test Request **
    //| `D`        | ** New Order - Single **
    //| `F`        | ** Order Cancel Request **
    //| `V`        | ** Market Data Request **
    //| `W`        | ** Market Data Snapshot / Full Refresh **
    //| `2`        | ** Resend Request **
    //| `G`        | ** Replace Order Request **

    public class BrokerFixMessageGenerator : IBrokerMessages
    {
        private static int msgSeqNum = 1;

        #region Session Layer
        public string GenerateLogonMsg()
        {
            throw new NotImplementedException();
        }

        public string GenerateLogoffMsg()
        {
            throw new NotImplementedException();
        }

        public string GenerateTestRequest()
        {
            throw new NotImplementedException();
        }

        public string GenerateHeartbeat()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Application Layer
        public string GenerateNewOrderSingle(string clOrdId, string symbol, Sides side, int qty, float price, string orderId, string execId, string client)
        {
            throw new NotImplementedException();
        }

        public string GenerateOrderCancelRequest()
        {
            throw new NotImplementedException();
        }

        public string GenerateMarketDataSnapshot()
        {
            throw new NotImplementedException();
        }

        public string GenerateResendRequest(int beginSeqNo, int endSeqNo)
        {
            throw new NotImplementedException();
        }

        public string GenerateOrderReplaceRequest()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
