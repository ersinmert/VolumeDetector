namespace VolumeDetector.Signals.Volume
{
    public class VolumeSignalResult
    {
        public decimal CurrentVolume { get; set; }
        public decimal LimitVolume { get; set; }
        public decimal AverageVolume { get; set; }
        public VolumeDirectionType VolumeDirection { get; set; }
        public VolumeStatus Status { get; set; }
    }
}
