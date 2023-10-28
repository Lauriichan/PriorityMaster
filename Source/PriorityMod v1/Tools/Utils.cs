using PriorityMod.Extensions;
using System;
using UnityEngine;

namespace PriorityMod.Tools
{
    public static class Utils
    {

        public static readonly float f1_6 = 1f / 6f;
        public static readonly float f1_2 = 1f / 2f;
        public static readonly float f1_3 = 1f / 3f;
        public static readonly float f2_3 = 2f / 3f;

        private static int HueToRGB(float p, float q, float t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < f1_6) return (p + (q - p) * 6 * t).RGBToInt();
            if (t < f1_2) return q.RGBToInt();
            if (t < f2_3) return (p + (q - p) * (f2_3 - t) * 6).RGBToInt();
            return p.RGBToInt();
        }

        public static void RGBToHSL(int red, int green, int blue, out float hue, out float saturation, out float lightness)
        {
            float r = red.RGBToFloat();
            float g = green.RGBToFloat();
            float b = blue.RGBToFloat();
            float max = Math.Max(Math.Max(r, g), b);
            float min = Math.Min(Math.Min(r, g), b);
            lightness = (max + min) / 2;
            if(min == max)
            {
                hue = saturation = 0;
                return;
            }
            float d = max - min;
            saturation = lightness > 0.5f ? d / (2 - max - min) : d / (max + min);
            if(max == r)
            {
                hue = ((g - b) / d + (g < b ? 6 : 0)) / 6;
                return;
            }
            if(max == g)
            {
                hue = ((b - r) / d + 2) / 6;
                return;
            }
            hue = ((r - g) / d + 4) / 6;
        }

        public static void HSLToRGB(float hue, float saturation, float lightness, out int red, out int green, out int blue)
        {
            if(saturation == 0)
            {
                red = green = blue = lightness.RGBToInt();
                return;
            }

            float q = (lightness < 0.5f ? lightness * (1 + saturation) : lightness + saturation - lightness * saturation);
            float p = 2 * lightness - q;
            red = HueToRGB(p, q, hue + f1_3);
            green = HueToRGB(p, q, hue);
            blue = HueToRGB(p, q, hue - f1_3);
        }

        public static Color FromHSL(float hue, float saturation, float lightness)
        {
            HSLToRGB(hue, saturation, lightness, out int red, out int green, out int blue);
            return FromRGB(red, green, blue);
        }

        public static Color FromRGB(int red, int green, int blue)
        {
            return new Color(red.RGBToFloat(), green.RGBToFloat(), blue.RGBToFloat(), 1f);
        }

    }
}
