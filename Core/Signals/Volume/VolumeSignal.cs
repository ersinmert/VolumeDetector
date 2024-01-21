namespace Core.Signals.Volume
{
    public class VolumeSignal
    {
        private readonly List<Candlestick.CandlestickDto> _candlesticks;
        private readonly decimal _multiplier;

        public VolumeSignal(List<Candlestick.CandlestickDto> candlesticks, decimal multiplier)
        {
            _candlesticks = candlesticks;
            _multiplier = multiplier;
        }

        public VolumeSignalResult GetSignal()
        {
            var volumes = _candlesticks.Select(x => x.Volume).ToList();
            var volumeSd = new StandardDeviation.StandardDeviation(volumes, 50);
            var volumeSdResult = volumeSd.Calculate();
            var currentCandleStick = _candlesticks.Last();
            var currentVolume = currentCandleStick.Volume;

            var limitVolume = volumeSdResult.Average + (volumeSdResult.StandardDeviation * _multiplier);

            var status = currentVolume > limitVolume ? VolumeStatus.HighVolume : VolumeStatus.LowVolume;
            var volumeDirection = currentCandleStick.OpenPrice > currentCandleStick.ClosePrice ? VolumeDirectionType.Negative : VolumeDirectionType.Positive;

            var volumePercentIncrease = ((currentVolume - volumeSdResult.Average) / volumeSdResult.Average) * 100;

            return new VolumeSignalResult
            {
                CurrentVolume = currentVolume,
                AverageVolume = volumeSdResult.Average,
                LimitVolume = limitVolume,
                VolumeIncreasePercent = volumePercentIncrease,
                VolumeDirection = volumeDirection,
                Status = status
            };
        }
    }
}
