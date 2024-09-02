using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PriorityMod.Core;
using PriorityMod.Extensions;
using System;
using System.Runtime.CompilerServices;
using Verse;

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
            int count = PriorityMaster.settings.GetMaxPriority();
            if (!PriorityMaster.settings.enableGradient)
            {
                int listSize = colors.Count;
                for(int i = 0; i < Math.Min(listSize, count); i++)
                {
                    calculatedColors.Add(colors[i].ToUnity());
                }
                if (count <= listSize)
                {
                    return;
                }
                if (listSize == 1)
                {
                    for (int i = listSize; i < count; i++)
                    {
                        calculatedColors.Add(calculatedColors[0]);
                    }
                    return;
                }
                calculatedColors.RemoveLast();
                SimpleColor start1 = colors[listSize - 2];
                SimpleColor end1 = colors[listSize - 1];
                for (float index = listSize - 1; index < count; index++)
                {
                    calculatedColors.Add(Interpolate((index / (count - 1)), start1, end1));
                }
                return;
            }
            SimpleColor start = colors[0].ToOkLab();
            SimpleColor end = colors[1].ToOkLab();
            if (PriorityMaster.settings.reverseGradient)
            {
                for (float index = 0; index < count; index++)
                {
                    calculatedColors.Add(Interpolate((index / (count - 1)), start, end));
                }
                return;
            }
            for (float index = count; index > 0; index--)
            {
                calculatedColors.Add(Interpolate(((index - 1) / (count - 1)), start, end));
            }
        }

        private static Color Interpolate(float ratio, SimpleColor start, SimpleColor end)
        {
            return start.Copy().Multiply(ratio).Add(end.Copy().Multiply(1f - ratio)).ToUnity();
        }

    }

    
}
