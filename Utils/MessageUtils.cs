using System.Runtime.InteropServices.JavaScript;

namespace Utils
{
    public static class MessageUtils
    {
        private static readonly Dictionary<string, string> msgTypeMap = new()
        {
            { "0",  "Heartbeat" },
            { "1",  "TestRequest" },
            { "2",  "ResendRequest" },
            { "3",  "Reject" },
            { "4",  "SequenceReset" },
            { "5",  "Logout" },
            { "A",  "Logon" },

            // Orders
            { "D",  "NewOrderSingle" },
            { "F",  "OrderCancelRequest" },
            { "G",  "OrderCancelReplaceRequest" },

            // Execution Reports
            { "8",  "ExecutionReport" },
            { "9",  "OrderCancelReject" },

            // Market Data
            { "V",  "MarketDataRequest" },
            { "W",  "MarketDataSnapshotFullRefresh" },
            { "X",  "MarketDataIncrementalRefresh" },

            // Session-Level
            { "j",  "BusinessMessageReject" }
        };

        private static readonly Dictionary<string, string> OrdStatusMap = new()
        {
            { "0", "New" },
            { "1", "Partially Filled" },
            { "2", "Filled" },
            { "3", "Done for Day" },
            { "4", "Canceled" },
            { "5", "Replaced" },
            { "6", "Pending Cancel" },
            { "7", "Stopped" },
            { "8", "Rejected" },
            { "9", "Suspended" },
            { "A", "Pending New" },
            { "B", "Calculated" },
            { "C", "Expired" },
            { "D", "Accepted for Bidding" },
            { "E", "Pending Replace" }
        };

        //TODO: Add tests
        public static int CalculateChecksum(string msg)
        {
            int sum = msg.Sum(c => (byte)c);
            return sum % 256;
        }

        //TODO: Add tests
        public static string GetFormatedDate()
        {
            return DateTime.UtcNow.ToString("yyyyMMdd-HH:mm:ss.fff");
        }

        //TODO: Add tests
        public static string PrepareFinalMsg(List<string> bodyFields)
        {
            string body = string.Join('|', bodyFields) + '|';
            string header = $"8=FIX.4.4|9={body.Length}|";
            string fullMessage = header + body;

            int chcekSum = CalculateChecksum(fullMessage);

            return fullMessage + $"10={chcekSum:D3}|";
        }

        public static string MsgType(string msgType)
        {
            if (msgTypeMap.TryGetValue(msgType, out string? pretty))
                return pretty;

            return $"Unknown({msgType})";
        }

        public static string OrdStatus(string ordStatus)
        {
            if (OrdStatusMap.TryGetValue(ordStatus, out string? pretty))
                return pretty;

            return $"Unknown ({ordStatus})";
        }
    }
}
