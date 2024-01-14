namespace VolumeDetector.Signals.Volume
{
    public class VolumeSignal
    {
        private readonly List<Candlestick.CandlestickDto> _candlesticks;
        private readonly int _multiplier;

        public VolumeSignal(List<Candlestick.CandlestickDto> candlesticks, int multiplier)
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

            return new VolumeSignalResult
            {
                CurrentVolume = currentVolume,
                AverageVolume = volumeSdResult.Average,
                LimitVolume = limitVolume,
                VolumeDirection = volumeDirection,
                Status = status
            };
        }
    }
}
