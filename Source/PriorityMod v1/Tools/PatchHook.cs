using PriorityMod.Core;

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
