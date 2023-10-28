using PriorityMod.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PriorityMod.Extensions
{
    public static class UnityExtensions
    {

        public static String ToHex(this Color color)
        {
            return '#' + color.r.RGBToInt().IntToHex() + color.g.RGBToInt().IntToHex() + color.b.RGBToInt().IntToHex();
        }

        public static Color ToColor(this String hex)
        {
            if (!hex.StartsWith("#"))
            {
                return Color.black;
            }
            String tmp = hex.Substring(1);
            switch (tmp.Length)
            {
                case 1:
                    hex = "#" + tmp + tmp + tmp + tmp + tmp + tmp;
                    break;
                case 3:
                    char[] arr = tmp.ToCharArray();
                    hex = "#" + arr[0] + arr[0] + arr[1] + arr[1] + arr[2] + arr[2];
                    break;
                case 6:
                    hex = "#" + tmp;
                    break;
                default:
                    return Color.black;
            }
            tmp = hex.Substring(1);
            return new Color(tmp.Substring(0, 2).HexToInt() / 255f, tmp.Substring(2, 2).HexToInt() / 255f, tmp.Substring(4, 2).HexToInt() / 255f, 1f);
        }

        public static void ToColor(this List<String> list, ref List<Color> colors)
        {
            foreach (String hex in list)
            {
                colors.Add(hex.ToColor());
            }
        }

        public static void ToRGB(this Color color, out int red, out int green, out int blue)
        {
            red = color.r.RGBToInt();
            green = color.g.RGBToInt();
            blue = color.b.RGBToInt();
        }

        public static void ToHSL(this Color color, out float hue, out float saturation, out float lightness)
        {
            color.ToRGB(out int red, out int green, out int blue);
            Utils.RGBToHSL(red, green, blue, out hue, out saturation, out lightness);
        }

        public static Color Interpolate(this float ratio, Color col1, Color col2)
        {
            return new Color(ratio.Interpolate(col1.r, col2.r), ratio.Interpolate(col1.g, col2.g), ratio.Interpolate(col1.b, col2.b), 1f);
        }

        private static float Interpolate(this float ratio, float from, float to)
        {
            return from * ratio + to * (1.0f - ratio);
        }

    }
}
