﻿using Newtonsoft.Json;

namespace VolumeDetector
{
    public class Candlestick
    {
        [JsonProperty("0")]
        public long OpenTime { get; set; }

        [JsonProperty("1")]
        public decimal OpenPrice { get; set; }

        [JsonProperty("2")]
        public decimal HighPrice { get; set; }

        [JsonProperty("3")]
        public decimal LowPrice { get; set; }

        [JsonProperty("4")]
        public decimal ClosePrice { get; set; }

        [JsonProperty("5")]
        public decimal Volume { get; set; }

        [JsonProperty("6")]
        public long CloseTime { get; set; }

        [JsonProperty("7")]
        public decimal QuoteAssetVolume { get; set; }

        [JsonProperty("8")]
        public int NumberOfTrades { get; set; }

        [JsonProperty("9")]
        public decimal TakerBuyBaseAssetVolume { get; set; }

        [JsonProperty("10")]
        public decimal TakerBuyQuoteAssetVolume { get; set; }

        [JsonProperty("11")]
        public string? Ignore { get; set; }

        public decimal VolumePrice
        {
            get
            {
                var hlco4 = (OpenPrice + ClosePrice + HighPrice + LowPrice) / 4;
                return hlco4 * Volume;
            }
        }
    }
}
