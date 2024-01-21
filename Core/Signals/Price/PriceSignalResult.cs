namespace Core.Signals.Price
{
    public class PriceSignalResult
    {
        public decimal LimitBuyPrice { get; set; }
        public decimal LimitSellPrice { get; set; }
        public decimal AveragePrice { get; set; }
        public SignalType Signal { get; set; }
    }
}
