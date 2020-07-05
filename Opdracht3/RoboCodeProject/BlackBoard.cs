using Robocode;
using System.Collections.Generic;
using System.Numerics;

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboCodeProject
{
    public class BlackBoard
    {
        public AdvancedRobot robot;
        public int count = 0;
        public double gunTurnAmt = 0;
        public string trackName;
        public bool movingForward;

        public BlackBoard(AdvancedRobot _robot)
        {
            robot = _robot;
        }
        /*
        public int moveDirection = 1;
        public int crazyMode;
        public AdvancedRobot robot;
        public ScannedRobotEvent lastScannedRobotEvent;
        public Vector2 enemyPosition = -Vector2.One;
        public double distToCurrentEnemy = 0;
        public double realCurrentAngle = 0;

        public List<ScannedRobotEvent> allTargets = new List<ScannedRobotEvent>();
        public ScannedRobotEvent currentTarget;*/
    }
}