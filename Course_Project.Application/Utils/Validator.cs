using Course_Project.Domain.Models.CustomIdModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Course_Project.Application.Utils
{
    public static class Validator
    {
        public static bool Validate(string str, List<CustomIdRule> rules, int seqValue = 0)
        {
            if (string.IsNullOrEmpty(str) || rules == null || rules.Count == 0)
                return false;

            int idx = 0;
            for (int i = 0; i < rules.Count; i++)
            {
                var isLast = i == rules.Count - 1;
                string pattern = BuildPattern(rules[i], seqValue, isLast);
                var match = Regex.Match(str.Substring(idx), pattern);
                if (!match.Success) return false;
                idx += match.Length;
            }
            return idx == str.Length;
        }

        private static string BuildPattern(CustomIdRule r, int seq, bool isLast)
        {
            string sep = GetSeparator(r, isLast);
            return r.IdType switch
            {
                IdType.Text => BuildTextPattern(sep),
                IdType.Rand6Digit => BuildRand6DigitPattern(sep),
                IdType.Rand9Digit => BuildRand9DigitPattern(sep),
                IdType.GUID => BuildGuidPattern(sep),
                IdType.DateTime => BuildDateTimePattern(r, sep),
                IdType.Sequence => BuildSequencePattern(seq, sep),
                IdType.Rand20Bit => BuildRand20BitPattern(r, sep),
                IdType.Rand32Bit => BuildRand32BitPattern(r, sep),
                _ => throw new NotSupportedException($"Unknown IdType: {r.IdType}")
            };
        }

        private static string GetSeparator(CustomIdRule r, bool isLast)
        {
            if (isLast) return "";
            return r.IdType switch
            {
                IdType.Rand20Bit => "-",
                IdType.Rand32Bit => "-",
                IdType.DateTime => "-",
                _ => string.IsNullOrEmpty(r.Rule) ? "" : Regex.Escape(r.Rule)
            };
        }

        private static string BuildTextPattern(string sep) => @"^.{5}" + sep;
        private static string BuildRand6DigitPattern(string sep) => @"^[1-9][0-9]{5}" + sep;
        private static string BuildRand9DigitPattern(string sep) => @"^[1-9][0-9]{8}" + sep;
        private static string BuildGuidPattern(string sep) => @"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}" + sep;
        private static string BuildDateTimePattern(CustomIdRule r, string sep) =>
            r.Rule?.ToLower() switch
            {
                "year" => @"^guid\.year\(\)" + sep,
                "month" => @"^guid\.month\(\)" + sep,
                "date" => @"^guid\.date\(\)" + sep,
                _ => @"^(year|month|date)" + sep
            };
        private static string BuildSequencePattern(int seq, string sep) => $"^{seq}" + sep;
        private static string BuildRand20BitPattern(CustomIdRule r, string sep) =>
            r.Rule?.ToLower() == "zeros" ? @"^[0-9A-Fa-f]{1,8}" + sep : @"^[1-9A-Fa-f][0-9A-Fa-f]{0,7}" + sep;
        private static string BuildRand32BitPattern(CustomIdRule r, string sep) =>
            r.Rule?.ToLower() == "zeros" ? @"^[0-9A-Fa-f]{1,10}" + sep : @"^[1-9A-Fa-f][0-9A-Fa-f]{0,9}" + sep;
    }

}

