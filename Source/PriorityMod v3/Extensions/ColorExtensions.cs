using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriorityMod.Extensions
{
    public static class ColorExtensions
    {

        public static int RGBToInt(this float value)
        {
            return Math.Min((int)Math.Floor(value * 256f), 255);
        }

        public static float RGBToFloat(this int value)
        {
            return value / 255f;
        }

        public static int HexToInt(this String value)
        {
            if (int.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out int number))
            {
                return number;
            }
            return 0;
        }

        public static String IntToHex(this int value)
        {
            return value.ToString("X2");
        }

    }
}
