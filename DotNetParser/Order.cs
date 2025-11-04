namespace DotNetParser
{
    public class Order
    {
        public string ClOrdID { get; set; }
        public string Symbol { get; set; }
        public Sides Side { get; set; } // 1 = Buy, 2 = Sell
        public decimal OrderQty { get; set; }
        public decimal Price { get; set; }
        public decimal CumQty { get; set; }
        public decimal LeavesQty => OrderQty - CumQty;
        public string Status { get; set; } // New, Filled, etc.
    }
}
