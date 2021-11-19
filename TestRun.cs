using System;
using System.Collections.Generic;
using System.Text;


public class TestRun
{
    static void Main(string[] args)
    {
        LidarSamples.north = 6;
        LidarSamples.south = 3;
        LidarSamples.east = 4;
        LidarSamples.west = 7;

        Tank.myDir = CardinalDirection.North;

        Map.UpdateMap2();

    }
}

