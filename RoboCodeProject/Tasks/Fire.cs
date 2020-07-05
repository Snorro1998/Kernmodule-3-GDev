using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboCodeProject
{
    class Fire : BTNode
    {
        int force;

        public Fire(BlackBoard _blackBoard, int _force)
        {
            blackBoard = _blackBoard;
            force = _force;
        }
        public override BTNodeStatus Tick()
        {
            blackBoard.robot.Fire(force);
            return BTNodeStatus.succes;
        }
    }
}
