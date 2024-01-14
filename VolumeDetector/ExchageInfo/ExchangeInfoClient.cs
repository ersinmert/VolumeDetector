using Newtonsoft.Json;

namespace VolumeDetector.ExchageInfo
{
    public class ExchangeInfoClient
    {
        private string _exchangeInfoUrl = $"https://api.binance.com/api/v3/exchangeInfo?&permissions=SPOT";

        public async Task<ExchangeInfoDto?> Get()
        {
            var exchangeInfo = new ExchangeInfoDto();
            using (var client = new HttpClient())
            {
                HttpResponseMessage exchangeInfoResponse = await client.GetAsync(_exchangeInfoUrl);
                exchangeInfoResponse.EnsureSuccessStatusCode();
                string exchangeInfoResponseBody = await exchangeInfoResponse.Content.ReadAsStringAsync();

                exchangeInfo = JsonConvert.DeserializeObject<ExchangeInfoDto>(exchangeInfoResponseBody);
            }

            return exchangeInfo;
        }
    }
}
