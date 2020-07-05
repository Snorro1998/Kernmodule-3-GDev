using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboCodeProject
{
    public enum BTNodeStatus { failed, running, succes }

    public abstract class BTNode
    {
        protected BlackBoard blackBoard;
        public abstract BTNodeStatus Tick();
    }
}
