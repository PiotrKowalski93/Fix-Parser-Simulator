namespace DotNetParser
{
    public static class Utils
    {
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

        public static string PrepareFinalMsg(List<string> bodyFields)
        {
            string body = string.Join('|', bodyFields) + '|';
            string header = $"8=FIX.4.4|9={body.Length}";
            string fullMessage = header + body;

            int chcekSum = CalculateChecksum(fullMessage);

            return fullMessage + $"10={chcekSum:D3}|";
        }
    }
}
