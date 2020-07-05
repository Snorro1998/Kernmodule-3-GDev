using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboCodeProject
{
    class TurnGunTowardsScannedTank:BTNode
    {
        public TurnGunTowardsScannedTank(BlackBoard blackBoard)
        {
            this.blackBoard = blackBoard;
        }

        public double TargetAngle()
        {
           // if(blackBoard.lastScannedRobotEvent.Distance < blackBoard.lastScannedRobotEvent.Distance - 70)

            double _angle = blackBoard.robot.Heading - blackBoard.robot.GunHeading + blackBoard.lastScannedRobotEvent.Bearing;
            return _angle;
        }
        public override BTNodeStatus Tick()
        {
            blackBoard.robot.SetTurnGunRight(TargetAngle());
            return BTNodeStatus.succes;
        }
    }
}
