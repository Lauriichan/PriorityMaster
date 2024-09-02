using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriorityMod.PatchesV4.Multiplayer
{
    public interface IMPWatcher
    {
        bool CanChangeSettings();
        bool ShouldWatch();
        void BeginWatch();
        void EndWatch();
        ISyncRef Get(string id);

    }

    public interface ISyncRef
    {
        void Watch();
    }

    internal class NOPWatcher : IMPWatcher
    {
        public void BeginWatch()
        {
        }

        public bool CanChangeSettings()
        {
            return true;
        }

        public void EndWatch()
        {
        }

        public ISyncRef Get(string id)
        {
            return null;
        }

        public bool ShouldWatch()
        {
            return false;
        }
    }
}
