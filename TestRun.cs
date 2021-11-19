using System;
using System.Collections.Generic;
using System.Text;


public class TestRun
{
    static void Main(string[] args)
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
}

