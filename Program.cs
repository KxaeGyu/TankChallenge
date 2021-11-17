using System;
using System.Linq;

public class Solution : ISolution
{
    Solution()
    {
        // You can initiate and calculate things here
    }
    /**
     * Executes a single step of the tank's programming. The tank can only move, turn,
     * or fire its cannon once per turn. Between each update, the tank's engine remains
     * running and consumes 1 fuel. This function will be called repeatedly until there
     * are no more targets left on the grid, or the tank runs out of fuel.
     */
    EnemyScanner enemyScanner = new EnemyScanner();
    VehicleMotionHandler driver = new VehicleMotionHandler();

    bool FindAndDestroy()
    {
        bool actionTaken = false;

        if (API.IdentifyTarget()) // If enemy within range, shoot
        {
            API.FireCannon();
            actionTaken = true;
        }
        if (enemyScanner.isEnemyApproaching()) // Find approaching enemy and face it
        {
            CardinalDirection enemyDirection = enemyScanner.GetDirectionNearestEnemy();
            driver.TurnTo(enemyDirection);
            actionTaken = true;
        }
        return actionTaken;
    }

    enum EndingAction
    {
        FireCanon,
        MoveForward,
        MoveBackward,
        TurnRight,
        TurnLeft
    }

    internal void Explore()
    {
        driver.AutoMoveCW();
    }

    public void Update()
    {
        //We are standing at some pos, need to check if monster is approaching us, in that case we need to face it and kill it.
        if (!FindAndDestroy())
        {
            Explore();
        }
        //Find our way around the map.
    }

    public class Navigator
    {
        public CardinalDirection currentHeading;

        public Navigator()
        {
            currentHeading = CardinalDirection.North; // Initial direction always north.
        }

        public static Navigator Instance
        {
            get
            {
                if (Instance == null) { Instance = new Navigator(); }
                return Instance;
            }
            private set { }
        }

        internal CardinalDirection CardinalDirectionRight()
        {
            var Arr = (CardinalDirection[])Enum.GetValues(typeof(CardinalDirection));
            int currentIndex = Arr.Where(d => d == currentHeading).;
            // use currentHeading and move +1 in array
            // if at last index, return first
            return Arr[currentIndex];
        }

        internal CardinalDirection CardinalDirectionLeft()
        {
            var Arr = (CardinalDirection[])Enum.GetValues(typeof(CardinalDirection));
            int currentIndex = Arr.Select((d, i) => (d, i)).First(d == currentHeading).index;
            // use currentHeading and move -1 in array.
            // if at first index, return last

            return Arr[i];
        }

        internal void TurnRight()
        {
            //throw new NotImplementedException();
        }

        internal void TurnLeft()
        {
            //throw new NotImplementedException();
        }

        internal void MoveForward()
        {
            //throw new NotImplementedException();
        }
    }

    internal class VehicleMotionHandler
    {
        public int LidarFrontTemp;

        internal void AutoMoveCW()
        {
            int LidarFrontTemp = API.LidarFront();
            int LidarLeftTemp = API.LidarLeft();
            if (LidarFrontTemp == 1)
            {
                if (!API.IdentifyTarget())
                {
                    TurnRight();
                }
            }
            else if (LidarLeftTemp > 1)
                {
                    TurnLeft();
                }
            else
            {
                MoveForward();
            }
        }

        void MoveForward()
        {
            Navigator.Instance.MoveForward();
            API.MoveForward();
        }

        void TurnRight()
        {
            Navigator.Instance.TurnRight();
            API.TurnRight();
        }

        internal void TurnLeft()
        {
            Navigator.Instance.TurnLeft();
            API.TurnLeft();
        }

        public void TurnTo(CardinalDirection cardinalDirection) { }
    }

    internal class EnemyScanner
    {
        internal CardinalDirection GetDirectionNearestEnemy()
        {
            throw new NotImplementedException();
        }

        internal bool isEnemyApproaching()
        {
            throw new NotImplementedException();
        }
    }

    public enum CardinalDirection
    {
        North,
        East,
        South,
        West
    }

    internal class MapMaker
    {
        enum blockTypes
        {
            Floor,
            Unidentified,
            Wall,
            Target
        }

        int[,] map = new int[1, 1]; //[1] upp/ner [2] vänster/höger
        int[] myPos = { 0, 0 };

        void ResizeMap()
        {
        }
        //API from High-level code
        void UpdateMap(int dst_North, int dst_South, int dst_East, int dst_West)
        {
        }

        void PrintMap()
        {
        }




    }
}
