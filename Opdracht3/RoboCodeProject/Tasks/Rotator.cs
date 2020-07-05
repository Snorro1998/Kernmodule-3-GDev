using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Robocode;

namespace RoboCodeProject
{
    class Rotator : BTNode
    {
        public enum Rotation
        {
            forward,
            left,
            right,
            backward,
            none
        }

        public double TargetAngle()
        {
            if (blackBoard.lastScannedRobotEvent == null)
            {
                return 0;
            }

            double _angle = blackBoard.lastScannedRobotEvent.Bearing - (blackBoard.robot.GunHeading - blackBoard.robot.Heading);

            return _angle;
        }

        public override BTNodeStatus Tick()
        {
            blackBoard.robot.TurnLeft(TargetAngle());

            return BTNodeStatus.succes;
        }
    }
}
