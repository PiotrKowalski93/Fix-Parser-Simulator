using Utils;

namespace Broker
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
        private string _senderCompID;
        private string _targetCompID;

        public BrokerFixMessageGenerator(string senderCompID = "BROKER", string targetCompID = "EXCHANGE")
        {
            _senderCompID = senderCompID;
            _targetCompID = targetCompID;
        }

        #region Session Layer
        public string GenerateLogonMsg()
        {
            var body = new List<string>()
            {
                "35=A",                                         // Logon
                $"49={_senderCompID}",
                $"56={_targetCompID}",
                $"34={msgSeqNum++}",
                $"52={DateTime.UtcNow:yyyyMMdd-HH:mm:ss.fff}",
                "98=0",                                         // Encryption (none)
                "108=30"                                        // Heartbeat in seconds
            };
            return MessageUtils.PrepareFinalMsg(body);
        }

        public string GenerateLogoffMsg()
        {
            var body = new List<string>()
            {
                "35=5",                                         // Logout
                $"49={_senderCompID}",
                $"56={_targetCompID}",
                $"34={msgSeqNum++}",
                $"52={DateTime.UtcNow:yyyyMMdd-HH:mm:ss.fff}"
            };
            return MessageUtils.PrepareFinalMsg(body);
        }

        public string GenerateTestRequest(string testRequestId)
        {
            var body = new List<string>()
            {
                "35=1",                                         // TestRequest
                $"49={_senderCompID}",
                $"56={_targetCompID}",
                $"34={msgSeqNum++}",
                $"52={DateTime.UtcNow:yyyyMMdd-HH:mm:ss.fff}",
                $"112={testRequestId}"                          // TestReqID
            };

            return MessageUtils.PrepareFinalMsg(body);
        }

        public string GenerateHeartbeat()
        {
            var body = new List<string>()
            {
                "35=0",                                         // Heartbeat
                $"49={_senderCompID}",
                $"56={_targetCompID}",
                $"34={msgSeqNum++}",
                $"52={DateTime.UtcNow:yyyyMMdd-HH:mm:ss.fff}"
            };

            return MessageUtils.PrepareFinalMsg(body);
        }
        #endregion

        #region Application Layer
        public string GenerateNewOrderSingle(
            string clOrdId, 
            string symbol, 
            string side, 
            int qty, 
            float price, 
            string execId, 
            string exDestination)
        {
            var body = new List<string>()
            {
                "35=D",                                         // NewOrderSingle
                $"49={_senderCompID}",
                $"56={_targetCompID}",
                $"34={msgSeqNum++}",
                $"52={DateTime.UtcNow:yyyyMMdd-HH:mm:ss.fff}",
                $"11={clOrdId}",                                // Client Order ID
                $"55={symbol}",                                 // Symbol
                $"54={side}",                                   // Side
                $"38={qty}",                                    // Quantity
                $"40=2",                                        // OrdType = Limit
                $"44={price}",                                  // Price
                "59=0",                                         // TimeInForce = Day
                $"100={exDestination}",                                // ExDestination or custom client tag
                $"60={DateTime.UtcNow:yyyyMMdd-HH:mm:ss}"       // TransactTime                          
            };

            return MessageUtils.PrepareFinalMsg(body);
        }

        public string GenerateOrderCancelRequest(
            string clOrdId,
            string orderNumber,
            string symbol,
            string side)
        {
            var body = new List<string>()
            {
                "35=F",                                         // OrderCancelRequest
                $"49={_senderCompID}",
                $"56={_targetCompID}",
                $"34={msgSeqNum++}",
                $"52={DateTime.UtcNow:yyyyMMdd-HH:mm:ss.fff}",
                $"11={clOrdId}",                                // OrigClOrdID
                $"41={orderNumber}",                            // Target Order to cancel
                $"54={side}",                                   // Side=Buy
                $"55={symbol}",                                 // Symbol
                $"60={DateTime.UtcNow:yyyyMMdd-HH:mm:ss}"
            };

            return MessageUtils.PrepareFinalMsg(body);
        }

        public string GenerateMarketDataSnapshot(
            string msgRequestId,
            string symbol)
        {
            var body = new List<string>()
            {
                "35=V",                                         // MarketDataRequest
                $"49={_senderCompID}",
                $"56={_targetCompID}",
                $"34={msgSeqNum++}",
                $"52={DateTime.UtcNow:yyyyMMdd-HH:mm:ss.fff}",
                $"262={msgRequestId}",                          // MDReqID
                "263=1",                                        // SubscriptionRequestType = 1 (snapshot + updates)
                "264=1",                                        // MarketDepth = 1
                "146=1",                                        // NoRelatedSym
                $"55={symbol}"                                  // Symbol
            };

            return MessageUtils.PrepareFinalMsg(body);
        }

        public string GenerateResendRequest(int beginSeqNo, int endSeqNo)
        {
            var body = new List<string>()
            {
                "35=2",                                         // Resend Request
                $"49={_senderCompID}",
                $"56={_targetCompID}",
                $"34={msgSeqNum++}",
                $"52={DateTime.UtcNow:yyyyMMdd-HH:mm:ss.fff}",
                $"7={beginSeqNo}",                              // BeginSeqNo
                $"16={endSeqNo}"                                // EndSeqNo
            };

            return MessageUtils.PrepareFinalMsg(body);
        }

        public string GenerateOrderReplaceRequest(
            string originalClOrdId,
            string newClOrdId,
            string symbol,
            string side,
            string newQty,
            string newPrice)
        {
            var body = new List<string>()
            {
                "35=G",                                         // OrderCancelReplaceRequest
                $"49={_senderCompID}",
                $"56={_targetCompID}",
                $"34={msgSeqNum++}",
                $"52={DateTime.UtcNow:yyyyMMdd-HH:mm:ss.fff}",
                $"41={originalClOrdId}",                        // OrigClOrdID (previous order)
                $"11={newClOrdId}",                             // New ClOrdID (new replacement)
                $"55={symbol}",                                 // Symbol
                $"54={side}",                                   // Side=Buy
                $"38={newQty}",                                 // New quantity
                $"44={newPrice}",                               // New price
                $"60={DateTime.UtcNow:yyyyMMdd-HH:mm:ss}"       // EndSeqNo
            };

            return MessageUtils.PrepareFinalMsg(body);
        }
        #endregion
    }
}
