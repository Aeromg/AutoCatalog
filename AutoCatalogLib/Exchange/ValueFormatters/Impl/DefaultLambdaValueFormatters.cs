using System;
using System.Linq;
using System.Text.RegularExpressions;
using AutoCatalogLib.Business;

namespace AutoCatalogLib.Exchange.ValueFormatters.Impl
{
    public static class DefaultLambdaValueFormatters
    {
        private static readonly Regex _longSpaces = new Regex(@"\s{2,}|\t+");
        private static readonly Regex _floatPattern = new Regex(@"[-—]{0,1}\d+([,.]\d+){0,1}");
        private static readonly Regex _integerPattern = new Regex(@"[-—]{0,1}\d+");
        private static readonly Regex _arrayDelimitter = new Regex(@"\s*[;,]\s*");

        public static IFormatter<string> ToCleanString { get; private set; }
        public static IFormatter<long> ToInteger { get; private set; }
        public static IFormatter<double> ToFloat { get; private set; }
        public static IFormatter<string[]> ToCleanStringArray { get; private set; }

        private static IFormatter<string> BuildToCleanStringFormatter()
        {
            var guid = Guid.Parse(@"33e3386e-f034-4dfe-8a2c-1c532785b75d");
            var name = @"Чистка строки";
            var description = @"Удаление из строки мусорных символов и лидирующих пробелов";
            Func<object, string> f = (i) => i == null ? String.Empty : _longSpaces.Replace(i.ToString().Trim(), @" ");

            return new LambdaValueFormatter<string>(guid, name, description, f);
        }

        private static IFormatter<long> BuildToIntegerFormatter()
        {
            var guid = Guid.Parse(@"2fb4d63c-0c57-45ef-9550-b07b76807585");
            var name = @"Целое число";
            var description = @"Преобразование в целое число со знаком. Округление в ближайшее меньшее";
            Func<object, long> f = (i) =>
            {
                if (i == null)
                    return 0L;

                var firstNumberMatch = _integerPattern.Matches(i.ToString()).Cast<Match>().FirstOrDefault();
                if (firstNumberMatch == null)
                    return 0L;

                var numberObj = Convert.ChangeType(firstNumberMatch.Value, TypeCode.Int64);

                return numberObj == null ? 0 : (long) numberObj;
            };

            return new LambdaValueFormatter<long>(guid, name, description, f);
        }

        private static IFormatter<double> BuildToFloatFormatter()
        {
            var guid = Guid.Parse(@"b02a61f5-cc86-48c4-85f5-07b556fc0358");
            var name = @"Вещественное число";
            var description = @"Преобразование в вещественное число со знаком";
            Func<object, double> f = (i) =>
            {
                if (i == null)
                    return 0F;

                var firstNumberMatch = _floatPattern.Matches(i.ToString()).Cast<Match>().FirstOrDefault();
                if (firstNumberMatch == null)
                    return 0F;

                var firstNumberString = firstNumberMatch.Value.Replace('.', ',');
                var numberObj = Convert.ChangeType(firstNumberString, TypeCode.Double);

                return numberObj == null ? 0F : (double)numberObj;
            };

            return new LambdaValueFormatter<double>(guid, name, description, f);
        }

        private static IFormatter<string[]> BuildToCleanStringArrayFormatter()
        {
            var guid = Guid.Parse(@"d132464d-5585-4a56-89b8-a1fa90003359");
            var name = @"Массив строк";
            var description = @"Деление строки на массив подстрок с автоматическим определением разделителя";
            Func<object, string[]> f = (i) =>
            {
                if(i == null) 
                    return new string[0];

                var tokens = _arrayDelimitter.Split(i.ToString().Trim()).Where(s => s.Length > 0);
                return tokens.Select(t => _longSpaces.Replace(t, @" ")).ToArray();
            };

            return new LambdaValueFormatter<string[]>(guid, name, description, f);
        }

        public static void Init()
        {
            ToCleanString = BuildToCleanStringFormatter();
            ToInteger = BuildToIntegerFormatter();
            ToFloat = BuildToFloatFormatter();
            ToCleanStringArray = BuildToCleanStringArrayFormatter();

            FormattersLocator.Set(ToCleanString);
            FormattersLocator.Set(ToInteger);
            FormattersLocator.Set(ToFloat);
            FormattersLocator.Set(ToCleanStringArray);
        }
    }
}
