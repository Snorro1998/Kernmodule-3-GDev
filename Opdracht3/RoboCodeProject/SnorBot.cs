using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;
using Robocode;
using Robocode.Util;


namespace RoboCodeProject
{
    public class SnorBot : AdvancedRobot
    {
        double max_e;

        int count = 0;
        double gunTurnAmt;
        String trackName;

        bool movingForward;
        bool track_stealthy;

        static readonly double thres_stealthy = 0.4; // inverse kans dat hij stealthy wordt. dus 1.0 - p = 0.6 in dit geval

        static readonly double thres_aggr = 0.66;
        static readonly double thres_normal = 0.33;

        void track()
        {
            IsAdjustGunForRobotTurn = true; // Keep the gun still when we turn
                                            // turn the Gun (looks for enemy)
            TurnGunRight(gunTurnAmt);
            // Keep track of how long we've been looking
            count++;
            // If we've haven't seen our target for 2 turns, look left
            if (count > 2)
            {
                gunTurnAmt = -10;
            }
            // If we still haven't seen our target for 5 turns, look right
            if (count > 5)
            {
                gunTurnAmt = 10;
            }
            // If we *still* haven't seen our target after 10 turns, find another target
            if (count > 11)
            {
                trackName = null;
            }
        }

        void crazy()
        {
            IsAdjustGunForRobotTurn = false;
            // Tell the game we will want to move Ahead 40000 -- some large number
            SetAhead(40000);
            movingForward = true;
            // Tell the game we will want to turn right 90
            SetTurnRight(90);
            // At this point, we have indicated to the game that *when we do something*,
            // we will want to move Ahead and turn right.  That's what "set" means.
            // It is important to realize we have not done anything yet!
            // In order to actually move, we'll want to call a method that
            // takes real time, such as WaitFor.
            // WaitFor actually starts the action -- we start moving and turning.
            // It will not return until we have finished turning.
            WaitFor(new TurnCompleteCondition(this));
            // Note:  We are still moving Ahead now, but the turn is complete.
            // Now we'll turn the other way...
            SetTurnLeft(180);
            // ... and wait for the turn to finish ...
            WaitFor(new TurnCompleteCondition(this));
            // ... then the other way ...
            SetTurnRight(180);
            // .. and wait for that turn to finish.
            WaitFor(new TurnCompleteCondition(this));
            // then back to the top to do it all again
        }

        void stealthy()
        {
            TurnGunRight(8);
        }

        void spin()
        {
            IsAdjustGunForRobotTurn = false;
            // Tell the game that when we take move,
            // we'll also want to turn right... a lot.
            SetTurnRight(10000);
            // Limit our speed to 5
            MaxVelocity = 5;
            // Start moving (and turning)
            Ahead(10000);
            // Repeat.
        }

        /**
         * run: SnorBot's default behavior
         */
        public override void Run()
        {
            // Initialization of the robot should be put here

            // After trying out your robot, try uncommenting the import at the top,
            // and the next line:

            // setColors(Color.red,Color.blue,Color.green); // body,gun,radar

            //agressief: Kies de dichtstbijzijnde tank en blijf deze achtervolgen. Probeer hem ook te rammen
            //normaal: kies dichtstbijzijnde tank en blijf deze van een redelijke afstand beschieten
            //voorzichtig: blijf bij de tanks vandaan. Blijf veel rondrijden en schiet af en toe
            max_e = Energy;

            // Prepare gun
            trackName = null; // Initialize to not tracking anyone
            IsAdjustGunForRobotTurn = true; // Keep the gun still when we turn
            gunTurnAmt = 10; // Initialize gunTurn to 10

            int behavior = -1, old_behavior;

            Random rand = new Random();

            // Robot main loop
            while (true)
            {
                double e = Energy;
                // het oude gedrag
                old_behavior = behavior;

                // pas schaalfactor aan voor het bepalen van het gedrag
                if (e > max_e)
                {
                    max_e = e;
                }
                else
                {
                    max_e *= 0.98;
                    if (max_e < 80)
                        max_e = 80;
                }

                // kies gedrag: aggresief tracker als >66%, crazy als >33% en anders spinbot
                if (e > max_e * thres_aggr)
                {
                    behavior = 0;
                }
                else if (e > max_e * thres_normal)
                {
                    behavior = 1;
                    track_stealthy = rand.NextDouble() >= thres_stealthy;
                }
                else
                {
                    behavior = 2;
                }
                Out.WriteLine("snuif: wacht op nieuw gedrag: max_e: " + max_e + ", e: " + e + ", b: " + behavior);

                // als we een nieuw gedrag hebben
                if (old_behavior != behavior)
                {
                    // stop oude stappen als we die nog in de wachtrij hebben
                    if (old_behavior != -1)
                    {
                        Out.WriteLine("snuif: stoppuh!");
                        Stop(true); // ja het moet dan twee keer
                        Stop(true);
                    }
                }

                // voer gedrag uit
                switch (behavior)
                {
                    case 0: track(); break;
                    case 1:
                        if (track_stealthy)
                            stealthy();
                        else
                            crazy();
                        break;
                    default: spin(); break;
                }
            }
        }

        void scan_stealthy(ScannedRobotEvent e)
        {
            // Calculate exact location of the robot
            double absoluteBearing = Heading + e.Bearing;
            double bearingFromGun = Utils.NormalRelativeAngleDegrees(absoluteBearing - GunHeading);

            // If it's close enough, Fire!
            if (Math.Abs(bearingFromGun) <= 3)
            {
                TurnGunRight(bearingFromGun);
                // We check gun heat here, because calling Fire()
                // uses a turn, which could cause us to lose track
                // of the other robot.
                if (GunHeat == 0)
                {
                    Fire(Math.Min(3 - Math.Abs(bearingFromGun), Energy - .1));
                }
            } // otherwise just set the gun to turn.
              // Note:  This will have no effect until we call scan()
            else
            {
                TurnGunRight(bearingFromGun);
            }
            // Generates another scan event if we see a robot.
            // We only need to call this if the gun (and therefore radar)
            // are not turning.  Otherwise, scan is called automatically.
            if (bearingFromGun == 0)
            {
                Scan();
            }
        }

        /**
         * onScannedRobot:  Here's the good stuff
         */
        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            // track ding
            if (Energy > max_e * thres_aggr)
                scanTrack(e);
            else if (track_stealthy)
                scan_stealthy(e);
            else // crazy of spin
                Fire(Energy > max_e * thres_normal ? 2 : 1);
        }

        public void scanTrack(ScannedRobotEvent e)
        {
            // If we have a target, and this isn't it, return immediately
            // so we can get more ScannedRobotEvents.
            Out.WriteLine("snuif: scan");

            if (trackName != null && !e.Name.Equals(trackName))
            {
                return;
            }
            // If we don't have a target, well, now we do!
            if (trackName == null)
            {
                trackName = e.Name;
                Out.WriteLine("Tracking " + trackName);
            }
            // This is our target.  Reset count (see the run method)
            count = 0;
            // If our target is too far away, turn and move toward it.
            if (e.Distance > 150)
            {
                gunTurnAmt = Utils.NormalRelativeAngleDegrees(e.Bearing + (Heading - RadarHeading));

                TurnGunRight(gunTurnAmt); // Try changing these to SetTurnGunRight,
                TurnRight(e.Bearing); // and see how much Tracker improves...
                                           // (you'll have to make Tracker an AdvancedRobot)
                Ahead(e.Distance - 140);
                return;
            }

            // Our target is close.
            gunTurnAmt = Utils.NormalRelativeAngleDegrees(e.Bearing + (Heading - RadarHeading));
            TurnGunRight(gunTurnAmt);
            Fire(3);

            // Our target is too close!  Back up.
            if (e.Distance < 100)
            {
                if (e.Bearing > -90 && e.Bearing <= 90)
                {
                    Back(40);
                }
                else
                {
                    Ahead(40);
                }
            }
            Scan();
        }

        /**
         * onHitRobot:  Set him as our new target
         */
        public override void OnHitRobot(HitRobotEvent e)
        {
            if (Energy <= max_e * thres_aggr)
            {
                if (e.Bearing > -10 && e.Bearing < 10)
                {
                    Fire(Energy > max_e * 0.33 ? 2 : 1);
                }
                if (e.IsMyFault)
                {
                    TurnRight(10);
                }
                return;
            }
            // Only print if he's not already our target.
            if (trackName != null && !trackName.Equals(e.Name))
            {
                Out.WriteLine("Tracking " + e.Name + " due to collision");
            }
            // Set the target
            trackName = e.Name;
            // Back up a bit.
            // Note:  We won't get scan events while we're doing this!
            // An AdvancedRobot might use SetBack(); execute();
            gunTurnAmt = Utils.NormalRelativeAngleDegrees(e.Bearing + (Heading - RadarHeading));
            TurnGunRight(gunTurnAmt);
            Fire(3);
            Back(50);
        }

        /**
         * onWin:  Do a victory dance
         */
        public override void OnWin(WinEvent e)
        {
            for (int i = 0; i < 50; i++)
            {
                TurnRight(30);
                TurnLeft(30);
            }
        }

        /**
         * onHitWall:  Handle collision with wall.
         */
        public override void OnHitWall(HitWallEvent e)
        {
            // Bounce off!
            reverseDirection();
        }

        /**
         * reverseDirection:  Switch from Ahead to back & vice versa
         */
        public void reverseDirection()
        {
            if (movingForward)
            {
                SetBack(40000);
                movingForward = false;
            }
            else
            {
                SetAhead(40000);
                movingForward = true;
            }
        }
    }
}
