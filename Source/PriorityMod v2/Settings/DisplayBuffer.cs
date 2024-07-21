using System;
using UnityEngine;
using PriorityMod.Tools;

namespace PriorityMod.Settings
{
    public class DisplayBuffer
    {

        private readonly PrioritySettings settings;

        public DisplayBuffer(PrioritySettings settings)
        {
            this.settings = settings;
        }


        public volatile int selected = -1;

        private SimpleColor rgb = new SimpleColor(ColorType.SRGB);
        private SimpleColor hsl = new SimpleColor(ColorType.HSL);

        public SimpleColor Color
        {
            get => rgb.Copy();
            set {
                if (value.Red == rgb.Red && value.Green == rgb.Green && value.Blue == rgb.Blue)
                    return;
                rgb.FromColor(value);
                hsl.FromColor(value);
                _hex = rgb.ToString();
                settings.SetColor(selected, Color);
            }
        }

        public Color UnityColor
        {
            get => rgb.ToUnity();
            set
            {
                SimpleColor newColor = SimpleColor.sRGB(value);
                if (newColor.Red == rgb.Red && newColor.Green == rgb.Green && newColor.Blue == rgb.Blue)
                    return;
                rgb.FromColor(newColor);
                hsl.FromColor(newColor);
                _hex = rgb.ToString();
                settings.SetColor(selected, Color);
            }
        }

        public int RedI
        {
            get => rgb.RedI;
            set
            {
                int prev = rgb.RedI;
                rgb.RedI = value;
                if (value == prev)
                    return;
                hsl.FromColor(rgb);
                _hex = rgb.ToString();
                settings.SetColor(selected, Color);
            }
        }

        public int GreenI
        {
            get => rgb.GreenI;
            set {
                int prev = rgb.GreenI;
                rgb.GreenI = value;
                if (value == prev)
                    return;
                hsl.FromColor(rgb);
                _hex = rgb.ToString();
                settings.SetColor(selected, Color);
            }
        }

        public int BlueI
        {
            get => rgb.BlueI;
            set
            {
                int prev = rgb.BlueI;
                rgb.BlueI = value;
                if (value == prev)
                    return;
                hsl.FromColor(rgb);
                _hex = rgb.ToString();
                settings.SetColor(selected, Color);
            }
        }

        public float Red
        {
            get => rgb.Red;
            set
            {
                float prev = rgb.Red;
                rgb.Red = value;
                if (value == prev)
                    return;
                hsl.FromColor(rgb);
                _hex = rgb.ToString();
                settings.SetColor(selected, Color);
            }
        }

        public float Green
        {
            get => rgb.Green;
            set
            {
                float prev = rgb.Green;
                rgb.Green = value;
                if (value == prev)
                    return;
                hsl.FromColor(rgb);
                _hex = rgb.ToString();
                settings.SetColor(selected, Color);
            }
        }

        public float Blue
        {
            get => rgb.Blue;
            set
            {
                float prev = rgb.Blue;
                rgb.Blue = value;
                if (value == prev)
                    return;
                hsl.FromColor(rgb);
                _hex = rgb.ToString();
                settings.SetColor(selected, Color);
            }
        }
        public float Hue
        {
            get => hsl.Hue;
            set
            {
                float prev = hsl.Hue;
                hsl.Hue = value;
                if (value == prev)
                    return;
                rgb.FromColor(hsl);
                _hex = rgb.ToString();
                settings.SetColor(selected, Color);
            }
        }

        public float Saturation
        {
            get => hsl.Saturation;
            set
            {
                float prev = hsl.Saturation;
                hsl.Saturation = value;
                if (value == prev)
                    return;
                rgb.FromColor(hsl);
                _hex = rgb.ToString();
                settings.SetColor(selected, Color);
            }
        }

        public float Lightness
        {
            get => hsl.Lightness;
            set
            {
                float prev = hsl.Lightness;
                hsl.Lightness = value;
                if (value == prev)
                    return;
                rgb.FromColor(hsl);
                _hex = rgb.ToString();
                settings.SetColor(selected, Color);
            }
        }

        private String _hex = "#000000";
        public String Hex
        {
            get => _hex;
            set
            {
                SimpleColor parsedHex = SimpleColor.sRGB(value);
                if (parsedHex.Red == rgb.Red && parsedHex.Green == rgb.Green && parsedHex.Blue == rgb.Blue)
                    return;
                rgb.FromColor(parsedHex);
                hsl.FromColor(parsedHex);
                _hex = value.StartsWith("#") ? value : '#' + value;
                settings.SetColor(selected, Color);
            }
        }

        /*
         * 
         * 
         * 
         */

        public String maxPriority;
        public String defPriority;

        public int priorityMaxRef = 20;
        public int priorityDefRef = 3;

        public bool setChangeMax = true;
        public bool setChangeDef = true;

    }
}
