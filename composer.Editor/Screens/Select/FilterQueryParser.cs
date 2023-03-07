using System.Globalization;
using System.Text.RegularExpressions;
using osu.Game.Screens.Select;
using osu.Game.Screens.Select.Filter;

namespace composer.Editor.Screens.Select
{
    public static class FilterQueryParser
    {
        private static readonly Regex query_syntax_regex = new Regex(
            @"\b(?<key>\w+)(?<op>(:|=|(>|<)(:|=)?))(?<value>("".*"")|(\S*))",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        internal static void ApplyQueries(FilterCriteria criteria, string query)
        {
            foreach (Match match in query_syntax_regex.Matches(query))
            {
                string key = match.Groups["key"].Value.ToLowerInvariant();
                var op = parseOperator(match.Groups["op"].Value);
                string value = match.Groups["value"].Value;

                if (tryParseKeywordCriteria(criteria, key, value, op))
                    query = query.Replace(match.ToString(), "");
            }

            criteria.SearchText = query;
        }

        private static bool tryParseKeywordCriteria(FilterCriteria criteria, string key, string value, Operator op)
        {
            switch (key)
            {
                case "star":
                case "stars":
                    return TryUpdateCriteriaRange(ref criteria.StarDifficulty, op, value, 0.01d / 2);

                case "ar":
                    return TryUpdateCriteriaRange(ref criteria.ApproachRate, op, value);

                case "dr":
                case "hp":
                    return TryUpdateCriteriaRange(ref criteria.DrainRate, op, value);

                case "cs":
                    return TryUpdateCriteriaRange(ref criteria.CircleSize, op, value);

                case "od":
                    return TryUpdateCriteriaRange(ref criteria.OverallDifficulty, op, value);

                case "bpm":
                    return TryUpdateCriteriaRange(ref criteria.BPM, op, value, 0.01d / 2);

                case "length":
                    return tryUpdateLengthRange(criteria, op, value);

                case "divisor":
                    return TryUpdateCriteriaRange(ref criteria.BeatDivisor, op, value, tryParseInt);

                case "status":
                    return TryUpdateCriteriaRange(ref criteria.OnlineStatus, op, value, tryParseEnum);

                case "creator":
                    return TryUpdateCriteriaText(ref criteria.Creator, op, value);

                case "artist":
                    return TryUpdateCriteriaText(ref criteria.Artist, op, value);

                default:
                    return criteria.RulesetCriteria?.TryParseCustomKeywordCriteria(key, op, value) ?? false;
            }
        }

        private static Operator parseOperator(string value)
        {
            switch (value)
            {
                case "=":
                case ":":
                    return Operator.Equal;

                case "<":
                    return Operator.Less;

                case "<=":
                case "<:":
                    return Operator.LessOrEqual;

                case ">":
                    return Operator.Greater;

                case ">=":
                case ">:":
                    return Operator.GreaterOrEqual;

                default:
                    throw new ArgumentOutOfRangeException(nameof(value), $"Unsupported operator {value}");
            }
        }
        
        private static int getLengthScale(string value) =>
            value.EndsWith("ms", StringComparison.Ordinal) ? 1 :
            value.EndsWith('s') ? 1000 :
            value.EndsWith('m') ? 60000 :
            value.EndsWith('h') ? 3600000 : 1000;

        private static bool tryParseFloatWithPoint(string value, out float result) =>
            float.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result);

        private static bool tryParseDoubleWithPoint(string value, out double result) =>
            double.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result);

        private static bool tryParseInt(string value, out int result) =>
            int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out result);
        
        private static bool tryParseEnum<TEnum>(string value, out TEnum result) where TEnum : struct
        {
            // First try an exact match.
            if (Enum.TryParse(value, true, out result))
                return true;

            // Then try a prefix match.
            string? prefixMatch = Enum.GetNames(typeof(TEnum)).FirstOrDefault(name => name.StartsWith(value, true, CultureInfo.InvariantCulture));

            if (prefixMatch == null)
                return false;

            return Enum.TryParse(prefixMatch, true, out result);
        }

        private static GroupCollection? tryMatchRegex(string value, string regex)
        {
            Match matches = Regex.Match(value, regex);

            if (matches.Success)
                return matches.Groups;

            return null;
        }
        
        public static bool TryUpdateCriteriaText(ref FilterCriteria.OptionalTextFilter textFilter, Operator op, string value)
        {
            switch (op)
            {
                case Operator.Equal:
                    textFilter.SearchTerm = value.Trim('"');
                    return true;

                default:
                    return false;
            }
        }
        
        public static bool TryUpdateCriteriaRange(ref FilterCriteria.OptionalRange<float> range, Operator op, string val, float tolerance = 0.05f)
            => tryParseFloatWithPoint(val, out float value) && tryUpdateCriteriaRange(ref range, op, value, tolerance);

        private static bool tryUpdateCriteriaRange(ref FilterCriteria.OptionalRange<float> range, Operator op, float value, float tolerance = 0.05f)
        {
            switch (op)
            {
                default:
                    return false;

                case Operator.Equal:
                    range.Min = value - tolerance;
                    range.Max = value + tolerance;
                    break;

                case Operator.Greater:
                    range.Min = value + tolerance;
                    break;

                case Operator.GreaterOrEqual:
                    range.Min = value - tolerance;
                    break;

                case Operator.Less:
                    range.Max = value - tolerance;
                    break;

                case Operator.LessOrEqual:
                    range.Max = value + tolerance;
                    break;
            }

            return true;
        }
        
        public static bool TryUpdateCriteriaRange(ref FilterCriteria.OptionalRange<double> range, Operator op, string val, double tolerance = 0.05)
            => tryParseDoubleWithPoint(val, out double value) && tryUpdateCriteriaRange(ref range, op, value, tolerance);

        private static bool tryUpdateCriteriaRange(ref FilterCriteria.OptionalRange<double> range, Operator op, double value, double tolerance = 0.05)
        {
            switch (op)
            {
                default:
                    return false;

                case Operator.Equal:
                    range.Min = value - tolerance;
                    range.Max = value + tolerance;
                    break;

                case Operator.Greater:
                    range.Min = value + tolerance;
                    break;

                case Operator.GreaterOrEqual:
                    range.Min = value - tolerance;
                    break;

                case Operator.Less:
                    range.Max = value - tolerance;
                    break;

                case Operator.LessOrEqual:
                    range.Max = value + tolerance;
                    break;
            }

            return true;
        }
        
        public delegate bool TryParseFunction<T>(string val, out T parsed);
        
        public static bool TryUpdateCriteriaRange<T>(ref FilterCriteria.OptionalRange<T> range, Operator op, string val, TryParseFunction<T> parseFunction)
            where T : struct
            => parseFunction.Invoke(val, out var converted) && tryUpdateCriteriaRange(ref range, op, converted);

        private static bool tryUpdateCriteriaRange<T>(ref FilterCriteria.OptionalRange<T> range, Operator op, T value)
            where T : struct
        {
            switch (op)
            {
                default:
                    return false;

                case Operator.Equal:
                    range.IsLowerInclusive = range.IsUpperInclusive = true;
                    range.Min = value;
                    range.Max = value;
                    break;

                case Operator.Greater:
                    range.IsLowerInclusive = false;
                    range.Min = value;
                    break;

                case Operator.GreaterOrEqual:
                    range.IsLowerInclusive = true;
                    range.Min = value;
                    break;

                case Operator.Less:
                    range.IsUpperInclusive = false;
                    range.Max = value;
                    break;

                case Operator.LessOrEqual:
                    range.IsUpperInclusive = true;
                    range.Max = value;
                    break;
            }

            return true;
        }
        
        private static bool tryUpdateLengthRange(FilterCriteria criteria, Operator op, string val)
        {
            List<string> parts = new List<string>();

            GroupCollection? match = null;

            match ??= tryMatchRegex(val, @"^((?<hours>\d+):)?(?<minutes>\d+):(?<seconds>\d+)$");
            match ??= tryMatchRegex(val, @"^((?<hours>\d+(\.\d+)?)h)?((?<minutes>\d+(\.\d+)?)m)?((?<seconds>\d+(\.\d+)?)s)?$");
            match ??= tryMatchRegex(val, @"^(?<seconds>\d+(\.\d+)?)$");

            if (match == null)
                return false;

            if (match["seconds"].Success)
                parts.Add(match["seconds"].Value + "s");
            if (match["minutes"].Success)
                parts.Add(match["minutes"].Value + "m");
            if (match["hours"].Success)
                parts.Add(match["hours"].Value + "h");

            double totalLength = 0;
            int minScale = 3600000;

            for (int i = 0; i < parts.Count; i++)
            {
                string part = parts[i];
                string partNoUnit = part.TrimEnd('m', 's', 'h');
                if (!tryParseDoubleWithPoint(partNoUnit, out double length))
                    return false;

                if (i != parts.Count - 1 && length >= 60)
                    return false;
                if (i != 0 && partNoUnit.Contains('.'))
                    return false;

                int scale = getLengthScale(part);
                totalLength += length * scale;
                minScale = Math.Min(minScale, scale);
            }

            return tryUpdateCriteriaRange(ref criteria.Length, op, totalLength, minScale / 2.0);
        }
    }
}
