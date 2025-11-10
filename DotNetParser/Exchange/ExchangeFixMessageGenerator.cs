namespace DotNetParser.Exchange
{
    public static class ExchangeFixMessageGenerator
    {
        public static string BuildExecutionReportNew(
            string clOrdId,
            string execId,
            string symbol,
            string side,
            decimal orderQty,
            decimal price,
            string seqNumber,
            string senderCompID = "EXCHANGE",
            string targetCompID = "BROKER")
        {
            string sendingTime = Utils.GetFormatedDate();
            var bodyFields = new List<string>()
            {
                "35=8",                         // MsgType = ExecutionReport
                $"49={senderCompID}",           // SenderCompID
                $"56={targetCompID}",           // TargetCompID
                $"34={seqNumber}",              // MsgSeqNum
                $"52={sendingTime}",            // Sending Time
                $"150=0",                       // ExecType = 0 (New)
                $"39=0",                        // OrdStatus = 0 (New)
                $"11={clOrdId}",                // ClOrdID
                $"17={execId}",                 // ExecID
                "20=0",                         // ExecTransType = New
                $"55={symbol}",                 // Symbol
                $"54={side}",                   // Side
                $"38={orderQty}",               // OrderQty
                $"40=2",                        // OrdType = Limit
                $"44={price}",                  // Price
                "59=0",                         // TimeInForce = Day
                "32=0",                         // LastShares = 0
                "31=0",                         // LastPx = 0
                "14=0",                         // CumQty = 0
                "6=0",                          // AvgPx = 0
                $"151={orderQty}",              // LeavesQty = OrderQty
                $"60={sendingTime}",            // TransactTime - sending time for now
            };

            return Utils.PrepareFinalMsg(bodyFields);
        }

        public static string BuildExecutionReportFill(
            string clOrdId,
            string execId,
            string symbol,
            string side,
            decimal orderQty,
            decimal price,
            string seqNumber,
            string senderCompID = "EXCHANGE",
            string targetCompID = "BROKER")
        {
            string sendingTime = Utils.GetFormatedDate();
            var bodyFields = new List<string>()
            {
                "35=8",
                $"49={senderCompID}",
                $"56={targetCompID}",
                $"34={seqNumber}",
                $"52={sendingTime}",
                "150=2",                   // ExecType = Fill
                "39=2",                    // OrdStatus = Filled
                $"11={clOrdId}",
                $"17={execId}",
                "20=0",
                $"55={symbol}",
                $"54={side}",
                $"38={orderQty}",
                $"32={orderQty}",          // LastShares = full fill
                $"31={price}",            // LastPx
                $"14={orderQty}",          // CumQty = fully filled
                $"6={price}",             // AvgPx
                "151=0",                   // LeavesQty = 0
                $"60={sendingTime}",
            };

            return Utils.PrepareFinalMsg(bodyFields);
        }

        public static string BuildExecutionReportPartialFill(
            string clOrdId,
            string execId,
            string symbol,
            string side,
            decimal orderQty,
            decimal filledQty,
            decimal fillPrice,
            string seqNumber,
            string senderCompID = "EXCHANGE",
            string targetCompID = "BROKER")
        {
            string sendingTime = Utils.GetFormatedDate();

            decimal cumQty = filledQty;           // w prostym przypadku zakładamy 1. częściowe wykonanie
            decimal leavesQty = orderQty - cumQty;
            decimal avgPx = fillPrice;            // w realnym systemie można by liczyć średnią z kilku filli

            var bodyFields = new List<string>()
            {
                "35=8",
                $"49={senderCompID}",
                $"56={targetCompID}",
                $"34={seqNumber}",
                $"52={sendingTime}",
                "150=1",                        // ExecType = Partial Fill
                "39=1",                         // OrdStatus = Partially Filled
                $"11={clOrdId}",
                $"17={execId}",
                "20=0",
                $"55={symbol}",
                $"54={side}",
                $"38={orderQty}",
                $"32={filledQty}",
                $"31={fillPrice}",
                $"14={cumQty}",
                $"6={avgPx}",
                $"151={leavesQty}",
                $"60={sendingTime}"
            };
            return Utils.PrepareFinalMsg(bodyFields);
        }

        public static string BuildExecutionReportCancel(
            string clOrdId,
            string execId,
            string origClOrdId,
            string symbol,
            string side,
            decimal orderQty, 
            string seqNumber,
            string senderCompID = "EXCHANGE",
            string targetCompID = "BROKER")
        {
            string sendingTime = Utils.GetFormatedDate();
            var bodyFields = new List<string>()
            {
                "35=8",
                $"49={senderCompID}",
                $"56={targetCompID}",
                $"34={seqNumber}",
                $"52={sendingTime}",
                "150=4",                    // ExecType = Canceled
                "39=4",                     // OrdStatus = Canceled
                $"11={clOrdId}",            // nowe ID żądania anulowania
                $"41={origClOrdId}",        // oryginalne ID zlecenia
                $"17={execId}",
                "20=0",
                $"55={symbol}",
                $"54={side}",
                $"38={orderQty}",
                "14=0",
                "151=0",
                $"60={sendingTime}"
            };
            return Utils.PrepareFinalMsg(bodyFields);
        }

        public static string BuildExecutionReportReject(
            string clOrdId,
            string execId,
            string symbol,
            string side,
            decimal orderQty,
            string reason,
            string seqNumber,
            string senderCompID = "EXCHANGE",
            string targetCompID = "BROKER")
        {
            string sendingTime = Utils.GetFormatedDate();
            var bodyFields = new List<string>()
            {
                "35=8",
                $"49={senderCompID}",
                $"56={targetCompID}",
                $"34={seqNumber}",
                $"52={sendingTime}",
                "150=8",                        // ExecType = Rejected
                "39=8",                         // OrdStatus = Rejected
                $"11={clOrdId}",
                $"17={execId}",
                "20=0",
                $"55={symbol}",
                $"54={side}",
                $"38={orderQty}",
                $"58={reason}",                 // Text = powód odrzucenia
                $"60={sendingTime}"
            };
            return Utils.PrepareFinalMsg(bodyFields);
        }
    }
}
