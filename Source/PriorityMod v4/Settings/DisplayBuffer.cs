using System;
using System.Linq;
using PriorityMod.Extensions;
using PriorityMod.Tools;
using UnityEngine;
using Verse;

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

        public Color Color
        {
            get => new Color(_redF, _greenF, _blueF);
            set
            {
                value.ToRGB(out int red, out int green, out int blue);
                if(_red == red && _green == green && _blue == blue)
                {
                    return;
                }
                _red = red;
                _green = green;
                _blue = blue;
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
                _redF = _red.RGBToFloat();
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
                _greenF = _green.RGBToFloat();
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
                _blueF = _blue.RGBToFloat();
                UpdateFromRGB();
            }
        }

        public float RedF
        {
            get => _redF;
            set => Red = value.RGBToInt();
        }

        public float GreenF
        {
            get => _greenF;
            set => Green = value.RGBToInt();
        }

        public float BlueF
        {
            get => _blueF;
            set => Blue = value.RGBToInt();
        }
        public float Hue
        {
            get => _hue;
            set {
                CheckHSLRange(ref value);
                if(_hue == value)
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
                String tmp = value.Substring(1);
                switch (tmp.Length)
                {
                    case 1:
                        _hex = "#" + tmp + tmp + tmp + tmp + tmp + tmp;
                        break;
                    case 3:
                        char[] arr = tmp.ToCharArray();
                        _hex = "#" + arr[0] + arr[0] + arr[1] + arr[1] + arr[2] + arr[2];
                        break;
                    case 6:
                        _hex = "#" + tmp;
                        break;
                    default:
                        return;
                }
                tmp = _hex.Substring(1);
                _red = tmp.Substring(0, 2).HexToInt();
                _green = tmp.Substring(2, 2).HexToInt();
                _blue = tmp.Substring(4, 2).HexToInt();
                _redF = _red.RGBToFloat();
                _greenF = _green.RGBToFloat();
                _blueF = _blue.RGBToFloat();
                UpdateFromRGB(false);
            }
        }

        private void UpdateFromHSL()
        {
            Utils.HSLToRGB(_hue, _saturation, _lightness, out _red, out _green, out _blue);
            _redF = _red.RGBToFloat();
            _greenF = _green.RGBToFloat();
            _blueF = _blue.RGBToFloat();
            UpdateHex();
        }

        private void UpdateFromRGB(bool updateHex = true)
        {
            Utils.RGBToHSL(_red, _green, _blue, out _hue, out _saturation, out _lightness);
            if(updateHex)
            {
                UpdateHex();
            }
        }

        private void UpdateHex()
        {
            _hex = '#' + _red.ToString("X2") + _green.ToString("X2") + _blue.ToString("X2");
            settings.SetColor(selected, _hex);
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
            if(value > 255)
                value = 255;
            else if(value < 0)
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
