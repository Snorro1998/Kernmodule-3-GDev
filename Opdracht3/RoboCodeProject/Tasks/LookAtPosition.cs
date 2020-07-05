using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace RoboCodeProject
{
    class LookAtPosition : BTNode
    {
#if false
        public ref decimal Vector2 dest();

        public LookAtPosition(BlackBoard _blackBoard, ref Vector2 _dest)
        {
            blackBoard = _blackBoard;
            dest = _dest;
        }

        public void LookAt(ref Vector2 dest)
        {
            Vector2 currentPos = new Vector2((float)blackBoard.robot.X, (float)blackBoard.robot.Y);
            //Vector2 dest = blackBoard.enemyPosition;

            double dX = currentPos.X - dest.X;
            double dY = currentPos.Y - dest.Y;
            //double angle = RadToDeg(Math.Atan2(dY, dX));
            //distance = Math.Sqrt(dX * dX + dY * dY);

            //TurnToRotation(angle + 180);
        }

        public override BTNodeStatus Tick()
        {
            //blackBoard.robot.TurnRight(blackBoard.lastScannedRobotEvent.Bearing);
            //blackBoard.robot.Ahead(blackBoard.lastScannedRobotEvent.Distance + 5);
            return BTNodeStatus.succes;
        }
#endif
        public override BTNodeStatus Tick()
        {
            return BTNodeStatus.succes;
        }
    }

}
