using System;
using System.Collections.Generic;
using System.Text;


public class TestRun
{
    static void Main(string[] args)
    {
        // Step 1
        LidarSamples.north = 6;
        LidarSamples.south = 3;
        LidarSamples.east = 4;
        LidarSamples.west = 7;

        Tank.myDir = CardinalDirection.North;

        Map.UpdateMap2();

        Tank.MoveForward();

        // Step 2
        LidarSamples.north = 5;
        LidarSamples.south = 4;
        LidarSamples.east = 4;
        LidarSamples.west = 7;

        Map.UpdateMap2();

        Tank.MoveForward();
    }
}

