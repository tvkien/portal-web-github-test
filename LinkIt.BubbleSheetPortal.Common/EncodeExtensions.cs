using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Common
{
    public static class EncodeExtensions
    {
        public static string HexEncode(this byte[] input)
        {
            var sb = new StringBuilder(input.Length * 2);
            foreach (var b in input)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
    }
}
