namespace RoboCodeProject
{
    /// <summary>
    /// Keert rijrichting om
    /// </summary>
    class DriveReverse : BTNode
    {
        public DriveReverse(BlackBoard _blackboard)
        {
            blackBoard = _blackboard;
        }

        public override BTNodeStatus Tick()
        {
            blackBoard.robot.SetBack(blackBoard.movingForward == true ? 40000 : -40000);
            blackBoard.movingForward = !blackBoard.movingForward;
            return BTNodeStatus.succes;
        }
    }
}
