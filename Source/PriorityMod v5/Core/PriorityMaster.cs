﻿using HarmonyLib;
using PriorityMod.PatchesV5;
using PriorityMod.PatchesV5.Compat;
using PriorityMod.Tools;
using PriorityMod.Settings;
using Verse;
using UnityEngine;
using static Verse.ParseHelper;

namespace PriorityMod.Core
{

    public class PriorityMaster : Mod
    {

        public static Harmony harmony;

        public static PrioritySettings settings;

        private static bool startup = false;

        public PriorityMaster(ModContentPack content) : base(content)
        {
            if (startup)
            {
                return;
            }
            startup = true;

            Parsers<Tools.SimpleColor>.Register(Tools.SimpleColor.sRGB);

            settings = GetSettings<PrioritySettings>();

            DrawingTools.UpdateColors();
                
            harmony = new Harmony("Lauriichen.PriorityMod");

            RestartPatches.Apply(harmony);

            CentricWorkPatches.Apply(harmony);
            FluffyWorkTabPatches.Apply(harmony);
            PlugAndPlayJoinerPatches.Apply(harmony);

            UIPatches.Apply(harmony);
            PriorityPatches.Apply(harmony);
        }

        private readonly ColorPicker picker = new ColorPicker(120, 20, 10, 24, 12, 80);

        public override void DoSettingsWindowContents(Rect inRect)
        {

            Listing_PriorityMod listing = new Listing_PriorityMod();
            listing.Begin(inRect);

            settings.SelectColor(listing.ColorList(settings.GetNewSelect(), settings.GetSelectMax(), settings.GetUnityColor));
            listing.Gap(24);
            picker.Draw(listing as Listing, in settings.buffer);
            listing.GapLine(36);

            Rect settingsRect = listing.GetRect(inRect.height - listing.CurHeight - 12);
            Listing_PriorityMod settingsListing = new Listing_PriorityMod();
            settingsListing.ColumnWidth = (settingsRect.width - Listing.ColumnSpacing) / 2f;
            settingsListing.Begin(settingsRect);

            settingsListing.LabeledNumericSliderInt("highestPriority".Translate() + "   ", ref settings.buffer.priorityMaxRef, ref settings.buffer.maxPriority, PrioritySettings.GLOBAL_MIN_PRIORITY, PrioritySettings.GLOBAL_MAX_PRIORITY);

            settingsListing.Gap(24);

            settingsListing.CheckboxLabeled("enableGradient".Translate(), ref settings.enableGradient, "gradientTooltip".Translate());

            settingsListing.NewColumn();

            settingsListing.LabeledNumericSliderInt("defaultPriority".Translate() + "   ", ref settings.buffer.priorityDefRef, ref settings.buffer.defPriority, PrioritySettings.GLOBAL_MIN_PRIORITY, settings.GetMaxPriority());

            settingsListing.Gap(24);

            settingsListing.CheckboxLabeled("reverseGradient".Translate(), ref settings.reverseGradient, "reverseGradientTooltip".Translate());

            settingsListing.End();

            listing.GapLine();
            listing.End();

            base.DoSettingsWindowContents(inRect);

            settings.StoreColors();
        }

        public override string SettingsCategory()
        {
            return "PriorityMaster".Translate();
        }

    }
}