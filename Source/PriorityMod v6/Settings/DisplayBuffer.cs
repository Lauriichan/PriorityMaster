using System;
using UnityEngine;
using PriorityMod.Tools;
using UnityEngine.Diagnostics;
using RimWorld;

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

        private int _red = 0;
        private int _green = 0;
        private int _blue = 0;

        private float _redF = 0;
        private float _greenF = 0;
        private float _blueF = 0;

        private float _hue;
        private float _saturation;
        private float _lightness;

        public SimpleColor Color
        {
            get => SimpleColor.sRGB(_redF, _greenF, _blueF);
            set
            {
                SimpleColor col = value.ToSRGB();
                _red = col.RedI;
                _green = col.GreenI;
                _blue = col.BlueI;
                _redF = col.Red;
                _greenF = col.Green;
                _blueF = col.Blue;
                UpdateFromRGB(true, false);
            }
        }

        public Color UnityColor
        {
            get => new Color(_redF, _greenF, _blueF);
            set
            {
                SimpleColor col = SimpleColor.sRGB(value);
                if (_red == col.RedI && _green == col.GreenI && _blue == col.BlueI)
                {
                    return;
                }
                _red = col.RedI;
                _green = col.GreenI;
                _blue = col.BlueI;
                UpdateFromRGB();
            }
        }

        public int Red
        {
            get => _red;
            set
            {
                CheckRGBRange(ref value);
                if (_red == value)
                    return;
                _red = value;
                _redF = SimpleColor.Int2RGB(_red);
                UpdateFromRGB();
            }
        }

        public int Green
        {
            get => _green;
            set
            {
                CheckRGBRange(ref value);
                if (_green == value)
                    return;
                _green = value;
                _greenF = SimpleColor.Int2RGB(_green);
                UpdateFromRGB();
            }
        }

        public int Blue
        {
            get => _blue;
            set
            {
                CheckRGBRange(ref value);
                if (_blue == value)
                    return;
                _blue = value;
                _blueF = SimpleColor.Int2RGB(_blue);
                UpdateFromRGB();
            }
        }

        public float RedF
        {
            get => _redF;
            set => Red = SimpleColor.RGB2Int(value);
        }

        public float GreenF
        {
            get => _greenF;
            set => Green = SimpleColor.RGB2Int(value);
        }

        public float BlueF
        {
            get => _blueF;
            set => Blue = SimpleColor.RGB2Int(value);
        }
        public float Hue
        {
            get => _hue;
            set
            {
                CheckHSLRange(ref value);
                if (_hue == value)
                    return;
                _hue = value;
                UpdateFromHSL();
            }
        }

        public float Saturation
        {
            get => _saturation;
            set
            {
                CheckHSLRange(ref value);
                if (_saturation == value)
                    return;
                _saturation = value;
                UpdateFromHSL();
            }
        }

        public float Lightness
        {
            get => _lightness;
            set
            {
                CheckHSLRange(ref value);
                if (_lightness == value)
                    return;
                _lightness = value;
                UpdateFromHSL();
            }
        }

        private String _hex = "#000000";
        public String Hex
        {
            get => _hex;
            set
            {
                if (_hex.Equals(value) || !value.StartsWith("#"))
                {
                    return;
                }
                SimpleColor col = SimpleColor.sRGB(value);
                _hex = col.ToString();
                _red = col.RedI;
                _green = col.GreenI;
                _blue = col.BlueI;
                _redF = col.Red;
                _greenF = col.Green;
                _blueF = col.Blue;
                UpdateFromRGB(false);
            }
        }

        private void UpdateFromHSL()
        {
            SimpleColor col = SimpleColor.HSL(_hue, _saturation, _lightness).ToSRGB();
            _red = col.RedI;
            _green = col.GreenI;
            _blue = col.BlueI;
            _redF = col.Red;
            _greenF = col.Green;
            _blueF = col.Blue;
            UpdateHex(true);
        }

        private void UpdateFromRGB(bool updateHex = true, bool saveHex = true)
        {
            SimpleColor col = SimpleColor.sRGB(_redF, _greenF, _blueF).ToHSL();
            _hue = col.Hue;
            _saturation = col.Saturation;
            _lightness = col.Lightness;
            if (updateHex)
            {
                UpdateHex(saveHex);
            }
        }

        private void UpdateHex(bool saveHex)
        {
            SimpleColor color = SimpleColor.sRGB(_redF, _greenF, _blueF);
            _hex = color.ToString();
            if (saveHex)
            {
                settings.SetColor(selected, color);
            }
        }

        private void CheckHSLRange(ref float value)
        {
            if (value > 1.0f)
                value = 1.0f;
            else if (value < 0f)
                value = 0f;
        }

        private void CheckRGBRange(ref int value)
        {
            if (value > 255)
                value = 255;
            else if (value < 0)
                value = 0;
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
