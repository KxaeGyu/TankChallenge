using System;
using System.Collections.Generic;
using System.Text;


public class TestRun
{
    static void Main(string[] args)
    {
        BuildLevel1();

        Map.map2[(-1, 1)].BlockType = BlockType.Unidentified;

        

        Map.PrintMap2();
    }

    private static void SpreadsheetExampleMock()
    {
        // Step 1
        Console.WriteLine("Step 1");
        LidarSamples.north = 6;
        LidarSamples.south = 3;
        LidarSamples.east = 4;
        LidarSamples.west = 7;

        Tank.myDir = CardinalDirection.North;

        Map.UpdateMap2();
        Map.PrintMap2();

        Tank.MoveForward();

        Console.WriteLine("");
        // Step 2
        Console.WriteLine("Step 2");
        LidarSamples.north = 5;
        LidarSamples.south = 4;
        LidarSamples.east = 4;
        LidarSamples.west = 7;

        Map.UpdateMap2();
        Map.PrintMap2();

        Tank.TurnRight();

        Console.WriteLine("");
        // Step3
        Console.WriteLine("Step 3");
        Map.UpdateMap2();
        Map.PrintMap2();

        Tank.MoveForward();

        Console.WriteLine("");
        // Step 4
        Console.WriteLine("Step 4");
        LidarSamples.north = 5;
        LidarSamples.south = 4;
        LidarSamples.east = 3;
        LidarSamples.west = 8;

        Map.UpdateMap2();
        Map.PrintMap2();
    }

    public static void BuildLevel1()
    {
        // Outer walls
        for (int x = -10; x <= 11; x++)
        {
            Map.map2.Add((x, 10), new Tile(x, 10, BlockType.Wall)); // top wall
            Map.map2.Add((x, -1), new Tile(x, -1, BlockType.Wall)); // bottow wall
        }
        for (int y = 0; y <= 9; y++)
        {
            Map.map2.Add((-10, y), new Tile(-10, y, BlockType.Wall));
            Map.map2.Add((11, y), new Tile(11, y, BlockType.Wall));
        }

        // Initialize everythin withing outer walls as Ground
        for (int y = 0; y <= 9; y++)
        {
            for (int x = -9; x <= 10; x++) 
            {
                Map.map2.Add((x, y), new Tile(x, y, BlockType.Ground));
            }
        }

        /////// Place inner walls and Hostiles
        ///
        // center wall placement
        for (int x = -7; x <= 8; x++)
        {
            Map.map2[(x, 5)].BlockType = BlockType.Wall;
        }
        // Hostile placement
        for (int y = 4; y <= 9; y += 5)
        {
            Map.map2[(-6, y)].BlockType = BlockType.Hostile;
            Map.map2[(-2, y)].BlockType = BlockType.Hostile;
            Map.map2[(3, y)].BlockType = BlockType.Hostile;
            Map.map2[(7, y)].BlockType = BlockType.Hostile;
        }

    }
}

