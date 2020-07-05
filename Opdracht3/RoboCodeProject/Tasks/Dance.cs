using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace RoboCodeProject
{
    /// <summary>
    /// Danst na een overwinning
    /// </summary>
    class Dance : BTNode
    {
        public Dance(BlackBoard _blackboard)
        {
            blackBoard = _blackboard;
        }

        public void SetRandomColor(int r, int g, int b)
        {
            Color color = Color.FromArgb(1, r, g, b);
            blackBoard.robot.SetColors(color, color, color);
        }

        public override BTNodeStatus Tick()
        {
            Random rand = new Random();

            for (int i = 0; i < 40; i++)
            {
                SetRandomColor(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
                blackBoard.robot.TurnRight(rand.Next(-90, 90));
                blackBoard.robot.TurnLeft(rand.Next(-90, 90));
            }

            return BTNodeStatus.succes;
        }
    }
}