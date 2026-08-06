using System.Globalization;

namespace filecabinet
{
    public static class ConsoleReader
    {
        public static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter)
        {
            while (true)
            {
                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                return conversionResult.Item3;
            }
        }

        public static Func<string, Tuple<bool, string, string>> StringConverter = input =>
        {
            if (!string.IsNullOrWhiteSpace(input))
                return Tuple.Create(true, string.Empty, input.Trim());
            return Tuple.Create(false, "Invalid string format", string.Empty);
        };

        public static Func<string, Tuple<bool, string, decimal>> DecimalConverter = input =>
        {
            if (decimal.TryParse(input,CultureInfo.InvariantCulture, out var result))
                return Tuple.Create(true, string.Empty, result);
            return Tuple.Create(false, "Invalid decimal format", 0m);
        };

        public static Func<string, Tuple<bool, string, DateTime>> DateTimeConverter = input =>
        {
            if (DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                return Tuple.Create(true, string.Empty, result);
            return Tuple.Create(false, "Invalid date format", default(DateTime));
        };

        public static Func<string, Tuple<bool, string, short>> ShortConverter = input =>
        {
            if (short.TryParse(input, out short result))
                return Tuple.Create(true, string.Empty, result);
            return Tuple.Create(false, "Invalid short format", default(short));
        };

        public static Func<string, Tuple<bool, string, char>> CharConverter = input =>
        {
            if (char.TryParse(input, out char result))
                return Tuple.Create(true, string.Empty, result);
            return Tuple.Create(false, "Invalid char format", default(char));
        };
    }
}