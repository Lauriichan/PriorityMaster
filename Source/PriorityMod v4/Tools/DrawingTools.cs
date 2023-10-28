using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PriorityMod.Core;
using PriorityMod.Extensions;
using System;

namespace PriorityMod.Tools
{
    public static class DrawingTools
    {

        private static List<Color> colors = new List<Color>();

        public static Color GetColorFromPriority(int priority)
        {
            return colors.ElementAtOrDefault(priority - 1);
        }

        public static void UpdateColors()
        {
            colors.Clear();
            List<String> hexColors = PriorityMaster.settings.hexColors;
            if (!PriorityMaster.settings.enableGradient)
            {
                hexColors.ToColor(ref colors);
                return;
            }
            Color col1 = hexColors[0].ToColor();
            Color col2 = hexColors[1].ToColor();
            int count = PriorityMaster.settings.GetMaxPriority();
            if (PriorityMaster.settings.reverseGradient)
            {
                for (float index = 0; index < count; index++)
                {
                    colors.Add((index / count).Interpolate(col1, col2));
                }
                return;
            }
            for (float index = count; index > 0; index--)
            {
                colors.Add((index / count).Interpolate(col1, col2));
            }
        }

    }
}
