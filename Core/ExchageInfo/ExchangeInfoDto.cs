using Newtonsoft.Json;

namespace Core.ExchageInfo
{
    public class ExchangeInfoDto
    {
        [JsonProperty("timezone")]
        public string? TimeZone { get; set; }

        [JsonProperty("serverTime")]
        public long ServerTime { get; set; }

        [JsonProperty("rateLimits")]
        public List<RateLimitDto>? RateLimits { get; set; }

        [JsonProperty("exchangeFilters")]
        public List<string>? ExchangeFilters { get; set; }

        [JsonProperty("symbols")]
        public List<SymbolDto>? Symbols { get; set; }
    }

    public class RateLimitDto
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

    public class SymbolDto
    {
        [JsonProperty("symbol")]
        public string? Name { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }

        public bool IsActive
        {
            get
            {
                return Status == "TRADING";
            }
        }
    }
}
