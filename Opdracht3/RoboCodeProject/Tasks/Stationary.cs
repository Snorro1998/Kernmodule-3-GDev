using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboCodeProject
{
    /// <summary>
    /// Staat stil en draait om zich heen
    /// </summary>
    class Stationary : BTNode
    {
        public Stationary(BlackBoard _blackboard)
        {
            blackBoard = _blackboard;
        }

        public override BTNodeStatus Tick()
        {
            blackBoard.robot.TurnGunRight(8);
            return BTNodeStatus.succes;
        }
    }
}
