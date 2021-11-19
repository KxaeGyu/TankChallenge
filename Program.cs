using System;
using System.Collections.Generic;
using System.Linq;

public class Solution
{
    public Solution()
    {
    }

    public void Update()
    {
        Tank.UpdateDistances();

        Map.ResizeMap();
        Map.UpdateMap2();

        Console.WriteLine("Current position is (x,y): (" + Tank.myPos.x + "," + Tank.myPos.y + ")");
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
}
public static class Map
{
    public static BlockType[,] map = new BlockType[1, 1];
    public static Dictionary<(int x, int y), Tile> map2 = new Dictionary<(int x, int y), Tile>();

    static Vector2 originOffset = new Vector2();
    static Vector2 globalPos;

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

    public static void UpdateMap2()
    {
        // North lidar
        Vector2 targetNorth = new Vector2() { x = Tank.myPos.x, y = (Tank.myPos.y + LidarSamples.north)};
        // --Target Tile / identify if facing north
        if (Tank.myDir == CardinalDirection.North)
        {
            if (!map2.ContainsKey((targetNorth.x, targetNorth.y))) // new Tile discovered
            {
                map2.Add((targetNorth.x, targetNorth.y),
                    new Tile(targetNorth.x, targetNorth.y, (API.IdentifyTarget() ? BlockType.Hostile : BlockType.Wall)));
            }
            else // override current BlockType with Wall.
            {
                map2[(targetNorth.x, targetNorth.y)].BlockType = (API.IdentifyTarget() ? BlockType.Hostile : BlockType.Wall);
            }
        }
        // --Tiles between Tank & Target
        for (int y = Tank.myPos.y; y < targetNorth.y; y++) // Tiles between Tank and North Target
        {
            if (!map2.ContainsKey((Tank.myPos.x, y))) // Set non-existing Tiles to Ground
            {
                map2.Add((Tank.myPos.x, y), new Tile(Tank.myPos.x, y, BlockType.Ground));
            }
            //else if (map2[(Tank.myPos.x, y)].BlockType == BlockType.Ground) // Object detected at Ground Tile ???
            //{
            //    map2[(Tank.myPos.x, y)].BlockType = BlockType.Hostile;
            //}
        }

        // South Lidar
        Vector2 targetSouth = new Vector2() { x = Tank.myPos.x, y = Tank.myPos.y - LidarSamples.south};
        // --Target Tile / identify if facing south
        if (Tank.myDir == CardinalDirection.South)
        {
            if (!map2.ContainsKey((targetSouth.x, targetSouth.y)))
            {
                map2.Add((targetSouth.x, targetSouth.y),
                    new Tile(targetSouth.x, targetSouth.y, (API.IdentifyTarget() ? BlockType.Hostile : BlockType.Wall)));
            }
            //add
        }
        // --Tiles between Tank & Target
        for (int y = targetSouth.y; y < Tank.myPos.y; y++) // Tiles between Tank and South Target
        {
            if (!map2.ContainsKey(((Tank.myPos.x, y)))) // Set non-existing Tiles to Ground
            {
                map2.Add((Tank.myPos.x, y), new Tile(Tank.myPos.x, y, BlockType.Ground));
            }
        }

        //East Lidar
        Vector2 targetEast = new Vector2() { x = Tank.myPos.x + LidarSamples.east, y = Tank.myPos.y };
        // --Target Tile / identify if facing East
        if (Tank.myDir == CardinalDirection.East)
        {
            if (!map2.ContainsKey((targetEast.x, targetEast.y)))
            {
                map2.Add((targetEast.x, targetEast.y),
                    new Tile(targetEast.x, targetEast.y, (API.IdentifyTarget() ? BlockType.Hostile : BlockType.Wall)));
            }
            //add
        }
        // --Tiles between Tank & Target
        for (int x = Tank.myPos.x; x < targetEast.x; x++) // Tiles between Tank and East Target
        {
            if (!map2.ContainsKey((x, Tank.myPos.y))) // Set non-existing Tiles to Ground
            {
                map2.Add((x, Tank.myPos.y), new Tile(x, Tank.myPos.y, BlockType.Ground));
            }
        }

        //West Lidar
        Vector2 targetWest = new Vector2() { x = Tank.myPos.x - LidarSamples.east};
        // --Target Tile / identify if facing West
        if (Tank.myDir == CardinalDirection.West)
        {
            if (!map2.ContainsKey((targetWest.x, targetWest.y)))
            {
                map2.Add((targetWest.x, targetWest.y),
                    new Tile(targetWest.x, targetWest.y, (API.IdentifyTarget() ? BlockType.Hostile : BlockType.Wall)));
            }
            //add
        }
        // --Tiles between Tank & Target
        for (int x = targetWest.x; x < Tank.myPos.x; x++)
        {
            if (!map2.ContainsKey((x, Tank.myPos.y)))
            {
                map2.Add((x, Tank.myPos.y), new Tile(x, Tank.myPos.y, BlockType.Ground));
            }
        }
        
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
        var stringPlot = "";
        int xMax = map2.Values.ToList().OrderByDescending(t => t.Position.x).FirstOrDefault().Position.x;
        int yMax = map2.Values.ToList().OrderByDescending(t => t.Position.y).FirstOrDefault().Position.y;

        var tiles = map2.Values.OrderByDescending(t => t.Position.y).ToList();
        foreach (Tile t in map2.Values)
        {
            //xMax =
        }
        int x = 0, y = 0;
        if (map2.ContainsKey((x, y)))
        {
            Console.WriteLine($"[{GetBlockTypeShortName( map2[(x, y)].BlockType )}]");
        }
    }

    public static string GetBlockTypeShortName(BlockType type)
    {
        switch (type)
        {
            case BlockType.Ground:
                return "[G]";
            case BlockType.Unidentified:
                return "[?]";
            case BlockType.Wall:
                return "[W]";
            case BlockType.Hostile:
                return "[H]";
            default:
                Console.WriteLine("GetBlockTypesShortName(): Unhandled BlockType encountered");
                return "[E]";
        }
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