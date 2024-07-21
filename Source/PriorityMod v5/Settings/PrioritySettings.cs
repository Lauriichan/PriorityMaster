using System.Collections.Generic;
using System.Linq;
using v = Verse;
using PriorityMod.Tools;
using UnityEngine;
using static Verse.ParseHelper;

namespace PriorityMod.Settings
{
    public class PrioritySettings : v.ModSettings
    {

        private static readonly string HEX_BLACK = "#000000";

        public static readonly int GLOBAL_MAX_PRIORITY = 99;
        public static readonly int GLOBAL_MIN_PRIORITY = 2;

        public readonly DisplayBuffer buffer;
        private System.Random random = new System.Random();

        static PrioritySettings()
        {
            Parsers<SimpleColor>.Register(SimpleColor.sRGB);
        }
        public PrioritySettings()
        {
            buffer = new DisplayBuffer(this);
        }

        public bool drawRestartButton = false;

        public bool enableGradient = true;
        public bool reverseGradient = false;

        private int maxPriority = 9;
        private int defPriority = 3;

        public List<SimpleColor> colors = new List<SimpleColor>(new SimpleColor[] { SimpleColor.sRGB("#00FF00"), SimpleColor.sRGB("#FF0000") });

        private List<Color> unityColors = new List<Color>();
        private bool colorsChanged = true;

        public override void ExposeData()
        {

            v.Scribe_Values.Look(ref maxPriority, "HighestPriority");
            v.Scribe_Values.Look(ref defPriority, "DefaultPriority");
            v.Scribe_Values.Look(ref enableGradient, "Gradient");
            v.Scribe_Values.Look(ref reverseGradient, "ReverseGradient");
            v.Scribe_Values.Look(ref drawRestartButton, "DrawRestartButton");
            v.Scribe_Collections.Look(ref colors, "HexColors", v.LookMode.Value);

            base.ExposeData();
        }

        /*
         * 
         * 
         *
         */

        public void SetEnableGradient(bool gradient)
        {
            if(enableGradient == gradient)
            {
                return;
            }
            this.enableGradient = gradient;
            this.colorsChanged = true;
        }

        public Color GetUnityColor(int color)
        {
            if (color == buffer.selected)
            {
                return buffer.UnityColor;
            }
            if (colorsChanged || color >= unityColors.Count)
            {
                UpdateColorList();
                unityColors.Clear();
                foreach (SimpleColor colorObj in colors)
                {
                    unityColors.Add(colorObj.ToUnity());
                }
                colorsChanged = false;
            }
            if(color >= unityColors.Count)
            {
                return Color.black;
            }
            return unityColors[color];
        }

        public SimpleColor GetColor(int color)
        {
            if (color == buffer.selected)
            {
                return buffer.Color;
            }
            return colors[color];
        }

        public void StoreColors()
        {
            UpdateColorList();
            DrawingTools.UpdateColors();
        }

        private void UpdateColorList()
        {
            int size = GetSelectMax();
            if (colors.Count > size)
            {
                for (int index = colors.Count - 1; index >= size; index--)
                    colors.RemoveAt(index);
            }
            else if (colors.Count < size)
            {
                while (colors.Count < size)
                {
                    SimpleColor color = new SimpleColor(ColorType.SRGB);
                    color.RedI = random.Next(0, 256);
                    color.GreenI = random.Next(0, 256);
                    color.BlueI = random.Next(0, 256);
                    colors.Add(color);
                }
                    
            }
        }

        /*
         * 
         * 
         * 
         */

        public void SetMaxPriority(int priority)
        {
            if (maxPriority == priority)
                return;
            buffer.setChangeMax = true;
            if (priority > GLOBAL_MAX_PRIORITY)
                maxPriority = GLOBAL_MAX_PRIORITY;
            else if (priority < GLOBAL_MIN_PRIORITY)
                maxPriority = GLOBAL_MIN_PRIORITY;
            else
                maxPriority = priority;
        }

        public int GetMaxPriority()
        {
            if (maxPriority != buffer.priorityMaxRef)
            {
                if (buffer.setChangeMax)
                {
                    buffer.setChangeMax = false;
                    buffer.maxPriority = maxPriority + "";
                    return buffer.priorityMaxRef = maxPriority;
                }
                return maxPriority = buffer.priorityMaxRef;
            }
            return maxPriority;
        }

        public void SetDefPriority(int priority)
        {
            if (defPriority == priority)
                return;
            buffer.setChangeDef = true;
            if (priority > maxPriority)
                defPriority = maxPriority;
            else if (priority < 1)
                defPriority = 1;
            else
                defPriority = priority;
        }

        public int GetDefPriority()
        {
            if (defPriority != buffer.priorityDefRef)
            {
                if (buffer.setChangeDef)
                {
                    buffer.setChangeDef = false;
                    buffer.defPriority = defPriority + "";
                    return buffer.priorityDefRef = defPriority;
                }
                return defPriority = buffer.priorityDefRef;
            }
            return defPriority;
        }

        /*
         * 
         * 
         * 
         */

        public int GetNewSelect()
        {
            if(buffer.selected == -1)
                SelectColor(0);

            int max = GetSelectMax();
            if (buffer.selected >= max)
                SelectColor(max - 1);
            return buffer.selected;
        }

        public int GetSelectMax()
        {
            return UseGradient() ? 2 : maxPriority;
        }

        public bool UseGradient()
        {
            return enableGradient && maxPriority >= 3;
        }

        public void SelectColor(int color)
        {
            if (buffer.selected == color || color < 0)
                return;

            if (colors.Count >= color)
            {
                buffer.Color = colors.ElementAt(color);
                buffer.selected = color;
                return;
            }
            buffer.Hex = HEX_BLACK;
            buffer.selected = color;
        }

        public void SetColor(int color, SimpleColor value)
        {
            if (color == -1)
            {
                return;
            }
            colorsChanged = true;
            UpdateColorList();
            colors.RemoveAt(color);
            colors.Insert(color, value);
        }

    }
}
