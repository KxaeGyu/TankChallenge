using System;
using System.Collections.Generic;
using System.Linq;

public class Solution
{
    int tic;
    public Solution()
    {
        tic = 0;
    }

    public void Update()
    {

        Tank.UpdateDistances();
        Map.UpdateMap2();

        Tile hostile = Map.GetClosestHostile();

        if (hostile != null)
        {
            // 1. move to line of sight
            // 2. face hostile
            // 3. shoot
            // steps above will not be remembered over simulation steps, so it must check which step is performed
        }

        Tile targetTile = Tank.GetTargetToScan(); // API.IdentifyTarget on best target





        Console.WriteLine("Current position is (x,y): (" + Tank.myPos.x + "," + Tank.myPos.y + ")");
        tic++;
    }


}
// Enumerations
public enum CardinalDirection : uint
{
    North,  //0
    East,   //1
    South,  //2
    West    //3
}

public enum BlockType
{
    Ground,
    Unidentified,
    Wall,
    Hostile
}
public static class Tank
{
    static int[] movementVector = { 1, 0, -1, 0 };

    public static Vector2 myPos = new Vector2();
    public static CardinalDirection myDir = CardinalDirection.North;

    //must run first in every loop
    public static void UpdateDistances()
    {
        Console.WriteLine("Updating Distances");

        int[] distances = new int[] { API.LidarFront(), API.LidarRight(), API.LidarBack(), API.LidarLeft() };
        int[] shiftedDistances = new int[distances.Length];

        for (int i = 0; i < distances.Length; i++)
        {
            shiftedDistances[(i + (int)myDir) % distances.Length] = distances[i];
        }
        LidarSamples.north = shiftedDistances[0];
        LidarSamples.east = shiftedDistances[1];
        LidarSamples.south = shiftedDistances[2];
        LidarSamples.west = shiftedDistances[3];
    }

    public static void TurnLeft()
    {
        Console.WriteLine("Turning Left");

        myDir = (CardinalDirection)(((uint)Tank.myDir - 1) % 4);
        API.TurnLeft();

        Console.WriteLine("I am facing: " + Tank.myDir);
    }
    public static void TurnRight()
    {
        Console.WriteLine("Turning Right");

        myDir = (CardinalDirection)(((uint)myDir + 1) % 4);
        API.TurnRight();

        Console.WriteLine("I am facing: " + Tank.myDir);
    }
    public static void MoveForward()
    {
        Console.WriteLine("Moving Forward");

        myPos.x += movementVector[((int)myDir + 3) % 4];
        myPos.y += movementVector[(int)myDir];

        API.MoveForward();
    }
    public static void MoveBackward()
    {
        Console.WriteLine("Moving Backward");

        myPos.x -= movementVector[((int)myDir + 3) % 4];
        myPos.y -= movementVector[(int)myDir];

        API.MoveBackward();
    }

    public static Tile GetTargetToScan()
    {
        // It shall return the best/cheapest target to identify. This tile can be used as an input to the path planner
        // Estimate outer border Tiles
        var priorityTargets = new List<Tile>();
        var targetCost = new Dictionary<Tile, int>();

        foreach (Tile tile in Map.map2.Values)
        {
            if (Enumerable.Range(Map.xMin + 1, Map.xMax - 1).Contains(tile.Position.x) && Enumerable.Range(Map.yMin + 1, Map.yMax - 1).Contains(tile.Position.y))
            {
                priorityTargets.Add(tile);
                targetCost.Add(tile, GetLineOfSightCost());
            }
        }

        if (priorityTargets.Count > 0)
        {
            foreach (Tile tile in priorityTargets)
            {

            }
        }

        return new Tile(0, 0); // replace with chosen tile
    }

    private static int GetLineOfSightCost()
    {
        throw new NotImplementedException();
    }
}
public static class Map
{
    // Resize map variables
    public static BlockType[,] map = new BlockType[1, 1];
    static Vector2 originOffset = new Vector2();
    static Vector2 globalPos;

    // UpdateMap2 variables
    public static Dictionary<(int x, int y), Tile> map2 = new Dictionary<(int x, int y), Tile>();
    public static int xMax = 0;
    public static int xMin = 0;
    public static int yMax = 0;
    public static int yMin = 0;

    public static void ResizeMap()
    {
        globalPos.x = (originOffset.x + Tank.myPos.x);
        globalPos.y = (originOffset.y + Tank.myPos.y);
        int currentDstNorth = globalPos.y + LidarSamples.north + 1 - map.GetLength(1);
        int currentDstEast = globalPos.x + LidarSamples.east + 1 - map.GetLength(0);
        int currentDstSouth = LidarSamples.south - globalPos.y;
        int currentDstWest = LidarSamples.west - globalPos.x;

        int deltaYn = (currentDstNorth > 0) ? currentDstNorth : 0;
        int deltaYs = (currentDstSouth - globalPos.y) > 0 ? (currentDstSouth - globalPos.y) : 0;
        int deltaXe = (currentDstEast > 0) ? currentDstEast : 0;
        int deltaXw = (currentDstWest - globalPos.x) > 0 ? (currentDstWest - globalPos.x) : 0;

        if (deltaYn > 0 || deltaYs > 0 || deltaXe > 0 || deltaXw > 0)
        {
            BlockType[,] newMap = new BlockType[deltaXe + deltaXw + map.GetLength(0), deltaYn + deltaYs + map.GetLength(1)];
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    newMap[deltaXw + i, deltaYs + j] = map[i, j];
                }
            }
            map = newMap;
            originOffset.x += deltaXw;
            originOffset.y += deltaYs;
            globalPos.x = (originOffset.x + Tank.myPos.x);
            globalPos.y = (originOffset.y + Tank.myPos.y);
        }
    }
    
    public static Vector2 UpdateMapDirection(CardinalDirection lidarDirection)
    {
        Vector2 target = new Vector2()
        { 
            x = lidarDirection == CardinalDirection.North || lidarDirection == CardinalDirection.South ? 
                Tank.myPos.x : 
                lidarDirection == CardinalDirection.East ? Tank.myPos.x + LidarSamples.east : Tank.myPos.x - LidarSamples.west, 
            y = lidarDirection == CardinalDirection.East || lidarDirection == CardinalDirection.West ? 
                Tank.myPos.y : 
                lidarDirection == CardinalDirection.North ? Tank.myPos.y + LidarSamples.north : Tank.myPos.y - LidarSamples.south
        };

        // Add or Update Target Tile
        if (!map2.ContainsKey((target.x, target.y))) // new Tile discovered
        {
            map2.Add((target.x, target.y),
                new Tile(target.x, target.y, (Tank.myDir == lidarDirection) ?
                    (API.IdentifyTarget() ? BlockType.Hostile : BlockType.Wall) // Facing target. API.IdentifyTarget available
                    :
                    BlockType.Unidentified                                      // Not facing target. Only Lidar available
                ));
        }
        else // Update Tile BlockType
        {
            if (Tank.myDir == lidarDirection) // Scan Target
            {
                map2[(target.x, target.y)].BlockType = API.IdentifyTarget() ? BlockType.Hostile : BlockType.Wall;
            }
            else if (map2[(target.x, target.y)].BlockType == BlockType.Ground) // Target scanned on Ground => Hostile
            {
                map2[(target.x, target.y)].BlockType = BlockType.Hostile;
            }
            else // Target found Left, Right or Behind Tank => can not identify => Unidentified
            {
                //map2[(target.x, target.y)].BlockType = BlockType.Unidentified;
            }
            //map2[(targetNorth.x, targetNorth.y)].BlockType = (Tank.myDir == CardinalDirection.North) ? 
            //    (API.IdentifyTarget() ? BlockType.Hostile : BlockType.Wall) :
            //    BlockType.Unidentified
            //    ;
        }

        // Set Tiles between Tank & Target to ground
        //int dxy = lidarDirection == CardinalDirection.North || lidarDirection == CardinalDirection.South ? Math.Min(Tank.myPos.y, target.y ) : Math.Min(Tank.myPos.x, target.x); // adjust to not cover target tile
        //int end = lidarDirection == CardinalDirection.North || lidarDirection == CardinalDirection.South ? Math.Max(Tank.myPos.y, target.y) : Math.Max(Tank.myPos.x, target.x);
        int xyStart = 0;
        int xyEnd = 0;
        if (lidarDirection == CardinalDirection.North)
        {
            xyStart = Tank.myPos.y + 1;
            xyEnd = target.y - 1;
        }
        else if (lidarDirection == CardinalDirection.South)
        {
            xyStart = target.y + 1;
            xyEnd = Tank.myPos.y - 1;
        }
        else if (lidarDirection == CardinalDirection.East)
        {
            xyStart = Tank.myPos.x + 1;
            xyEnd = target.x - 1;
        }
        else if(lidarDirection == CardinalDirection.West)
        {
            xyStart = target.x + 1;
            xyEnd = Tank.myPos.x - 1;
        }

        if (lidarDirection == CardinalDirection.North || lidarDirection == CardinalDirection.South) // iterate on y-axis
        {
            for (int y = xyStart; y <= xyEnd; y++)
            {
                if (!map2.ContainsKey((Tank.myPos.x, y))) // Set non-existing Tiles to Ground
                {
                    map2.Add((Tank.myPos.x, y), new Tile(Tank.myPos.x, y, BlockType.Ground));
                }
                else // override to ground
                {
                    map2[(Tank.myPos.x, y)].BlockType = BlockType.Ground;
                }
            }
        }
        else // iterate on x-axis
        {
            for (int x = xyStart; x <= xyEnd; x++)
            {
                if (!map2.ContainsKey((x, Tank.myPos.y))) // Set non-existing Tiles to Ground
                {
                    map2.Add((x, Tank.myPos.y), new Tile(x, Tank.myPos.y, BlockType.Ground));
                }
                else // override to ground
                {
                    map2[(x, Tank.myPos.y)].BlockType = BlockType.Ground;
                }
            }

        }


        return target;
    }
    public static void UpdateMap2()
    {

        foreach (var lidar in Enum.GetValues(typeof(CardinalDirection)).Cast<CardinalDirection>().ToList())
        {
            UpdateMapDirection(lidar);
        }

        //// North lidar
        //Vector2 targetNorth = new Vector2() { x = Tank.myPos.x, y = (Tank.myPos.y + LidarSamples.north)};
        //// --Target Tile / identify if facing north
        //if (!map2.ContainsKey((targetNorth.x, targetNorth.y))) // new Tile discovered
        //{
        //    map2.Add((targetNorth.x, targetNorth.y),
        //        new Tile(targetNorth.x, targetNorth.y, (Tank.myDir == CardinalDirection.North) ?
        //            (API.IdentifyTarget() ? BlockType.Hostile : BlockType.Wall) // Facing target. API.IdentifyTarget available
        //            :
        //            BlockType.Unidentified                                      // Not facing target. Only Lidar available
        //        ));
        //}
        //else // Update Tile BlockType
        //{
        //    if (Tank.myDir == CardinalDirection.North) // Scan Target
        //    {
        //        map2[(targetNorth.x, targetNorth.y)].BlockType = API.IdentifyTarget() ? BlockType.Hostile : BlockType.Wall;
        //    }
        //    else if (map2[(targetNorth.x, targetNorth.y)].BlockType == BlockType.Ground) // Target scanned on Ground => Hostile
        //    {
        //        map2[(targetNorth.x, targetNorth.y)].BlockType = BlockType.Hostile;
        //    }
        //    else // Target found Left, Right or Behind Tank => can not identify => Unidentified
        //    {
        //        map2[(targetNorth.x, targetNorth.y)].BlockType = BlockType.Unidentified;
        //    }
        //    //map2[(targetNorth.x, targetNorth.y)].BlockType = (Tank.myDir == CardinalDirection.North) ? 
        //    //    (API.IdentifyTarget() ? BlockType.Hostile : BlockType.Wall) :
        //    //    BlockType.Unidentified
        //    //    ;
        //}

        //// --Tiles between Tank & Target
        //for (int y = Tank.myPos.y; y < targetNorth.y; y++) // Tiles between Tank and North Target
        //{
        //    if (!map2.ContainsKey((Tank.myPos.x, y))) // Set non-existing Tiles to Ground
        //    {
        //        map2.Add((Tank.myPos.x, y), new Tile(Tank.myPos.x, y, BlockType.Ground));
        //    }
        //    //else if (map2[(Tank.myPos.x, y)].BlockType == BlockType.Ground) // Object detected at Ground Tile ???
        //    //{
        //    //    map2[(Tank.myPos.x, y)].BlockType = BlockType.Hostile;
        //    //}
        //}

        //// South Lidar
        //Vector2 targetSouth = new Vector2() { x = Tank.myPos.x, y = Tank.myPos.y - LidarSamples.south};
        //// --Target Tile / identify if facing south
        //if (!map2.ContainsKey((targetSouth.x, targetSouth.y)))
        //{
        //    map2.Add((targetSouth.x, targetSouth.y),
        //        new Tile(targetSouth.x, targetSouth.y, (API.IdentifyTarget() ? BlockType.Hostile : BlockType.Wall)));
        //}
        //else // Update Tile BlockType
        //{

        //}

        //// --Tiles between Tank & Target
        //for (int y = Tank.myPos.y; y > targetSouth.y; y--) // Tiles between Tank and South Target
        //{
        //    if (!map2.ContainsKey(((Tank.myPos.x, y)))) // Set non-existing Tiles to Ground
        //    {
        //        map2.Add((Tank.myPos.x, y), new Tile(Tank.myPos.x, y, BlockType.Ground));
        //    }
        //}

        ////East Lidar
        //Vector2 targetEast = new Vector2() { x = Tank.myPos.x + LidarSamples.east, y = Tank.myPos.y };
        //// --Target Tile / identify if facing East
        //if (Tank.myDir == CardinalDirection.East)
        //{
        //    if (!map2.ContainsKey((targetEast.x, targetEast.y)))
        //    {
        //        map2.Add((targetEast.x, targetEast.y),
        //            new Tile(targetEast.x, targetEast.y, (API.IdentifyTarget() ? BlockType.Hostile : BlockType.Wall)));
        //    }
        //    //add
        //}
        //// --Tiles between Tank & Target
        //for (int x = Tank.myPos.x; x < targetEast.x; x++) // Tiles between Tank and East Target
        //{
        //    if (!map2.ContainsKey((x, Tank.myPos.y))) // Set non-existing Tiles to Ground
        //    {
        //        map2.Add((x, Tank.myPos.y), new Tile(x, Tank.myPos.y, BlockType.Ground));
        //    }
        //}

        ////West Lidar
        //Vector2 targetWest = new Vector2() { x = Tank.myPos.x - LidarSamples.west, y = Tank.myPos.y};
        //// --Target Tile / identify if facing West
        //if (Tank.myDir == CardinalDirection.West)
        //{
        //    if (!map2.ContainsKey((targetWest.x, targetWest.y)))
        //    {
        //        map2.Add((targetWest.x, targetWest.y),
        //            new Tile(targetWest.x, targetWest.y, (API.IdentifyTarget() ? BlockType.Hostile : BlockType.Wall)));
        //    }
        //    //add
        //}
        //// --Tiles between Tank & Target
        //for (int x = Tank.myPos.x; x > targetWest.x; x--)
        //{
        //    if (!map2.ContainsKey((x, Tank.myPos.y)))
        //    {
        //        map2.Add((x, Tank.myPos.y), new Tile(x, Tank.myPos.y, BlockType.Ground));
        //    }
        //}

    }
    public static void PrintMap()
    {
        for (int j = map.GetLength(1) - 1; j >= 0; j--)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                if (i == globalPos.x && j == globalPos.y)
                {
                    Console.Write("[X]");
                }
                else if (i == originOffset.x && j == originOffset.y)
                {
                    Console.Write("[O]");
                }
                else
                {
                    switch (map[i, j])
                    {
                        case BlockType.Ground:
                            Console.Write("[ ]");
                            break;
                        case BlockType.Unidentified:
                            Console.Write("[?]");
                            break;
                        case BlockType.Wall:
                            Console.Write("[W]");
                            break;
                        case BlockType.Hostile:
                            Console.Write("[H]");
                            break;
                    }
                }

            }
            Console.WriteLine();
        }
    }

    public static void PrintMap2()
    {
        

        foreach (var coordinate in map2.Keys.ToList())
        {
            if (coordinate.x < xMin) xMin = coordinate.x;
            if (coordinate.x > xMax) xMax = coordinate.x;
            if (coordinate.y < yMin) yMin = coordinate.y;
            if (coordinate.y > yMax) yMax = coordinate.y;
        };


        for (int y = yMax; y >= yMin; y--)
        {
            for (int x = xMin; x <= xMax; x++)
            {
                if (map2.ContainsKey((x, y)))
                {
                    Console.Write($"[{GetBlockTypeShortName(map2[(x, y)].BlockType)}]");
                }
                else
                {
                    Console.Write("[ ]");
                }
            }
            Console.Write(Environment.NewLine);
        }
        
    }

    public static string GetBlockTypeShortName(BlockType type)
    {
        switch (type)
        {
            case BlockType.Ground:
                return "G";
            case BlockType.Unidentified:
                return "?";
            case BlockType.Wall:
                return "W";
            case BlockType.Hostile:
                return "H";
            default:
                Console.WriteLine("GetBlockTypesShortName(): Unhandled BlockType encountered");
                return "E";
        }
    }

    internal static Tile GetClosestHostile()
    {
        List<Tile> list = map2.Values.Where(t => t.BlockType == BlockType.Hostile).ToList();
        return list.FirstOrDefault();
    }
}


public static class LidarSamples
{
    public static int north { get; set; }
    public static int east { get; set; }
    public static int south { get; set; }
    public static int west { get; set; }
}
public class Vector2
{
    public int x = 0;
    public int y = 0;

}

public class Tile
{
    public Vector2 Position { get; set; }
    public BlockType BlockType { get; set; }
    public Tile(int x, int y, BlockType blockType = BlockType.Unidentified)
    {
        Position = new Vector2();
        Position.x = x;
        Position.y = y;

        BlockType = blockType;
    }

}

// test albin