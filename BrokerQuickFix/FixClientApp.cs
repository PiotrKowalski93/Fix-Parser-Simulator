using QuickFix;

namespace BrokerQuickFix
{
    public class FixClientApp : MessageCracker, IApplication
    {
        public void OnCreate(SessionID sessionID)
        {
            Console.WriteLine($"[OnCreate] Session created: {sessionID}");
        }

        public void OnLogon(SessionID sessionID)
        {
            Console.WriteLine($"[OnLogon] Successfully logged on: {sessionID}");
        }

        public void OnLogout(SessionID sessionID)
        {
            Console.WriteLine($"[OnLogout] Logged out: {sessionID}");
        }

        public void ToAdmin(Message msg, SessionID sessionID)
        {
            Console.WriteLine($"[ToAdmin] {msg}");
        }

        public void FromAdmin(Message msg, SessionID sessionID)
        {
            Console.WriteLine($"[FromAdmin] {msg}");
        }

        public void ToApp(Message msg, SessionID sessionID)
        {
            Console.WriteLine($"[ToApp] {msg}");
        }

        public void FromApp(Message msg, SessionID sessionID)
        {
            Console.WriteLine($"[FromApp] {msg}");
            Crack(msg, sessionID);
        }
    }
}
