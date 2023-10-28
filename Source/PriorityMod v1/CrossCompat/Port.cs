using Harmony;
using System;
using System.Reflection;
using UnityEngine;
using Verse;

namespace PriorityMod.CrossCompat
{
    public static class Port
    {
        private static readonly bool Linux = SystemInfo.operatingSystemFamily == OperatingSystemFamily.Linux;

        public static bool UnityGUIBugsFixer_MouseDrag(int button)
        {
            if (!Linux)
            {
                return Event.current.type == EventType.MouseDrag && Event.current.button == button;
            }
            return Input.GetMouseButton(button);
        }

        public static FieldInfo AccessTools_DeclaredField(Type type, string name)
        {
            foreach (FieldInfo info in AccessTools.GetDeclaredFields(type))
            {
                if (info.Name.Equals(name))
                {
                    return info;
                }
            }
            return null;
        }

        public static void Widgets_DrawBoxSolidWithOutline(Rect rect, Color solidColor, Color outlineColor, int outlineThickness = 1)
        {
            Widgets.DrawBoxSolid(rect, solidColor);
            Color color = GUI.color;
            GUI.color = outlineColor;
            Widgets.DrawBox(rect, outlineThickness);
            GUI.color = color;
        }
    }
}
