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

    static Vector2 originOffset = new Vector2();
    static Vector2 globalPos;

    public static void ResizeMap()
    {
        globalPos.x = (originOffset.x + Tank.myPos.x);
        globalPos.y = (originOffset.y + Tank.myPos.y);
        int currentDstNorth = LidarSamples.north - globalPos.y;
        int currentDstEast  = LidarSamples.east  - globalPos.x;
        int currentDstSouth = LidarSamples.south - globalPos.y;
        int currentDstWest  = LidarSamples.west  - globalPos.x;

        int deltaYn = currentDstNorth - (map.GetLength(1) - globalPos.y);
        int deltaYs = currentDstSouth - globalPos.y;
        int deltaXe = currentDstEast - (map.GetLength(0) - globalPos.x);
        int deltaXw = currentDstWest - globalPos.x;


        if (deltaYn+ deltaYs+ deltaXe+ deltaXw > 0)
        {
            BlockType[,] newMap = new BlockType[deltaXe+deltaXw+map.GetLength(0), deltaYn + deltaYs + map.GetLength(1)];

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    newMap[deltaXe + i, deltaYs + j] = map[i, j];
                }
            }

            originOffset.x += deltaXe;
            originOffset.y += deltaYs;
        }


    }

    public static void PrintMap()
    {
        /*
        [ ][ ][ ][ ][W]
                 [ ][?]
        */
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