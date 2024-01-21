using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Signals.Price
{
    public class PriceSignal
    {
        private readonly List<Candlestick.CandlestickDto> _candlesticks;
        private readonly decimal _multiplier;
        private readonly decimal _currentPrice;

        public PriceSignal(List<Candlestick.CandlestickDto> candlesticks, decimal multiplier, decimal currentPrice)
        {
            _candlesticks = candlesticks;
            _multiplier = multiplier;
            _currentPrice = currentPrice;
        }

        public PriceSignalResult GetSignal()
        {
            var prices = _candlesticks.Select(x => x.Ohlc4).ToList();
            var priceSd = new StandardDeviation.StandardDeviation(prices, 50);
            var priceSdResult = priceSd.Calculate();

            var limitBuyPrice = priceSdResult.Average - (priceSdResult.StandardDeviation * _multiplier);
            var limitSellPrice = priceSdResult.Average + (priceSdResult.StandardDeviation * _multiplier);

            SignalType signal = SignalType.None;
            if (_currentPrice < limitBuyPrice)
                signal = SignalType.Buy;
            else if (_currentPrice > limitSellPrice)
                signal = SignalType.Sell;
            else
                signal = SignalType.None;

            return new PriceSignalResult
            {
                Signal = signal,
                AveragePrice = priceSdResult.Average,
                LimitBuyPrice = limitBuyPrice,
                LimitSellPrice = limitSellPrice
            };
        }
    }
}
