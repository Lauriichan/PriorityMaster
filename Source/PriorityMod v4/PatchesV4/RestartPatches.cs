using HarmonyLib;
using PriorityMod.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace PriorityMod.PatchesV4
{
    public static class RestartPatches
    {

        public static void Apply(Harmony harmony)
        {
            harmony.Patch(Reflection.Method("MainMenuDrawer", "DoMainMenuControls"), prefix: PatchHelper.Method<Rect>((rect) => RestartPatches.MainControlsPrefix(ref rect)), postfix: PatchHelper.Method<Rect>((rect) => RestartPatches.MainControlsPostfix(ref rect)));
            harmony.Patch(Reflection.Method("OptionListingUtility", "DrawOptionListing"), prefix: PatchHelper.Method<Rect, List<ListableOption>>((rect, list) => RestartPatches.OptionListingUtilPrefix(rect, list)));
        }

        private static bool drawing;

        private static void MainControlsPrefix(ref Rect rect)
        {
            if (Prefs.DevMode)
            {
                rect.height += 45f;
            }
            drawing = true;
        }

        private static void MainControlsPostfix(ref Rect rect)
        {
            drawing = false;
        }

        private static void OptionListingUtilPrefix(Rect rect, List<ListableOption> optList)
        {
            if (!drawing || !Prefs.DevMode) return;

            string quitToOs = "QuitToOS".Translate();
            int idx = optList.FindIndex(opt => opt.label == quitToOs);
            if (idx != -1)
            {
                optList.Insert(idx, new ListableOption("Restart", () =>
                {
                    GenCommandLine.Restart();
                }));
            }
        }
        
    }
}
