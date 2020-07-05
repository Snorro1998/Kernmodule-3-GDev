using Robocode;

namespace RoboCodeProject
{
    class MoveAhead : BTNode
    {
        private int moveDistance;
        private int moveDirection = 1;
        public MoveAhead(BlackBoard blackBoard, int movePixels)
        {
            this.blackBoard = blackBoard;
            moveDistance = movePixels;
        }
      
        public void OnHitWall(HitWallEvent e)
        {
            moveDirection *= -1;
        }

        public void OnHitRobot(HitRobotEvent e)
        {
            moveDirection *= -1;
            
        }

        public override BTNodeStatus Tick()
        {
            blackBoard.robot.Ahead(moveDistance * blackBoard.moveDirection);

            return BTNodeStatus.succes;
        }
    }
}
