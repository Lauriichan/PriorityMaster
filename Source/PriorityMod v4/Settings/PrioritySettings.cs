using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using PriorityMod.Tools;
using UnityEngine;
using PriorityMod.Extensions;

namespace PriorityMod.Settings
{
    public class PrioritySettings : ModSettings
    {

        private static readonly string HEX_BLACK = "#000000";

        public static readonly int GLOBAL_MAX_PRIORITY = 99;
        public static readonly int GLOBAL_MIN_PRIORITY = 2;

        public readonly DisplayBuffer buffer;
        private System.Random random = new System.Random();

        public PrioritySettings()
        {
            buffer = new DisplayBuffer(this);
        }

        public bool enableGradient = true;
        public bool reverseGradient = false;

        private int maxPriority = 9;
        private int defPriority = 3;

        public List<String> hexColors = new List<String>(new String[] { "#00FF00", "#FF0000" });

        private List<Color> unityColors = new List<Color>();
        private bool colorsChanged = true;

        public override void ExposeData()
        {

            Scribe_Values.Look(ref maxPriority, "HighestPriority");
            Scribe_Values.Look(ref defPriority, "DefaultPriority");
            Scribe_Values.Look(ref enableGradient, "Gradient");
            Scribe_Values.Look(ref reverseGradient, "ReverseGradient");
            Scribe_Collections.Look(ref hexColors, "HexColors", LookMode.Value);

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

        public Color GetColor(int color)
        {
            if (color == buffer.selected)
            {
                return buffer.Color;
            }
            if (colorsChanged || color >= unityColors.Count)
            {
                UpdateColorList();
                unityColors.Clear();
                hexColors.ToColor(ref unityColors);
                colorsChanged = false;
            }
            if(color >= unityColors.Count)
            {
                return Color.black;
            }
            return unityColors[color];
        }

        public void StoreColors()
        {
            UpdateColorList();
            DrawingTools.UpdateColors();
        }

        private void UpdateColorList()
        {
            int size = GetSelectMax();
            if (hexColors.Count > size)
            {
                for (int index = hexColors.Count - 1; index >= size; index--)
                    hexColors.RemoveAt(index);
            }
            else if (hexColors.Count < size)
            {
                while (hexColors.Count < size)
                {
                    hexColors.Add('#' + random.Next(0, 256).IntToHex() + random.Next(0, 256).IntToHex() + random.Next(0, 256).IntToHex());
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
            else if (priority < GLOBAL_MIN_PRIORITY)
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

            if (hexColors.Count >= color)
            {
                buffer.Hex = hexColors.ElementAt(color);
                buffer.selected = color;
                return;
            }
            buffer.Hex = HEX_BLACK;
            buffer.selected = color;
        }

        public void SetColor(int color, string value)
        {
            if (color == -1)
            {
                return;
            }
            colorsChanged = true;
            UpdateColorList();
            hexColors.RemoveAt(color);
            hexColors.Insert(color, value);
        }

    }
}
