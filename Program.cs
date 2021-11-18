using System;
using System.Collections.Generic;

public class Solution
{
    public Solution()
    {
    }

    public void Update()
    {
        Tank.UpdateDistances();
        Map.UpdateMap();

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

    public static void UpdateMap()
    {
    }
    public static void UpdateMap2()
    {

        if (map2.ContainsKey((Tank.myPos.x, LidarSamples.north))) 
        {
            for (int y = Tank.myPos.y; y < (Tank.myPos.y + LidarSamples.north); y++) // Tiles between Tank and North Target
            {
                if (!map2.ContainsKey((Tank.myPos.x, y))) // Set non-existing Tiles to Ground
                {
                    map2.Add((Tank.myPos.x, y), new Tile(Tank.myPos.x, y, BlockType.Ground));
                }
            }

            for (int y = (Tank.myPos.y - LidarSamples.south); y < Tank.myPos.y; y++) // Tiles between Tank and South Target
            {
                if (!map2.ContainsKey(((Tank.myPos.x, y)))) // Set non-existing Tiles to Ground
                {
                    map2.Add((Tank.myPos.x, y), new Tile(Tank.myPos.x, y, BlockType.Ground));
                }
            }

            for (int x = Tank.myPos.x; x < (Tank.myPos.x + LidarSamples.east); x++) // Tiles between Tank and East Target
            {
                if (!map2.ContainsKey((x, Tank.myPos.y))) // Set non-existing Tiles to Ground
                {
                    map2.Add((x, Tank.myPos.y), new Tile(x, Tank.myPos.y, BlockType.Ground));
                }
            }

            for (int x = (Tank.myPos.x - LidarSamples.west); x < Tank.myPos.x; x++)
            {
                if (!map2.ContainsKey((x, Tank.myPos.y)))
                {
                    map2.Add((x, Tank.myPos.y), new Tile(x, Tank.myPos.y, BlockType.Ground);
                }
            }
        }
        
        
        var a = Tank.myPos;

        LidarSamples.north
        
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