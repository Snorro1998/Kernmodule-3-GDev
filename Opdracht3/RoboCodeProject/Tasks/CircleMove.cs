using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboCodeProject
{
    /// <summary>
    /// Rij rond in een cirkelpatroon
    /// </summary>
    class CircleMove : BTNode
    {
        public CircleMove(BlackBoard _blackboard)
        {
            blackBoard = _blackboard;
        }

        public override BTNodeStatus Tick()
        {
            blackBoard.robot.IsAdjustGunForRobotTurn = false;
            blackBoard.robot.SetTurnRight(10000);
            //MaxVelocity = 5;
            blackBoard.robot.Ahead(10000);

            return BTNodeStatus.succes;
        }
    }
}
