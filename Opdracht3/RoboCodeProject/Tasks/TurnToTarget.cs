using Robocode;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RoboCodeProject
{
    class TurnToTarget : BTNode
    {
        public TurnToTarget(BlackBoard blackBoard)
        {
            this.blackBoard = blackBoard;        
        }

        public override BTNodeStatus Tick()
        {
            blackBoard.robot.TurnRight(blackBoard.lastScannedRobotEvent.Bearing);
            blackBoard.robot.Ahead(blackBoard.lastScannedRobotEvent.Distance + 5);
            return BTNodeStatus.succes;
        }      
    }
}
