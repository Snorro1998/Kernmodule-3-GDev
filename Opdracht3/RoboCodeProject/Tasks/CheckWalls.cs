using Robocode;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RoboCodeProject
{
    class Checkwalls : BTNode
    {
        int offset;
        public Checkwalls(BlackBoard blackBoard, int offset)
        {
            this.blackBoard = blackBoard;
            this.offset = offset;

        }

        public void CheckwallsFunc(int battleX, int BattleY)
        {
         //   if(blackBoard.robot.X > 0 + offset || blackBoard.robot.X < battleX - offset ||)
        }


        public override BTNodeStatus Tick()
        {
            blackBoard.robot.SetTurnRight(blackBoard.lastScannedRobotEvent.Bearing);
            blackBoard.robot.SetAhead(1000 * blackBoard.moveDirection);
            blackBoard.robot.FireBullet(150);
            blackBoard.robot.FireBullet(150);
            blackBoard.robot.FireBullet(150);
            return BTNodeStatus.succes;
        }


    }
}
