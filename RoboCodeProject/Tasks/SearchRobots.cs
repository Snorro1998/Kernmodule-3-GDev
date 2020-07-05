using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboCodeProject
{
    public class SearchRobots : BTNode
    {
        private float scanDegrees;

        public SearchRobots(BlackBoard blackBoard, float scanDegrees)
        {
            this.blackBoard = blackBoard;
            this.scanDegrees = scanDegrees;
        }
        public override BTNodeStatus Tick()
        {
            blackBoard.robot.TurnRadarLeft(scanDegrees);
            return BTNodeStatus.succes;
        }
    }
}
