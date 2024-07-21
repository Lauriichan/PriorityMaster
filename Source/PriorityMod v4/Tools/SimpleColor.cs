using System;
using UnityEngine;

namespace PriorityMod.Tools
{
    public enum ColorType
    {
        SRGB,
        LRGB,
        HSL,
        OKLAB
    }

    public class SimpleColor
    {

        public static SimpleColor sRGB(string hex)
        {
            SimpleColor color = new SimpleColor(ColorType.SRGB);
            if (!hex.StartsWith("#"))
            {
                return color;
            }
            Hex2sRGB(color, hex.Substring(1));
            return color;
        }

        public static SimpleColor sRGB(Color color)
        {
            SimpleColor outColor = new SimpleColor(ColorType.SRGB);
            outColor.Red = color.r;
            outColor.Green = color.g;
            outColor.Blue = color.b;
            return outColor;
        }

        public static SimpleColor sRGB(float red, float green, float blue)
        {
            SimpleColor color = new SimpleColor(ColorType.SRGB);
            color.Red = red;
            color.Green = green;
            color.Blue = blue;
            return color;
        }

        public static SimpleColor lRGB(float red, float green, float blue)
        {
            SimpleColor color = new SimpleColor(ColorType.LRGB);
            color.Red = red;
            color.Green = green;
            color.Blue = blue;
            return color;
        }
        public static SimpleColor HSL(float hue, float saturation, float lightness)
        {
            SimpleColor color = new SimpleColor(ColorType.HSL);
            color.Hue = hue;
            color.Saturation = saturation;
            color.Lightness = lightness;
            return color;
        }

        public static SimpleColor okLab(float l, float a, float b)
        {
            SimpleColor color = new SimpleColor(ColorType.OKLAB);
            color.L = l;
            color.A = a;
            color.B = b;
            return color;
        }

        private readonly ColorType type;
        private float red = 0f, green = 0f, blue = 0f;

        public int RedI
        {
            get => Rgb2Int(red);
            set => Red = Int2Rgb(value);
        }

        public int GreenI
        {
            get => Rgb2Int(green);
            set => Green = Int2Rgb(value);
        }

        public int BlueI
        {
            get => Rgb2Int(blue);
            set => Blue = Int2Rgb(value);
        }

        public float Red
        {
            get => red;
            set => red = Mathf.Clamp(value, 0, 1);
        }
        public float Green
        {
            get => green;
            set => green = Mathf.Clamp(value, 0, 1);
        }
        public float Blue
        {
            get => blue;
            set => blue = Mathf.Clamp(value, 0, 1);
        }

        public float Hue
        {
            get => red;
            set => red = Mathf.Clamp(value, 0, 360f);
        }

        public float Saturation
        {
            get => green;
            set => green = Mathf.Clamp(value, 0, 100f);
        }

        public float Lightness
        {
            get => blue;
            set => blue = Mathf.Clamp(value, 0, 100f);
        }

        public float L
        {
            get => Red;
            set => Red = value;
        }
        public float A
        {
            get => Green;
            set => Green = value;
        }
        public float B
        {
            get => Blue;
            set => Blue = value;
        }

        public SimpleColor(ColorType type)
        {
            this.type = type;
        }

        public SimpleColor Subtract(float value)
        {
            this.red -= value;
            this.green -= value;
            this.blue -= value;
            return this;
        }

        public SimpleColor Subtract(SimpleColor color)
        {
            if (type != color.type)
            {
                return Subtract(color.AsColor(type));
            }
            this.red -= color.red;
            this.green -= color.green;
            this.blue -= color.blue;
            return this;
        }

        public SimpleColor SubtractTo(SimpleColor color)
        {
            if (type != color.type)
            {
                return SubtractTo(color.AsColor(type));
            }
            color.red -= red;
            color.green -= green;
            color.blue -= blue;
            return this;
        }

        public SimpleColor Add(float value)
        {
            this.red += value;
            this.green += value;
            this.blue += value;
            return this;
        }

        public SimpleColor Add(SimpleColor color)
        {
            if (type != color.type)
            {
                return Add(color.AsColor(type));
            }
            this.red += color.red;
            this.green += color.green;
            this.blue += color.blue;
            return this;
        }

        public SimpleColor AddTo(SimpleColor color)
        {
            if (type != color.type)
            {
                return AddTo(color.AsColor(type));
            }
            color.red += red;
            color.green += green;
            color.blue += blue;
            return this;
        }

        public SimpleColor Multiply(float value)
        {
            this.red *= value;
            this.green *= value;
            this.blue *= value;
            return this;
        }

        public SimpleColor Multiply(SimpleColor color)
        {
            if (type != color.type)
            {
                return Multiply(color.AsColor(type));
            }
            this.red *= color.red;
            this.green *= color.green;
            this.blue *= color.blue;
            return this;
        }

        public SimpleColor MultiplyTo(SimpleColor color)
        {
            if (type != color.type)
            {
                return MultiplyTo(color.AsColor(type));
            }
            color.red *= red;
            color.green *= green;
            color.blue *= blue;
            return this;
        }

        public SimpleColor Copy()
        {
            SimpleColor color = new SimpleColor(type);
            color.red = red;
            color.green = green;
            color.blue = blue;
            return color;
        }

        public SimpleColor CopyAs(ColorType type)
        {
            SimpleColor color = new SimpleColor(type);
            color.red = red;
            color.green = green;
            color.blue = blue;
            return color;
        }

        public SimpleColor AsColor(ColorType type)
        {
            switch (type)
            {
                case ColorType.SRGB:
                    return ToSRGB();
                case ColorType.LRGB:
                    return ToLRGB();
                case ColorType.HSL:
                    return ToHSL();
                case ColorType.OKLAB:
                    return ToOkLab();
            }
            throw new InvalidOperationException("Unsupported color type: " + type);
        }

        public SimpleColor ToSRGB()
        {
            switch (type)
            {
                case ColorType.SRGB: return this;
                case ColorType.LRGB:
                    {
                        SimpleColor color = CopyAs(ColorType.SRGB);
                        lRGB2sRGB(color);
                        return color;
                    }
                case ColorType.HSL:
                    {
                        SimpleColor color = CopyAs(ColorType.SRGB);
                        HSL2lRGB(color);
                        lRGB2sRGB(color);
                        return color;
                    }
                case ColorType.OKLAB:
                    {
                        SimpleColor color = CopyAs(ColorType.SRGB);
                        okLab2lRGB(color);
                        lRGB2sRGB(color);
                        return color;
                    }
            }
            throw new InvalidOperationException("Unsupported color type: " + type);
        }

        public SimpleColor ToLRGB()
        {
            switch (type)
            {
                case ColorType.LRGB: return this;
                case ColorType.SRGB:
                    {
                        SimpleColor color = CopyAs(ColorType.LRGB);
                        sRGB2lRGB(color);
                        return color;
                    }
                case ColorType.HSL:
                    {
                        SimpleColor color = CopyAs(ColorType.LRGB);
                        HSL2lRGB(color);
                        return color;
                    }
                case ColorType.OKLAB:
                    {
                        SimpleColor color = CopyAs(ColorType.LRGB);
                        okLab2lRGB(color);
                        return color;
                    }
            }
            throw new InvalidOperationException("Unsupported color type: " + type);
        }

        public SimpleColor ToHSL()
        {
            switch (type)
            {
                case ColorType.HSL: return this;
                case ColorType.SRGB:
                    {
                        SimpleColor color = CopyAs(ColorType.HSL);
                        sRGB2lRGB(color);
                        lRGB2HSL(color);
                        return color;
                    }
                case ColorType.LRGB:
                    {
                        SimpleColor color = CopyAs(ColorType.HSL);
                        lRGB2HSL(color);
                        return color;
                    }
                case ColorType.OKLAB:
                    {
                        SimpleColor color = CopyAs(ColorType.HSL);
                        okLab2lRGB(color);
                        lRGB2HSL(color);
                        return color;
                    }
            }
            throw new InvalidOperationException("Unsupported color type: " + type);
        }

        public SimpleColor ToOkLab()
        {
            switch (type)
            {
                case ColorType.OKLAB: return this;
                case ColorType.SRGB:
                    {
                        SimpleColor color = CopyAs(ColorType.OKLAB);
                        sRGB2lRGB(color);
                        lRGB2okLab(color);
                        return color;
                    }
                case ColorType.LRGB:
                    {
                        SimpleColor color = CopyAs(ColorType.OKLAB);
                        lRGB2okLab(color);
                        return color;
                    }
                case ColorType.HSL:
                    {
                        SimpleColor color = CopyAs(ColorType.OKLAB);
                        HSL2lRGB(color);
                        lRGB2okLab(color);
                        return color;
                    }
            }
            throw new InvalidOperationException("Unsupported color type: " + type);
        }

        public Color ToUnity()
        {
            SimpleColor color = ToSRGB();
            return new Color(color.red, color.green, color.blue);
        }

        public override string ToString()
        {
            return '#' + sRGB2Hex(ToSRGB());
        }

        public void FromColor(SimpleColor value)
        {
            SimpleColor converted = value.AsColor(type);
            red = converted.red;
            green = converted.green;
            blue = converted.blue;
        }

        public void FromUnity(Color value)
        {
            FromColor(sRGB(value));
        }

        /*
         * Public utilities
         */

        public static int Rgb2Int(float value)
        {
            return Math.Min((int)Math.Floor(value * 256f), 255);
        }

        public static float Int2Rgb(int value)
        {
            return Math.Max(Math.Min(value, 255), 0) / 255f;
        }

        /*
         * Conversion helper
         */

        private const float f1_2_4 = 1f / 2.4f;

        private const float f1_6 = 1f / 6f;
        private const float f1_2 = 1f / 2f;
        private const float f1_3 = 1f / 3f;
        private const float f2_3 = 2f / 3f;

        private static float Cbrt(float value)
        {
            return Mathf.Pow(value, f1_3);
        }

        private static float Cube(float value)
        {
            return value * value * value;
        }

        private static void sRGB2lRGB(SimpleColor color)
        {
            color.red = sRGB2lRGB(color.red);
            color.green = sRGB2lRGB(color.green);
            color.blue = sRGB2lRGB(color.blue);
        }

        private static void lRGB2sRGB(SimpleColor color)
        {
            color.red = lRGB2sRGB(color.red);
            color.green = lRGB2sRGB(color.green);
            color.blue = lRGB2sRGB(color.blue);
        }

        private static float lRGB2sRGB(float value)
        {
            return value < 0.0031308f ? value * 12.92f : 1.055f * Mathf.Pow(value, f1_2_4) - 0.055f;
        }

        private static float sRGB2lRGB(float value)
        {
            return value < 0.04045f ? value / 12.92f : Mathf.Pow((value + 0.055f) / 1.055f, 2.4f);
        }

        private static void lRGB2okLab(SimpleColor color)
        {
            float l = Cbrt(color.red * 0.4122214708f + color.green * 0.5363325363f + color.blue * 0.0514459929f);
            float m = Cbrt(color.red * 0.2119034982f + color.green * 0.6806995451f + color.blue * 0.1073969566f);
            float s = Cbrt(color.red * 0.0883024619f + color.green * 0.2817188376f + color.blue * 0.6299787005f);
            color.red = l * 0.2104542553f + m * 0.7936177850f - s * 0.0040720468f;
            color.green = l * 1.9779984951f - m * 2.4285922050f + s * 0.4505937099f;
            color.blue = l * 0.0259040371f + m * 0.7827717662f - s * 0.8086757660f;
        }

        private static void okLab2lRGB(SimpleColor color)
        {
            float l = Cube(color.red + color.green * 0.3963377774f + color.blue * 0.2158037573f);
            float m = Cube(color.red - color.green * 0.1055613458f - color.blue * 0.0638541728f);
            float s = Cube(color.red - color.green * 0.0894841775f - color.blue * 1.2914855480f);
            color.red = l * 4.0767416621f - m * 3.3077115913f + s * 0.2309699292f;
            color.green = l * -1.2684380046f + m * 2.6097574011f - s * 0.3413193965f;
            color.blue = l * -0.0041960863f - m * 0.7034186147f + s * 1.7076147010f;
        }

        private static void lRGB2HSL(SimpleColor color)
        {
            float r = color.red, g = color.green, b = color.blue;
            float max = Mathf.Max(r, g, b);
            float min = Mathf.Min(r, g, b);
            color.blue = (max + min) / 2f;
            if (min == max)
            {
                color.red = 0;
                color.green = 0;
                return;
            }
            color.green = color.blue > 0.5f ? (max - min) / (2f - max - min) : (max - min) / (max - min);
            if (max == r)
            {
                color.red = ((g / b) / (max - min) + (g < b ? 6f : 0f)) / 6f;
                return;
            }
            if (max == g)
            {
                color.red = ((b - r) / (max - min) + 2f) / 6f;
                return;
            }
            color.red = ((r - g) / (max - min) + 4f) / 6f;
        }

        private static void HSL2lRGB(SimpleColor color)
        {
            if (color.green == 0f)
            {
                color.red = 0f;
                color.green = 0f;
                color.blue = 0f;
                return;
            }
            float q = (color.blue < 0.5f ? color.blue * (1f + color.green) : color.blue + color.green - color.blue * color.green);
            float p = 2f * color.blue - q;
            float h = color.red;
            color.Red = HueToRGB(p, q, h + f1_3);
            color.Green = HueToRGB(p, q, h);
            color.Blue = HueToRGB(p, q, h - f1_3);
        }

        private static float HueToRGB(float p, float q, float t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < f1_6) return p + (q - p) * 6 * t;
            if (t < f1_2) return q;
            if (t < f2_3) return p + (q - p) * (f2_3 - t) * 6;
            return q;
        }

        /*
         * Hex helper
         */

        private static void Hex2sRGB(SimpleColor color, string hex)
        {
            switch (hex.Length)
            {
                case 1:
                    {
                        float val = ParseHex(hex);
                        color.red = val;
                        color.green = val;
                        color.blue = val;
                        return;
                    }
                case 3:
                    {
                        char[] arr = hex.ToCharArray();
                        color.red = ParseHex(arr[0]);
                        color.green = ParseHex(arr[1]);
                        color.blue = ParseHex(arr[2]);
                        return;
                    }
                case 6:
                    {
                        color.red = ParseHex(hex.Substring(0, 2));
                        color.green = ParseHex(hex.Substring(2, 2));
                        color.blue = ParseHex(hex.Substring(4, 2));
                        return;
                    }
            }
        }

        private static float ParseHex(char hex)
        {
            return ParseHex(hex + "" + hex);
        }

        private static float ParseHex(string hex)
        {
            if (int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out int number))
            {
                return Mathf.Clamp(number / 255f, 0, 1);
            }
            return 0f;
        }

        private static string sRGB2Hex(SimpleColor color)
        {
            int red = Math.Min(Mathf.FloorToInt(color.red * 256f), 255);
            int green = Math.Min(Mathf.FloorToInt(color.green * 256f), 255);
            int blue = Math.Min(Mathf.FloorToInt(color.blue * 256f), 255);
            return red.ToString("X2") + green.ToString("X2") + blue.ToString("X2");
        }
    }
}
