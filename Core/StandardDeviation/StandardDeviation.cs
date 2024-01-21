namespace Core.StandardDeviation
{
    public class StandardDeviation
    {
        private readonly List<decimal> _values;

        public StandardDeviation(List<decimal> values, int length = 200)
        {
            _values = values.Count() > length ? values.TakeLast(length).ToList() : values;
        }

        public StandardDeviationResult Calculate()
        {
            var average = _values.Average();
            var sumDeviationSquare = _values.Sum(value =>
            {
                return (average - value) * (average - value);
            });

            var variance = sumDeviationSquare / _values.Count();

            double.TryParse(variance.ToString(), out double varianceD);
            var standartDeviation = Math.Sqrt(varianceD);

            decimal.TryParse(standartDeviation.ToString(), out decimal standartDeviationDecimal);

            return new StandardDeviationResult
            {
                Average = average,
                StandardDeviation = standartDeviationDecimal
            };
        }
    }
}
