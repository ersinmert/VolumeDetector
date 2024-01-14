// See https://aka.ms/new-console-template for more information


using Newtonsoft.Json;
using System.Text.RegularExpressions;
using VolumeDetector;

string exchangeInfoUrl = $"https://api.binance.com/api/v3/exchangeInfo?&permissions=SPOT";
List<string> pairs = new List<string>();

HttpClient client = new HttpClient();
HttpResponseMessage exchangeInfoResponse = await client.GetAsync(exchangeInfoUrl);
exchangeInfoResponse.EnsureSuccessStatusCode();
string exchangeInfoResponseBody = await exchangeInfoResponse.Content.ReadAsStringAsync();

var exchangeInfos = JsonConvert.DeserializeObject<ExchangeInfo>(exchangeInfoResponseBody);

var usdtSymbols = exchangeInfos?.Symbols?.Where(x =>
{
    Regex regex = new Regex("USDT$");
    if (string.IsNullOrEmpty(x.Name))
        return false;

    return regex.IsMatch(x.Name);
}).ToList();

if (usdtSymbols?.Any() == true)
{
    foreach (var symbol in usdtSymbols)
    {
        await CheckVolumeSignal(symbol?.Name, pairs);
    }
}

//foreach (var pair in pairs)
//{
//    Console.WriteLine(pair);
//}

Console.WriteLine("Herhangi bir tuşa basınız...");
Console.Read();

static async Task CheckVolumeSignal(string? symbol, List<string> pairs)
{
    if (string.IsNullOrEmpty(symbol)) return;

    string interval = "1m";
    string candlestickUrl = $"https://api.binance.com/api/v3/klines?symbol={symbol}&interval={interval}";

    HttpClient client = new HttpClient();
    HttpResponseMessage candleStickResponse = await client.GetAsync(candlestickUrl);
    candleStickResponse.EnsureSuccessStatusCode();
    string candlestickResponseBody = await candleStickResponse.Content.ReadAsStringAsync();

    var candlesticks = JsonConvert.DeserializeObject<List<List<string>>>(candlestickResponseBody);

    if (candlesticks?.Any() == true)
    {
        List<Candlestick> list = new List<Candlestick>();

        foreach (var candleStick in candlesticks)
        {
            long.TryParse(candleStick[0], out long openTime);
            decimal.TryParse(candleStick[1].Replace('.', ','), out decimal openPrice);
            decimal.TryParse(candleStick[2].Replace('.', ','), out decimal highPrice);
            decimal.TryParse(candleStick[3].Replace('.', ','), out decimal lowPrice);
            decimal.TryParse(candleStick[4].Replace('.', ','), out decimal closePrice);
            decimal.TryParse(candleStick[5].Replace('.', ','), out decimal volume);
            long.TryParse(candleStick[6], out long closeTime);
            decimal.TryParse(candleStick[7].Replace('.', ','), out decimal quoteAssetVolume);
            int.TryParse(candleStick[8], out int numberOfTrades);
            decimal.TryParse(candleStick[9].Replace('.', ','), out decimal takerBuyBaseAssetVolume);
            decimal.TryParse(candleStick[10].Replace('.', ','), out decimal takerBuyQuoteAssetVolume);

            Candlestick candle = new Candlestick
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
                Ignore = candleStick[11]
            };

            list.Add(candle);
        }

        var avaregeVolume = list.Average(x => x.Volume);
        var sumDeviationSquare = list.Sum(x =>
        {
            return (avaregeVolume - x.Volume) * (avaregeVolume - x.Volume);
        });
        var variance = sumDeviationSquare / list.Count();
        double.TryParse(variance.ToString(), out double varianceD);
        var standartDeviation = Math.Sqrt(varianceD);

        var overVolumeCandles = list.Where(x => x.Volume > avaregeVolume + Convert.ToDecimal(standartDeviation)).ToList();

        var dates = overVolumeCandles.Select(x =>
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(x.OpenTime);
            DateTime dateTime = dateTimeOffset.LocalDateTime;
            return dateTime;
        }).ToList();

        var dateNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);

        if (dates.Contains(dateNow))
        {
            pairs.Add(symbol);

            var yon = list.Last().OpenPrice > list.Last().ClosePrice ? "Negatif" : "Pozitif";
            Console.WriteLine($"Sembol: {symbol} Tarih: {DateTimeOffset.FromUnixTimeMilliseconds(list.Last().OpenTime).LocalDateTime} Yön: {yon} " +
                $"Hacim: {list.Last().Volume} ; Standart Sapma Hacim: {avaregeVolume + Convert.ToDecimal(standartDeviation)}");
        }
    }
}