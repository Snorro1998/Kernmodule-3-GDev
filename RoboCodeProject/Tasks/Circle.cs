using Robocode;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RoboCodeProject
{
    class CircleMove : BTNode
    {
        double speed;
        int circleRadius;
        double fieldWidth;
        double fieldHeight;
        Vector2 worldCenter;
        Random rnd = new Random();

        double distance = 0;

        public CircleMove(BlackBoard _blackBoard, double _speed, int _circleRadius)
        {
            blackBoard = _blackBoard;
            speed = _speed;
            circleRadius = _circleRadius;
            fieldWidth = blackBoard.robot.BattleFieldWidth;
            fieldHeight = blackBoard.robot.BattleFieldHeight;
            worldCenter = new Vector2((float)fieldWidth / 2, (float)fieldHeight / 2);
        }

        private void TurnToRotation(double endAngle)
        {
            blackBoard.robot.TurnLeft(CalcTurningAngle(endAngle));
        }

        private double CalcTurningAngle(double endAngle)
        {
            endAngle = endAngle % 360;
            double currentAngle = (360 - (blackBoard.robot.Heading - 90)) % 360;

            double turnAngle = endAngle - currentAngle;

            if (turnAngle < -180)
            {
                turnAngle += 360;
            }

            //turnleft
            return turnAngle;
        }

        public double DegToRad(double a)
        {
            return (a * Math.PI) / 180;
        }

        public double RadToDeg(double a)
        {
            return (a * 180) / Math.PI;
        }

        public void LookAtPosition(Vector2 dest)
        {
            Vector2 currentPos = new Vector2((float)blackBoard.robot.X, (float)blackBoard.robot.Y);

            double dX = currentPos.X - dest.X;
            double dY = currentPos.Y - dest.Y;
            double angle = RadToDeg(Math.Atan2(dY, dX));
            distance = Math.Sqrt(dX * dX + dY * dY);

            TurnToRotation(angle + 180);
        }

        public void MoveToPosition(Vector2 dest)
        {
            Vector2 currentPos = new Vector2((float)blackBoard.robot.X, (float)blackBoard.robot.Y);

            double dX = currentPos.X - dest.X;
            double dY = currentPos.Y - dest.Y;
            double angle = RadToDeg(Math.Atan2(dY, dX));
            distance = Math.Sqrt(dX * dX + dY * dY);

            TurnToRotation(angle + 180);
            blackBoard.robot.Ahead(distance);
        }

        public void MoveToEdgeOfCircle()
        {
            Vector2 currentPos = new Vector2((float)blackBoard.robot.X, (float)blackBoard.robot.Y);
            Vector2 dest = CalcRandomCirclePos();

            double dX = currentPos.X - dest.X;
            double dY = currentPos.Y - dest.Y;
            double angle = RadToDeg(Math.Atan2(dY, dX));
            distance = Math.Sqrt(dX * dX + dY * dY);

            double fac = circleRadius / distance;
            dest = new Vector2((float)(dest.X + dX * fac), (float)(dest.Y + dY * fac));

            TurnToRotation(angle + 180);
            blackBoard.robot.Ahead(distance * fac);
            blackBoard.robot.TurnRight(90);

            blackBoard.robot.MaxVelocity = speed;
            blackBoard.robot.SetAhead(Math.PI * 2 * circleRadius);
            blackBoard.robot.SetTurnLeft(360);
            blackBoard.robot.WaitFor(new TurnCompleteCondition(blackBoard.robot));
            blackBoard.robot.Stop();
            blackBoard.robot.MaxVelocity = Rules.MAX_VELOCITY;
        }

        public Vector2 CalcRandomCirclePos()
        {    
            double rX = rnd.Next((int)blackBoard.robot.Width + circleRadius, (int)fieldWidth - (int)blackBoard.robot.Width - circleRadius);
            double rY = rnd.Next((int)blackBoard.robot.Height + circleRadius, (int)fieldHeight - (int)blackBoard.robot.Height - circleRadius);
            return new Vector2((float)rX, (float)rY);
        }

        public override BTNodeStatus Tick()
        {
            MoveToEdgeOfCircle(); 
            return BTNodeStatus.succes;
        }
    }
}
