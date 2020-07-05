using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Robocode;
using Robocode.RobotInterfaces;

namespace RoboCodeProject
{
    class UpdateColour : BTNode
    {
        private Color color;

        public UpdateColour(BlackBoard blackBoard, Color color)
        {
            this.blackBoard = blackBoard;
            this.color = color;
        }

        public override BTNodeStatus Tick()
        {
            blackBoard.robot.Out.WriteLine("UPDATECOLOR" + color);
            blackBoard.robot.SetAllColors(color);
            return BTNodeStatus.succes;
        }
    }
}
