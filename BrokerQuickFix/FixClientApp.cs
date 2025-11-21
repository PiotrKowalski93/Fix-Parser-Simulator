using QuickFix;
using QuickFix.Fields;
using Utils;

namespace BrokerQuickFix
{
    public class FixClientApp : MessageCracker, IApplication
    {
        public SessionID? SessionID { get; private set; }
        public bool LoggedIn { get; private set; }

        public void OnCreate(SessionID sessionID)
        {
            Console.WriteLine($"[OnCreate] Session created: {sessionID}");
            SessionID = sessionID;
            LoggedIn = true;
        }

        public void OnLogon(SessionID sessionID)
        {
            Console.WriteLine($"[OnLogon] Successfully logged on: {sessionID}");
            SessionID = sessionID;
            LoggedIn = true;
        }

        public void OnLogout(SessionID sessionID)
        {
            Console.WriteLine($"[OnLogout] Logged out: {sessionID}");
            SessionID = null;
            LoggedIn = false;
        }

        public void ToAdmin(Message msg, SessionID sessionID)
        {
            var seq = msg.Header.GetInt(Tags.MsgSeqNum);
            var type = MessageUtils.MsgType(msg.Header.GetString(Tags.MsgType));

            Console.WriteLine($"[ToAdmin] SeqNum: {seq} | Type: {type}");
        }

        public void FromAdmin(Message msg, SessionID sessionID)
        {
            var seq = msg.Header.GetInt(Tags.MsgSeqNum);
            var type = MessageUtils.MsgType(msg.Header.GetString(Tags.MsgType));

            if (type == MsgType.RESEND_REQUEST)
            {
                Console.WriteLine($"[FromAdmin] RESEND_REQUEST SeqNum: {seq} | Type: {type}");
                //HandleResendRequest(msg, sessionID);
            }

            if (type == MsgType.SEQUENCE_RESET)
            {
                Console.WriteLine($"[FromAdmin] SEQUENCE_RESET SeqNum: {seq} | Type: {type}");
                //HandleSequenceReset(msg, sessionID);
            }
        }

        public void ToApp(Message msg, SessionID sessionID)
        {
            var seq = msg.Header.GetInt(Tags.MsgSeqNum);
            var type = MessageUtils.MsgType(msg.Header.GetString(Tags.MsgType));

            Console.WriteLine($"[ToApp] SeqNum: {seq} | Type: {type}");
        }

        public void FromApp(Message msg, SessionID sessionID)
        {
            Crack(msg, sessionID);
        }

        // ---------------------------------------
        //       MESSAGE HANDLERS
        // ---------------------------------------

        public void OnMessage(QuickFix.FIX44.ExecutionReport executionReport, SessionID sessionID)
        {
            var seq = executionReport.Header.GetInt(Tags.MsgSeqNum);
            var type = MessageUtils.MsgType(executionReport.Header.GetString(Tags.MsgType));
            var ordStatus = MessageUtils.OrdStatus(executionReport.GetString(Tags.OrdStatus));

            Console.WriteLine($"[FromApp] SeqNum: {seq} | Type: {type} | Order Status: {ordStatus}");
        }

        // ---------------------------------------
    }
}
