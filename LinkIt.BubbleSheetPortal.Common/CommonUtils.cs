using System;
using System.Collections.Generic;
using System.Linq;
using static LinkIt.BubbleSheetPortal.Common.Constanst;

namespace LinkIt.BubbleSheetPortal.Common
{
    public static class CommonUtils
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public static int ConverStringToInt(string strInt, int defaultValue)
        {
            if (!string.IsNullOrWhiteSpace(strInt))
            {
                int iOutPut = -1;
                if (int.TryParse(strInt, out iOutPut))
                    return iOutPut;
            }
            return defaultValue;
        }
        public static decimal? RoundNumber(decimal? number, int digit = NumberFormats.DECIMAL_DEFAULT_DIGIT)
        {
            if (!number.HasValue)
                return number;
            return Math.Round(number.Value, digit, MidpointRounding.AwayFromZero);
        }

        public static int GetMaxDecimalPlace(IEnumerable<string> numbers)
        {
            try
            {
                return numbers.Select(x => x.Split('.'))
                .Where(parts => parts.Length > 1)
                .Select(parts => parts[1].Length)
                .DefaultIfEmpty(0)
                .Max();
            }
            catch { }

            return default;
        }
    }
}
