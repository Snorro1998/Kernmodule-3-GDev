using System.Drawing;

namespace RoboCodeProject
{
    class ChangeColor : BTNode
    {
        Color color;

        public ChangeColor(BlackBoard _blackBoard, Color _color)
        {
            blackBoard = _blackBoard;
            color = _color;
        }

        public override BTNodeStatus Tick()
        {
            blackBoard.robot.SetColors(color, color, color);
            return BTNodeStatus.succes;
        }
    }
}
