using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetParser
{
    public class BrokerFixMessageGenerator
    {
        private static int msgSeqNum = 1;

        /// <summary>
        /// Session Layer
        /// </summary>
        /// <returns></returns>
        public static string GenerateLogonMsg()
        {
            return null;
        }

        public static string GenerateLogoffMsg()
        {
            return null;
        }

        public static string GenerateTestRequest()
        {
            return null;
        }

        public static string GenerateOrderCancelRequest()
        {
            return null;
        }

        public static string GenerateMarketDataSnapshot()
        {
            return null;
        }

        /// <summary>
        /// Application Layer 
        /// </summary>
        /// <param name="clOrdId"></param>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <param name="qty"></param>
        /// <param name="price"></param>
        /// <param name="orderId"></param>
        /// <param name="execId"></param>
        /// <returns></returns>
        public static string GenerateNewOrderSingle(string clOrdId, string symbol, Sides side, int qty, float price, string orderId, string execId, string client)
        {
            string sendingTime = DateTime.UtcNow.ToString("yyyyMMdd-HH:mm:ss.fff");
            var fields = new List<string>()
            {
                "35=D",                 // NewOrderSingle
                $"34={msgSeqNum++}",
                $"49={client}",         // SenderCompID
                $"52={sendingTime}",
                $"37={orderId}",
                $"17={execId}",
                $"11={clOrdId}",
                "150=0",                // ExecType = New
                "39=0",                 // OrdStatus = New
                $"55={symbol}",
                $"54={side}",           // Side: 1-Buy, 2-Sell
                $"38={qty}",            // Quotation
                $"44={price}",
                $"60={sendingTime}"
            };

            string body = string.Join('\x01', fields) + '\x01';
            string header = $"8=FIX.4.2\x019={body.Length}\x01";

            string fullMessage = header + body;

            int chcekSum = Utils.CalculateChecksum(fullMessage);
            return fullMessage + $"10={chcekSum:D3}\x01";
        }
    }
}
