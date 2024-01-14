using Newtonsoft.Json;

namespace VolumeDetector.TickerPrice
{
    public class TickerPriceClient
    {
        private string _tickerPriceUrl = "https://api.binance.com/api/v3/ticker/price?symbol={0}";

        public TickerPriceClient(string symbol)
        {
            _tickerPriceUrl = string.Format(_tickerPriceUrl, symbol);
        }

        public async Task<TickerPriceDto?> Get()
        {
            var tickerPrice = new TickerPriceDto();

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(_tickerPriceUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                tickerPrice = JsonConvert.DeserializeObject<TickerPriceDto>(responseBody);
            }

            return tickerPrice;
        }
    }
}
