using System;


public class Solution : ISolution
{
    public Solution()
    {
        // You can initiate and calculate things here
    }
    /**
     * Executes a single step of the tank's programming. The tank can only move, turn,
     * or fire its cannon once per turn. Between each update, the tank's engine remains
     * running and consumes 1 fuel. This function will be called repeatedly until there
     * are no more targets left on the grid, or the tank runs out of fuel.
     */
    public EnemyScanner enemyScanner = new EnemyScanner();
    public VehicleMotionHandler driver = new VehicleMotionHandler();
    public Navigator navigator = new Navigator();

    void FindAndDestroy()
    {
        if (API.IdentifyTarget()) // If enemy within range, shoot
        {
            API.FireCannon();
        }
        if (enemyScanner.isEnemyApproaching()) // Find approaching enemy and face it
        {
            CardinalDirection enemyDirection = enemyScanner.GetDirectionNearestEnemy();
            driver.TurnTo(enemyDirection);
        }
    }

    internal void Explore()
    {
        /**
         * 
         * 
         */
    }

    public void Update()
    {
        //We are standing at some pos, need to check if monster is approaching us, in that case we need to face it and kill it.
        FindAndDestroy();
        //Find our way around the map.
        Explore();
    }

    internal class Navigator
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


    }

    internal class VehicleMotionHandler
    {
        public int LidarFrontTemp;

        void AutoMoveCW()
        {
            int LidarFrontTemp = API.LidarFront();
            if (LidarFrontTemp == 1)
            {
                if (!API.IdentifyTarget())
                {
                    TurnRight();
                }
            }
            else
            {
                MoveForward();
            }
        }

        public void MoveForward()
        {
            navigator.MoveForward();
            API.MoveForward();
        }

        public void TurnRight()
        {
            navigator.TurnRight();
            API.TurnRight();
        }

        public void TurnLeft()
        {
            navigator.TurnLeft();
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

    enum CardinalDirection
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
