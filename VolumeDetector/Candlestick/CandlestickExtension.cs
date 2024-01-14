namespace VolumeDetector.Candlestick
{
    public static class CandlestickExtension
    {
        public static CandlestickDto? Map(this List<string>? candlestick)
        {
            if (candlestick == null)
                return null;

            long.TryParse(candlestick[0], out long openTime);
            decimal.TryParse(candlestick[1].Replace('.', ','), out decimal openPrice);
            decimal.TryParse(candlestick[2].Replace('.', ','), out decimal highPrice);
            decimal.TryParse(candlestick[3].Replace('.', ','), out decimal lowPrice);
            decimal.TryParse(candlestick[4].Replace('.', ','), out decimal closePrice);
            decimal.TryParse(candlestick[5].Replace('.', ','), out decimal volume);
            long.TryParse(candlestick[6], out long closeTime);
            decimal.TryParse(candlestick[7].Replace('.', ','), out decimal quoteAssetVolume);
            int.TryParse(candlestick[8], out int numberOfTrades);
            decimal.TryParse(candlestick[9].Replace('.', ','), out decimal takerBuyBaseAssetVolume);
            decimal.TryParse(candlestick[10].Replace('.', ','), out decimal takerBuyQuoteAssetVolume);

            return new CandlestickDto
            {
                OpenTime = openTime,
                OpenPrice = openPrice,
                HighPrice = highPrice,
                LowPrice = lowPrice,
                ClosePrice = closePrice,
                Volume = volume,
                CloseTime = closeTime,
                QuoteAssetVolume = quoteAssetVolume,
                NumberOfTrades = numberOfTrades,
                TakerBuyBaseAssetVolume = takerBuyBaseAssetVolume,
                TakerBuyQuoteAssetVolume = takerBuyQuoteAssetVolume,
                Ignore = candlestick[11]
            };
        }
    }
}
