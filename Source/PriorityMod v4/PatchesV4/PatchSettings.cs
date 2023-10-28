using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriorityMod.PatchesV4
{
    internal class PatchSettings
    {
        public static bool Patch_GetPriority 
        {
            get
            {
                return patch_GetPriority;
            }
        }
        public static bool Patch_EnableAndInitialize
        {
            get
            {
                return patch_EnableAndInitialize;
            }
        }

        public static void Disable_GetPriority_Patch()
        {
            patch_GetPriority = false;
        }

        public static void Disable_EnableAndInitialize_Patch()
        {
            patch_EnableAndInitialize = false;
        }


        private static bool patch_GetPriority = true;
        private static bool patch_EnableAndInitialize = true;

    }
}
