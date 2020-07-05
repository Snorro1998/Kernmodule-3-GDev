using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Robocode;

namespace RoboCodeProject
{
    /// <summary>
    /// Rij rond in een cirkelpatroon
    /// </summary>
    class DrunkMove : BTNode
    {
        public DrunkMove(BlackBoard _blackboard)
        {
            blackBoard = _blackboard;
        }

        public override BTNodeStatus Tick()
        {
            blackBoard.robot.IsAdjustGunForRobotTurn = false;
            // Rij voor lange tijd vooruit
            blackBoard.robot.SetAhead(40000);
            blackBoard.movingForward = true;
            blackBoard.robot.SetTurnRight(90);
            // Wacht tot hij klaar is met draaien
            blackBoard.robot.WaitFor(new TurnCompleteCondition(blackBoard.robot));
            // Draai de andere kant op
            blackBoard.robot.SetTurnLeft(180);
            // Wacht weer tot hij klaar is met draaien
            blackBoard.robot.WaitFor(new TurnCompleteCondition(blackBoard.robot));
            blackBoard.robot.SetTurnRight(180);
            blackBoard.robot.WaitFor(new TurnCompleteCondition(blackBoard.robot));

            return BTNodeStatus.succes;
        }
    }
}
