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

        int count = 0;
        double gunTurnAmt;
        String trackName;

        bool movingForward;
        bool trackStealthy;

        // inverse kans dat hij stealthy wordt. dus 1.0 - p = 0.6 in dit geval
        static readonly double thresStealthy = 0.4;

        static readonly double thresAggr = 0.66;
        static readonly double thresNormal = 0.33;

        /// <summary>
        /// Zoek naar een doelwit en rij erachteraan
        /// </summary>
        void track()
        {
            IsAdjustGunForRobotTurn = true;
            TurnGunRight(gunTurnAmt);
            count++;

            if (count > 2)
            {
                gunTurnAmt = -10;
            }
            
            // Kijk de andere kant op als hij niets heeft kunnen vinden
            if (count > 5)
            {
                gunTurnAmt = 10;
            }
            
            // Geef het op als het nog niet gelukt is
            if (count > 11)
            {
                trackName = null;
            }
        }

        /// <summary>
        /// Beweegt in een vreemd patroon om kogels zoveel mogelijk te ontwijken
        /// </summary>
        void crazy()
        {
            IsAdjustGunForRobotTurn = false;
            // Rij voor lange tijd vooruit
            SetAhead(40000);
            movingForward = true;
            SetTurnRight(90);
            // Wacht tot hij klaar is met draaien
            WaitFor(new TurnCompleteCondition(this));
            // Draai de andere kant op
            SetTurnLeft(180);
            // Wacht weer tot hij klaar is met draaien
            WaitFor(new TurnCompleteCondition(this));
            SetTurnRight(180);
            WaitFor(new TurnCompleteCondition(this));
        }

        /// <summary>
        /// Zoek nauwkeurig naar een doelwit
        /// </summary>
        void stealthy()
        {
            TurnGunRight(8);
        }

        /// <summary>
        /// Rij rond in een cirkelpatroon en schiet wanneer hij iemand ziet
        /// </summary>
        void spin()
        {
            IsAdjustGunForRobotTurn = false;
            SetTurnRight(10000);
            MaxVelocity = 5;
            Ahead(10000);
        }

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
            maxEnergy = Energy;

            // Prepare gun
            trackName = null;
            IsAdjustGunForRobotTurn = true;
            gunTurnAmt = 10;

            int behavior = -1, old_behavior;

            Random rand = new Random();

            // Robot hoofdloop
            while (true)
            {
                double e = Energy;
                // het oude gedrag
                old_behavior = behavior;

                // pas schaalfactor aan voor het bepalen van het gedrag
                if (e > maxEnergy)
                {
                    maxEnergy = e;
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

                // kies gedrag: aggresief tracker als >66%, crazy als >33% en anders spinbot
                if (e > maxEnergy * thresAggr)
                {
                    behavior = 0;
                }

                else if (e > maxEnergy * thresNormal)
                {
                    behavior = 1;
                    // kiest of hij langzaam gaat zoeken of willekeurig gaat bewegen
                    trackStealthy = rand.NextDouble() >= thresStealthy;
                }

                else
                {
                    behavior = 2;
                }

                // als we een nieuw gedrag hebben
                if (old_behavior != behavior)
                {
                    // stop oude stappen als we die nog in de wachtrij hebben
                    if (old_behavior != -1)
                    {
                        // Tweemalig om alles te resetten
                        Stop(true);
                        Stop(true);
                    }
                }

                // Voer gedrag uit
                switch (behavior)
                {
                    case 0:
                        SetAllColors(Color.Green);
                        track();
                        break;
                    case 1:
                        // Gedrag is willekeurig
                        if (trackStealthy)
                        {
                            SetAllColors(Color.Yellow);
                            stealthy();
                        }
                        else
                        {
                            SetAllColors(Color.Blue);
                            crazy();
                        }
                        break;
                    default:
                        SetAllColors(Color.Red);
                        spin();
                        break;
                }
            }
        }


        /// <summary>
        /// Zoek nauwkeurig en schiet als dat kan
        /// </summary>
        /// <param name="e"></param>
        void scan_stealthy(ScannedRobotEvent e)
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
            if (Energy > maxEnergy * thresAggr)
            {
                scanTrack(e);
            }
                
            else if (trackStealthy)
            {
                scan_stealthy(e);
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
                gunTurnAmt = Utils.NormalRelativeAngleDegrees(e.Bearing + (Heading - RadarHeading));

                SetTurnGunRight(gunTurnAmt);
                TurnRight(e.Bearing);
                Ahead(e.Distance - 140);
                return;
            }

            // Doelwit is dichtbij
            gunTurnAmt = Utils.NormalRelativeAngleDegrees(e.Bearing + (Heading - RadarHeading));
            TurnGunRight(gunTurnAmt);
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

            if (trackName != null && e.Name != trackName)
            {
                return;
            }

            // Stel doelwit bij
            if (trackName == null)
            {
                trackName = e.Name;
                Out.WriteLine("Snorbot: Start tracking " + trackName);
            }
            count = 0;

            StayCloseToTarget(e);

            Scan();
        }

        /// <summary>
        /// Hij rijdt tegen een andere robot aan
        /// </summary>
        public override void OnHitRobot(HitRobotEvent e)
        {
            // Schiet terug of draai als we niet agressief zijn
            if (Energy <= maxEnergy * thresAggr)
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
                if (trackName != null && trackName != e.Name)
                {
                    Out.WriteLine("Snorbot: now tracking " + e.Name + " due to collision");
                }

                trackName = e.Name;
                gunTurnAmt = Utils.NormalRelativeAngleDegrees(e.Bearing + (Heading - RadarHeading));
                TurnGunRight(gunTurnAmt);
                Fire(3);
                Back(50);
            }
        }

        /// <summary>
        /// Doe een overwinningsdans als hij gewonnen heeft
        /// </summary>
        /// <param name="e"></param>
        public override void OnWin(WinEvent e)
        {
            for (int i = 0; i < 40; i++)
            {
                TurnRight(20);
                TurnLeft(20);
            }
        }

        /// <summary>
        /// Hij botst tegen een muur
        /// </summary>
        public override void OnHitWall(HitWallEvent e)
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
