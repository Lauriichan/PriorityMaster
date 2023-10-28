using UnityEngine;
using Verse;

namespace PriorityMod.CrossCompat
{
    public static class Port
    {
        public static bool UnityGUIBugsFixer_MouseDrag(int button)
        {
            if (!UnityGUIBugsFixer.IsSteamDeckOrLinuxBuild)
            {
                return Event.current.type == EventType.MouseDrag && Event.current.button == button;
            }
            return Input.GetMouseButton(button);
        }
    }
}
