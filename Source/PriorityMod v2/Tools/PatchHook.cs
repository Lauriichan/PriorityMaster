using PriorityMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriorityMod.Tools
{
    public static class PatchHook
    {

        public static int GetDefaultPriority()
        {
            return PriorityMaster.settings.GetDefPriority();
        }

        public static int GetMaximumPriority()
        {
            return PriorityMaster.settings.GetMaxPriority();
        }

    }
}
