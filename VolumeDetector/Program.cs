// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;
using VolumeDetector.Candlestick;
using VolumeDetector.ExchageInfo;
using VolumeDetector.Signals.PriceSignal;
using VolumeDetector.Signals.Volume;
using VolumeDetector.TickerPrice;

var exchangeInfoClient = new ExchangeInfoClient();
var exchangeInfo = await exchangeInfoClient.Get();

var usdtSymbols = exchangeInfo?.Symbols?.Where(x =>
{
    Regex regex = new Regex("USDT$");
    if (string.IsNullOrEmpty(x.Name))
        return false;

    return regex.IsMatch(x.Name) && x.IsActive;
}).ToList();

if (usdtSymbols?.Any() == true)
{
    foreach (var symbol in usdtSymbols)
    {
        await CheckSignal(symbol?.Name);
    }
}

Console.WriteLine("Herhangi bir tuşa basınız...");
Console.ReadKey();

static async Task CheckSignal(string? symbol)
{
    try
    {
        if (string.IsNullOrEmpty(symbol)) return;

        string interval = "15m";
        int multiplier = 3;
        var candlestickClient = new CandlestickClient(symbol, interval);
        var candlesticks = await candlestickClient.Get();

        var volumeSignalResult = new VolumeSignal(candlesticks, multiplier).GetSignal();

        var currentPrice = await GetCurrentPrice(symbol);
        var priceSignalResult = new PriceSignal(candlesticks, multiplier, currentPrice).GetSignal();

        if (volumeSignalResult.VolumeDirection == VolumeDirectionType.Positive
            &&
            volumeSignalResult.Status == VolumeStatus.HighVolume
            //&&
            //priceSignalResult.Signal == SignalType.Buy
            )
        {
            Console.WriteLine($"Sembol: {symbol} Tarih: {candlesticks.Last().OpenTimeDate} /n" +
                $"/t Hacim: {volumeSignalResult.CurrentVolume} ; Hacim Ortalama: {volumeSignalResult.AverageVolume} ; Hacim Limit: {volumeSignalResult.LimitVolume}" +
                $"/t Fiyat: {currentPrice} ; Fiyat Ortalama: {priceSignalResult.AveragePrice} ; Fiyat Limit Alt: {priceSignalResult.LimitBuyPrice} ; Fiyat Limit Üst: {priceSignalResult.LimitSellPrice} /n");
        }
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