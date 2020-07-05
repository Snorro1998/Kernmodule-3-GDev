namespace RoboCodeProject
{
    /// <summary>
    /// Zoek naar een doelwit en rij erachteraan
    /// </summary>
    class TrackRobot : BTNode
    {
        public TrackRobot(BlackBoard _blackboard)
        {
            blackBoard = _blackboard;
        }

        public override BTNodeStatus Tick()
        {
            blackBoard.robot.IsAdjustGunForRobotTurn = true;
            blackBoard.robot.TurnGunRight(blackBoard.gunTurnAmt);
            blackBoard.count++;

            if (blackBoard.count > 2)
            {
                blackBoard.gunTurnAmt = -10;
            }

            // Kijk de andere kant op als hij niets heeft kunnen vinden
            if (blackBoard.count > 5)
            {
                blackBoard.gunTurnAmt = 10;
            }

            // Geef het op als het nog niet gelukt is
            if (blackBoard.count > 11)
            {
                blackBoard.trackName = null;
            }

            return BTNodeStatus.succes;
        }
    }
}
