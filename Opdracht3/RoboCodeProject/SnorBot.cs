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
        double maxEnergy;
        bool trackStealthy;

        // kans dat hij stealthy wordt
        static readonly double chanceStealthy = 0.5;

        static readonly double thresAgressive = 0.66;
        static readonly double thresNormal = 0.33;

        public BTNode BhvAgressive;
        public BTNode BhvCautious;
        public BTNode BhvNormal;
        public BTNode BhvNormalAlt;
        public BTNode BhvReverse;
        public BTNode BhvVictoryDance;

        public BlackBoard blackBoard;

        /// <summary>
        /// Hoofdprogramma van de robot. Kiest tussen drie verschillende patronen afhankelijk van resterende energie:
        /// * Agressief: Kies de dichtstbijzijnde tank en blijf deze achtervolgen. Probeer hem ook te rammen
        /// * Normaal: kies dichtstbijzijnde tank en blijf deze van een redelijke afstand beschieten
        /// * Voorzichtig: blijf bij de tanks vandaan. Blijf veel rondrijden en schiet af en toe
        /// 
        /// Het normale patroon kiest afwisselend tussen de twee patronen: langzaam scannen en gek bewegen
        /// Dit maakt hem een stuk onvoorspelbaarder
        /// </summary>
        public override void Run()
        {
            Random rand = new Random();
            blackBoard = new BlackBoard(this);

            maxEnergy = Energy;
            IsAdjustGunForRobotTurn = true;
            blackBoard.gunTurnAmt = 10;
            int behaviour = -1, oldBehaviour;

            BhvAgressive = new Sequence(blackBoard,
                new ChangeColor(blackBoard, Color.Green),
                new TrackRobot(blackBoard)
            );

            BhvCautious = new Sequence(blackBoard,
                new ChangeColor(blackBoard, Color.Red),
                new CircleMove(blackBoard)
            );

            BhvNormal = new Sequence(blackBoard,
                new ChangeColor(blackBoard, Color.Blue),
                new DrunkMove(blackBoard)
            );

            BhvNormalAlt = new Sequence(blackBoard,
                new ChangeColor(blackBoard, Color.Yellow),
                new Stationary(blackBoard)
            );

            BhvReverse = new Sequence(blackBoard,
                new ChangeColor(blackBoard, Color.Purple),
                new DriveReverse(blackBoard)
            );

            BhvVictoryDance = new Sequence(blackBoard,
                new Dance(blackBoard)
            );

            // Robot hoofdloop
            while (true)
            {
                // het oude gedrag
                oldBehaviour = behaviour;

                // pas schaalfactor aan voor het bepalen van het gedrag
                if (Energy > maxEnergy)
                {
                    maxEnergy = Energy;
                }

                else
                {
                    // Verlaag langzaam de drempelwaarde om ervoor te zorgen dat zijn gedrag verandert als een gevecht lang duurt
                    maxEnergy *= 0.98;
                    if (maxEnergy < 80)
                    {
                        maxEnergy = 80;
                    }       
                }

                // kies gedrag: aggresief als >66%, normaal als >33% en anders spinbot
                if (Energy > maxEnergy * thresAgressive)
                {
                    behaviour = 0;
                }

                else if (Energy > maxEnergy * thresNormal)
                {
                    behaviour = 1;
                    // kiest of hij langzaam gaat zoeken of willekeurig gaat bewegen
                    trackStealthy = rand.NextDouble() >= chanceStealthy;
                }

                else
                {
                    behaviour = 2;
                }

                // als we een nieuw gedrag hebben
                if (oldBehaviour != behaviour)
                {
                    // stop oude stappen als we die nog in de wachtrij hebben
                    if (oldBehaviour != -1)
                    {
                        // Tweemalig om alles te resetten
                        Stop(true);
                        Stop(true);
                    }
                }

                // Voer gedrag uit
                switch (behaviour)
                {
                    case 0:
                        BhvAgressive.Tick();
                        break;
                    case 1:
                        // Gedrag is willekeurig
                        if (trackStealthy)
                        {
                            BhvNormalAlt.Tick();
                        }

                        else
                        {
                            BhvNormal.Tick();
                        }

                        break;
                    default:
                        BhvCautious.Tick();
                        break;
                }
            }
        }

        /// <summary>
        /// Zoek nauwkeurig en schiet als dat kan
        /// </summary>
        /// <param name="e"></param>
        void ScanStealthy(ScannedRobotEvent e)
        {
            // Bepaal de positie van de gespotte robot
            double absoluteBearing = Heading + e.Bearing;
            double bearingFromGun = Utils.NormalRelativeAngleDegrees(absoluteBearing - GunHeading);

            // Schiet als hij dichtbij is
            if (Math.Abs(bearingFromGun) <= 3)
            {
                TurnGunRight(bearingFromGun);

                // Schiet alleen als dat kan
                if (GunHeat == 0)
                {
                    Fire(Math.Min(3 - Math.Abs(bearingFromGun), Energy - .1));
                }
            }

            else
            {
                TurnGunRight(bearingFromGun);
            }

            if (bearingFromGun == 0)
            {
                Scan();
            }
        }

        /// <summary>
        /// Bepaal gedrag wat uitgevoerd zal worden wanneer er een robot gespot wordt
        /// </summary>
        /// <param name="e"></param>
        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            // Achtervolgt robot als deze agressief is
            if (Energy > maxEnergy * thresAgressive)
            {
                scanTrack(e);
            }
                
            else if (trackStealthy)
            {
                ScanStealthy(e);
            }
                
            else
            {
                // Het leeft! Schiet!
                Fire(Energy > maxEnergy * thresNormal ? 2 : 1);
            }       
        }

        /// <summary>
        /// Zorgt ervoor dat hij dicht genoeg bij het doelwit is maar niet zo dichtbij dat hij er tegen aanknalt
        /// </summary>
        /// <param name="e"></param>
        public void StayCloseToTarget(ScannedRobotEvent e)
        {
            // Rij richting doelwit als hij er te ver vandaan is
            if (e.Distance > 150)
            {
                blackBoard.gunTurnAmt = Utils.NormalRelativeAngleDegrees(e.Bearing + (Heading - RadarHeading));

                SetTurnGunRight(blackBoard.gunTurnAmt);
                TurnRight(e.Bearing);
                Ahead(e.Distance - 140);
                return;
            }

            // Doelwit is dichtbij
            blackBoard.gunTurnAmt = Utils.NormalRelativeAngleDegrees(e.Bearing + (Heading - RadarHeading));
            TurnGunRight(blackBoard.gunTurnAmt);
            Fire(3);

            // Rij terug als hij te dichtbij is
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
        }

        public void scanTrack(ScannedRobotEvent e)
        {
            // Wacht voor een doelwit
            Out.WriteLine("Snorbot: scan");

            if (blackBoard.trackName != null && e.Name != blackBoard.trackName)
            {
                return;
            }

            // Stel doelwit bij
            if (blackBoard.trackName == null)
            {
                blackBoard.trackName = e.Name;
                Out.WriteLine("Snorbot: Start tracking " + blackBoard.trackName);
            }

            blackBoard.count = 0;
            StayCloseToTarget(e);

            Scan();
        }

        /// <summary>
        /// Hij rijdt tegen een andere robot aan
        /// </summary>
        public override void OnHitRobot(HitRobotEvent e)
        {
            // Schiet terug of draai als we niet agressief zijn
            if (Energy <= maxEnergy * thresAgressive)
            {
                if (e.Bearing > -10 && e.Bearing < 10)
                {
                    Fire(Energy > maxEnergy * 0.33 ? 2 : 1);
                }

                if (e.IsMyFault)
                {
                    TurnRight(10);
                }
            }

            else
            {
                if (blackBoard.trackName != null && blackBoard.trackName != e.Name)
                {
                    Out.WriteLine("Snorbot: now tracking " + e.Name + " due to collision");
                }

                blackBoard.trackName = e.Name;
                blackBoard.gunTurnAmt = Utils.NormalRelativeAngleDegrees(e.Bearing + (Heading - RadarHeading));
                TurnGunRight(blackBoard.gunTurnAmt);
                Fire(3);
                Back(50);
            }
        }

        /// <summary>
        /// Doe een overwinningsdans als hij heeft gewonnen
        /// </summary>
        /// <param name="e"></param>
        public override void OnWin(WinEvent e)
        {
            BhvVictoryDance.Tick();
        }

        /// <summary>
        /// Hij botst tegen een muur
        /// </summary>
        public override void OnHitWall(HitWallEvent e)
        {
            BhvReverse.Tick();
        }
    }
}
