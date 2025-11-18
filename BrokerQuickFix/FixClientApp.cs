using QuickFix;
using QuickFix.Fields;

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
            var type = msg.Header.GetString(Tags.MsgType);

            Console.WriteLine($"[ToAdmin] SeqNum: {seq} | Type: {type}");
        }

        public void FromAdmin(Message msg, SessionID sessionID)
        {
            var seq = msg.Header.GetInt(Tags.MsgSeqNum);
            var type = msg.Header.GetString(Tags.MsgType);

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
            var type = msg.Header.GetString(Tags.MsgType);

            Console.WriteLine($"[ToApp] SeqNum: {seq} | Type: {type}");
        }

        public void FromApp(Message msg, SessionID sessionID)
        {
            var seq = msg.Header.GetInt(Tags.MsgSeqNum);
            var type = msg.Header.GetString(Tags.MsgType);

            Console.WriteLine($"[FromApp] SeqNum: {seq} | Type: {type}");
        }
    }
}
