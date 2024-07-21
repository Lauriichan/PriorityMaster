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
        private float red_hue_l = 0f, green_saturation_a = 0f, blue_lightness_b = 0f;

        public int RedI
        {
            get => RGB2Int(red_hue_l);
            set => Red = Int2RGB(value);
        }

        public int GreenI
        {
            get => RGB2Int(green_saturation_a);
            set => Green = Int2RGB(value);
        }

        public int BlueI
        {
            get => RGB2Int(blue_lightness_b);
            set => Blue = Int2RGB(value);
        }

        public float Red
        {
            get => red_hue_l;
            set => red_hue_l = Mathf.Clamp(value, 0, 1);
        }
        public float Green
        {
            get => green_saturation_a;
            set => green_saturation_a = Mathf.Clamp(value, 0, 1);
        }
        public float Blue
        {
            get => blue_lightness_b;
            set => blue_lightness_b = Mathf.Clamp(value, 0, 1);
        }

        public float Hue
        {
            get => red_hue_l;
            set
            {
                if (value < 0f)
                {
                    value *= -1;
                }
                if (value > 1f)
                {
                    value %= 1f;
                }
                red_hue_l = value;
            }
        }

        public float Saturation
        {
            get => green_saturation_a;
            set => green_saturation_a = Mathf.Clamp(value, 0f, 1f);
        }

        public float Lightness
        {
            get => blue_lightness_b;
            set => blue_lightness_b = Mathf.Clamp(value, 0f, 1f);
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
            this.red_hue_l -= value;
            this.green_saturation_a -= value;
            this.blue_lightness_b -= value;
            return this;
        }

        public SimpleColor Subtract(SimpleColor color)
        {
            if (type != color.type)
            {
                return Subtract(color.AsColor(type));
            }
            this.red_hue_l -= color.red_hue_l;
            this.green_saturation_a -= color.green_saturation_a;
            this.blue_lightness_b -= color.blue_lightness_b;
            return this;
        }

        public SimpleColor SubtractTo(SimpleColor color)
        {
            if (type != color.type)
            {
                return SubtractTo(color.AsColor(type));
            }
            color.red_hue_l -= red_hue_l;
            color.green_saturation_a -= green_saturation_a;
            color.blue_lightness_b -= blue_lightness_b;
            return this;
        }

        public SimpleColor Add(float value)
        {
            this.red_hue_l += value;
            this.green_saturation_a += value;
            this.blue_lightness_b += value;
            return this;
        }

        public SimpleColor Add(SimpleColor color)
        {
            if (type != color.type)
            {
                return Add(color.AsColor(type));
            }
            this.red_hue_l += color.red_hue_l;
            this.green_saturation_a += color.green_saturation_a;
            this.blue_lightness_b += color.blue_lightness_b;
            return this;
        }

        public SimpleColor AddTo(SimpleColor color)
        {
            if (type != color.type)
            {
                return AddTo(color.AsColor(type));
            }
            color.red_hue_l += red_hue_l;
            color.green_saturation_a += green_saturation_a;
            color.blue_lightness_b += blue_lightness_b;
            return this;
        }

        public SimpleColor Multiply(float value)
        {
            this.red_hue_l *= value;
            this.green_saturation_a *= value;
            this.blue_lightness_b *= value;
            return this;
        }

        public SimpleColor Multiply(SimpleColor color)
        {
            if (type != color.type)
            {
                return Multiply(color.AsColor(type));
            }
            this.red_hue_l *= color.red_hue_l;
            this.green_saturation_a *= color.green_saturation_a;
            this.blue_lightness_b *= color.blue_lightness_b;
            return this;
        }

        public SimpleColor MultiplyTo(SimpleColor color)
        {
            if (type != color.type)
            {
                return MultiplyTo(color.AsColor(type));
            }
            color.red_hue_l *= red_hue_l;
            color.green_saturation_a *= green_saturation_a;
            color.blue_lightness_b *= blue_lightness_b;
            return this;
        }

        public SimpleColor Copy()
        {
            SimpleColor color = new SimpleColor(type);
            color.red_hue_l = red_hue_l;
            color.green_saturation_a = green_saturation_a;
            color.blue_lightness_b = blue_lightness_b;
            return color;
        }

        public SimpleColor CopyAs(ColorType type)
        {
            SimpleColor color = new SimpleColor(type);
            color.red_hue_l = red_hue_l;
            color.green_saturation_a = green_saturation_a;
            color.blue_lightness_b = blue_lightness_b;
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
                        HSL2sRGB(color);
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
                        HSL2sRGB(color);
                        sRGB2lRGB(color);
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
                        sRGB2HSL(color);
                        return color;
                    }
                case ColorType.LRGB:
                    {
                        SimpleColor color = CopyAs(ColorType.HSL);
                        lRGB2sRGB(color);
                        sRGB2HSL(color);
                        return color;
                    }
                case ColorType.OKLAB:
                    {
                        SimpleColor color = CopyAs(ColorType.HSL);
                        okLab2lRGB(color);
                        lRGB2sRGB(color);
                        sRGB2HSL(color);
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
                        HSL2sRGB(color);
                        sRGB2lRGB(color);
                        lRGB2okLab(color);
                        return color;
                    }
            }
            throw new InvalidOperationException("Unsupported color type: " + type);
        }

        public Color ToUnity()
        {
            SimpleColor color = ToSRGB();
            return new Color(color.red_hue_l, color.green_saturation_a, color.blue_lightness_b);
        }

        public override string ToString()
        {
            return '#' + sRGB2Hex(ToSRGB());
        }

        public void FromColor(SimpleColor value)
        {
            SimpleColor converted = value.AsColor(type);
            red_hue_l = converted.red_hue_l;
            green_saturation_a = converted.green_saturation_a;
            blue_lightness_b = converted.blue_lightness_b;
        }

        public void FromUnity(Color value)
        {
            FromColor(sRGB(value));
        }

        /*
         * Public utilities
         */

        public static int RGB2Int(float value)
        {
            return Math.Min((int)Math.Floor(value * 256f), 255);
        }

        public static float Int2RGB(int value)
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
            color.red_hue_l = sRGB2lRGB(color.red_hue_l);
            color.green_saturation_a = sRGB2lRGB(color.green_saturation_a);
            color.blue_lightness_b = sRGB2lRGB(color.blue_lightness_b);
        }

        private static void lRGB2sRGB(SimpleColor color)
        {
            color.red_hue_l = lRGB2sRGB(color.red_hue_l);
            color.green_saturation_a = lRGB2sRGB(color.green_saturation_a);
            color.blue_lightness_b = lRGB2sRGB(color.blue_lightness_b);
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
            float l = Cbrt(color.red_hue_l * 0.4122214708f + color.green_saturation_a * 0.5363325363f + color.blue_lightness_b * 0.0514459929f);
            float m = Cbrt(color.red_hue_l * 0.2119034982f + color.green_saturation_a * 0.6806995451f + color.blue_lightness_b * 0.1073969566f);
            float s = Cbrt(color.red_hue_l * 0.0883024619f + color.green_saturation_a * 0.2817188376f + color.blue_lightness_b * 0.6299787005f);
            color.red_hue_l = l * 0.2104542553f + m * 0.7936177850f - s * 0.0040720468f;
            color.green_saturation_a = l * 1.9779984951f - m * 2.4285922050f + s * 0.4505937099f;
            color.blue_lightness_b = l * 0.0259040371f + m * 0.7827717662f - s * 0.8086757660f;
        }

        private static void okLab2lRGB(SimpleColor color)
        {
            float l = Cube(color.red_hue_l + color.green_saturation_a * 0.3963377774f + color.blue_lightness_b * 0.2158037573f);
            float m = Cube(color.red_hue_l - color.green_saturation_a * 0.1055613458f - color.blue_lightness_b * 0.0638541728f);
            float s = Cube(color.red_hue_l - color.green_saturation_a * 0.0894841775f - color.blue_lightness_b * 1.2914855480f);
            color.red_hue_l = l * 4.0767416621f - m * 3.3077115913f + s * 0.2309699292f;
            color.green_saturation_a = l * -1.2684380046f + m * 2.6097574011f - s * 0.3413193965f;
            color.blue_lightness_b = l * -0.0041960863f - m * 0.7034186147f + s * 1.7076147010f;
        }

        private static void sRGB2HSL(SimpleColor color)
        {
            float r = color.red_hue_l, g = color.green_saturation_a, b = color.blue_lightness_b;
            float max = Mathf.Max(r, g, b);
            float min = Mathf.Min(r, g, b);
            color.blue_lightness_b = (max + min) / 2;
            if (max == min)
            {
                color.red_hue_l = color.green_saturation_a = 0;
                return;
            }
            float d = max - min;
            color.green_saturation_a = d / (1 - Mathf.Abs(2 * color.blue_lightness_b - 1));
            if (max == r)
            {
                color.red_hue_l = ((g - b) / d + (g < b ? 6 : 0)) / 6;
                return;
            }
            if (max == g)
            {
                color.red_hue_l = ((b - r) / d + 2) / 6;
                return;
            }
            color.red_hue_l = ((r - g) / d + 4) / 6;
        }

        private static void HSL2sRGB(SimpleColor color)
        {
            if (color.green_saturation_a == 0f)
            {
                color.red_hue_l = color.green_saturation_a = color.blue_lightness_b;
                return;
            }
            float _h = color.red_hue_l, _s = color.green_saturation_a, _l = color.blue_lightness_b;
            float q = _l < 0.5 ? _l * (1 + _s) : _l + _s - _l * _s;
            float p = 2f * _l - q;
            color.Red = HueToRGB(p, q, _h + f1_3);
            color.Green = HueToRGB(p, q, _h);
            color.Blue = HueToRGB(p, q, _h - f1_3);
        }

        private static float HueToRGB(float p, float q, float t)
        {
            if (t < 0f) t += 1f;
            if (t > 1f) t -= 1f;
            if (t < f1_6) return p + (q - p) * 6f * t;
            if (t < f1_2) return q;
            if (t < f2_3) return p + (q - p) * (f2_3 - t) * 6f;
            return p;
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
                        color.red_hue_l = val;
                        color.green_saturation_a = val;
                        color.blue_lightness_b = val;
                        return;
                    }
                case 3:
                    {
                        char[] arr = hex.ToCharArray();
                        color.red_hue_l = ParseHex(arr[0]);
                        color.green_saturation_a = ParseHex(arr[1]);
                        color.blue_lightness_b = ParseHex(arr[2]);
                        return;
                    }
                case 6:
                    {
                        color.red_hue_l = ParseHex(hex.Substring(0, 2));
                        color.green_saturation_a = ParseHex(hex.Substring(2, 2));
                        color.blue_lightness_b = ParseHex(hex.Substring(4, 2));
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
                return Int2RGB(number);
            }
            return 0f;
        }

        private static string sRGB2Hex(SimpleColor color)
        {
            return RGB2Int(color.red_hue_l).ToString("X2") + RGB2Int(color.green_saturation_a).ToString("X2") + RGB2Int(color.red_hue_l).ToString("X2");
        }
    }
}
