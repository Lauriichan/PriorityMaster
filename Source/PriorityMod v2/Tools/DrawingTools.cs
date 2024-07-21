using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PriorityMod.Core;
using PriorityMod.Extensions;
using System;
using System.Runtime.CompilerServices;

namespace PriorityMod.Tools
{
    public static class DrawingTools
    {

        private static List<Color> calculatedColors = new List<Color>();

        public static Color GetColorFromPriority(int priority)
        {
            return calculatedColors.ElementAtOrDefault(priority - 1);
        }

        public static void UpdateColors()
        {
            calculatedColors.Clear();
            List<SimpleColor> colors = PriorityMaster.settings.colors;
            if (!PriorityMaster.settings.enableGradient)
            {
                foreach (SimpleColor color in colors)
                {
                    calculatedColors.Add(color.ToUnity());
                }
                return;
            }
            SimpleColor start = colors[0].ToOkLab();
            SimpleColor end = colors[1].ToOkLab();
            int count = PriorityMaster.settings.GetMaxPriority();
            if (PriorityMaster.settings.reverseGradient)
            {
                for (float index = 0; index < count; index++)
                {
                    calculatedColors.Add(Interpolate((index / count), start, end));
                }
                return;
            }
            for (float index = count; index > 0; index--)
            {
                calculatedColors.Add(Interpolate((index / count), start, end));
            }
        }

        private static Color Interpolate(float ratio, SimpleColor start, SimpleColor end)
        {
            return start.Copy().Multiply(1f - ratio).Add(end.Copy().Multiply(ratio)).ToUnity();
        }

    }

    
}
