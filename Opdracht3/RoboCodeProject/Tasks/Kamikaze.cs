using Robocode;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RoboCodeProject
{
    class Kamikaze : BTNode
    {
        public int speed;
        public int damage;
        public Kamikaze(BlackBoard blackBoard, int speed, int damage)
        {
            this.blackBoard = blackBoard;
            this.speed = speed;
            this.damage = damage;

        }

        public override BTNodeStatus Tick()
        {
            blackBoard.robot.SetTurnRight(blackBoard.lastScannedRobotEvent.Bearing);
            blackBoard.robot.SetAhead(speed * blackBoard.moveDirection);
            blackBoard.robot.FireBullet(damage);
            blackBoard.robot.FireBullet(damage);
            blackBoard.robot.FireBullet(damage);
            return BTNodeStatus.succes;
        }


    }
}
