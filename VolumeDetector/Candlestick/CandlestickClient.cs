using Newtonsoft.Json;

namespace VolumeDetector.Candlestick
{
    public class CandlestickClient
    {
        private string _candlestickUrl = "https://api.binance.com/api/v3/klines?symbol={0}&interval={1}&limit={2}";

        public CandlestickClient(string symbol, string interval, int limit = 500)
        {
            _candlestickUrl = string.Format(_candlestickUrl, symbol, interval, limit);
        }

        public async Task<List<CandlestickDto>> Get()
        {
            List<CandlestickDto> list = new List<CandlestickDto>();
            var candlesticks = new List<List<string>>();
            using (var client = new HttpClient())
            {
                HttpResponseMessage candleStickResponse = await client.GetAsync(_candlestickUrl);
                candleStickResponse.EnsureSuccessStatusCode();
                string candlestickResponseBody = await candleStickResponse.Content.ReadAsStringAsync();

                candlesticks = JsonConvert.DeserializeObject<List<List<string>>>(candlestickResponseBody);
            }
            if (candlesticks?.Any() == true)
            {
                foreach (var candleStick in candlesticks)
                {
                    var candle = candleStick.Map();

                    if (candle != null)
                        list.Add(candle);
                }
            }

            return list.OrderBy(x => x.CloseTimeDate).ToList();
        }
    }
}
