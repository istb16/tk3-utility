using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public static partial class StringExtension
    {
        public static bool ToBool(this string value) => bool.Parse(value);
        public static int ToInt(this string value) => value.ToIntOrNull() ?? throw new ArgumentNullException(nameof(value));
        public static int? ToIntOrNull(this string value) => int.TryParse(value, out int rtval) ? rtval : null;
        public static DateTime ToDateTime(this string value) => value.ToDateTimeOrNull() ?? throw new ArgumentNullException(nameof(value));
        public static DateTime? ToDateTimeOrNull(this string? value) => DateTime.TryParse(value, out DateTime rtval) ? rtval : null;
    }
}
