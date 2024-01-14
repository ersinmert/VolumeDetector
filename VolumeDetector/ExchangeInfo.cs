using Newtonsoft.Json;
using System.Text.Json;

namespace VolumeDetector
{
    public class ExchangeInfo
    {
        [JsonProperty("timezone")]
        public string? TimeZone { get; set; }

        [JsonProperty("serverTime")]
        public long ServerTime { get; set; }

        [JsonProperty("rateLimits")]
        public List<RateLimit>? RateLimits { get; set; }

        [JsonProperty("exchangeFilters")]
        public List<string>? ExchangeFilters { get; set; }

        [JsonProperty("symbols")]
        public List<Symbol>? Symbols { get; set; }
    }

    public class RateLimit
    {
        [JsonProperty("rateLimitType")]
        public string? RateLimitType { get; set; }

        [JsonProperty("interval")]
        public string? Interval { get; set; }

        [JsonProperty("intervalNum")]
        public int IntervalNum { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }
    }

    public class Symbol
    {
        [JsonProperty("symbol")]
        public string? Name { get; set; }
    }
}
