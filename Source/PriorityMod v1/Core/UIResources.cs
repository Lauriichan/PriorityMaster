using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace PriorityMod.Core
{
    [Verse.StaticConstructorOnStartup]
    public static class UIResources
    {

        public static readonly Texture2D SelectionCircle = ContentFinder<Texture2D>.Get("UI/SelectionCircle", true);

    }
}
