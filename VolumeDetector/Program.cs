// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text.RegularExpressions;
using VolumeDetector.Candlestick;
using VolumeDetector.ExchageInfo;
using VolumeDetector.Signals.PriceSignal;
using VolumeDetector.Signals.Volume;
using VolumeDetector.TickerPrice;

int counter = 1;
Console.WriteLine("Program Başladı!");

var exchangeInfoClient = new ExchangeInfoClient();
var exchangeInfo = await exchangeInfoClient.Get();

var usdtSymbols = exchangeInfo?.Symbols?.Where(x =>
{
    Regex regex = new Regex("USDT$");
    if (string.IsNullOrEmpty(x.Name))
        return false;

    return regex.IsMatch(x.Name) && x.IsActive;
}).ToList();

while (true)
{
    Console.WriteLine("Döngü Başladı!");

    var timer = new Stopwatch();
    timer.Start();

    var tasks = new List<Task>();
    if (usdtSymbols?.Any() == true)
    {
        foreach (var symbol in usdtSymbols)
        {
            tasks.Add(CheckSignal(symbol?.Name));
            //await CheckSignal(symbol?.Name);
        }

        await Task.WhenAll(tasks);
    }

    Console.WriteLine($"Çalışma sayısı: {counter}");
    counter++;

    Console.WriteLine("Döngü Bitti!");
    timer.Stop();
    var time = timer.Elapsed.TotalSeconds;
    Console.WriteLine($"Döngüde Toplam Süre: {time}sn");
    Console.WriteLine("Tekrar başlamak için beklenecek süre: 10 sn");
    Thread.Sleep(10000); // 10sn beklet
}

static async Task CheckSignal(string? symbol)
{
    try
    {
        if (string.IsNullOrEmpty(symbol)) return;
        await Console.Out.WriteLineAsync($"{symbol} için kontrol başladı!");
        var timer = new Stopwatch();
        timer.Start();

        string interval = "15m";
        decimal multiplier = 2.5M;
        var candlestickClient = new CandlestickClient(symbol, interval, 60);
        var candlesticks = await candlestickClient.Get();

        var volumeSignalResult = new VolumeSignal(candlesticks, multiplier).GetSignal();

        var currentPrice = await GetCurrentPrice(symbol);
        var now = DateTime.Now;
        var priceSignalResult = new PriceSignal(candlesticks, multiplier, currentPrice).GetSignal();

        if (volumeSignalResult.VolumeDirection == VolumeDirectionType.Positive
            &&
            volumeSignalResult.Status == VolumeStatus.HighVolume
            //&&
            //priceSignalResult.Signal == SignalType.Buy
            )
        {
            var symbolText = $"Sembol: {symbol} Tarih: {candlesticks.Last().OpenTimeDate} Anlık Tarih: {now}";
            var volumeText = $"Hacim: {volumeSignalResult.CurrentVolume} ; Hacim Ortalama: {volumeSignalResult.AverageVolume} ; Hacim Limit: {volumeSignalResult.LimitVolume} ; Hacim Artışı: %{volumeSignalResult.VolumeIncreasePercent}";
            var priceText = $"Fiyat: {currentPrice} ; Fiyat Ortalama: {priceSignalResult.AveragePrice} ; Fiyat Limit Alt: {priceSignalResult.LimitBuyPrice} ; Fiyat Limit Üst: {priceSignalResult.LimitSellPrice}";

            var fullText = symbolText + "\n" + "\t" + volumeText + "\n" + "\t" + priceText + "\n";

            Console.WriteLine(fullText);

            string filePath = "C:\\Repos\\VolumeDetector\\output.txt";
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(fullText);
            }
        }

        timer.Stop();
        var time = timer.Elapsed.TotalSeconds;
        await Console.Out.WriteLineAsync($"{symbol} için süre: {time}sn");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Bir hata oluştu! Sembol: {symbol}");
        Console.WriteLine(ex.Message);
    }
}

static async Task<decimal> GetCurrentPrice(string symbol)
{
    var tickerPriceClient = new TickerPriceClient(symbol);
    var tickerPrice = await tickerPriceClient.Get();

    return tickerPrice?.Price ?? throw new Exception($"Anlık Fiyat Bilgisi çekilemedi! Sembol: {symbol}");
}