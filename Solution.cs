using System;
using System.Collections.Generic;

public class Solution : ISolution
{
    EndingAction endingAction;
    CardinalDirection currentHeading;
    Lidar lidar;
    blockType[,] worldMap;
    //Dictionary<blockType> worldMap2;
    public Solution()
    {
        // You can initiate and calculate things here
        worldMap = new blockType[,] { { blockType.Ground }, { blockType.Ground } };
        currentHeading = CardinalDirection.North;
        lidar = new Lidar();
        //worldMap2 = new Dictionary<blockType> { }
    }

    /**
     * Executes a single step of the tank's programming. The tank can only move, turn,
     * or fire its cannon once per turn. Between each update, the tank's engine remains
     * running and consumes 1 fuel. This function will be called repeatedly until there
     * are no more targets left on the grid, or the tank runs out of fuel.
     */
    public void Update()
    {
        AnalyzeSurroundings();
    }

    private void AnalyzeSurroundings()
    {
        UpdateMap();

        IdentifyUnknowns();
    }

    private void UpdateMap()
    {
        LidarSamples lidarDistance = lidar.GetLidarDistances(currentHeading);
    }
}
// Enumerations
public enum CardinalDirection
{
    North,
    East,
    South,
    West
}

public enum EndingAction
{
    FireCanon,
    MoveForward,
    MoveBackward,
    TurnRight,
    TurnLeft
}

public enum blockType
{
    Ground,
    Unidentified,
    Wall,
    Hostile
}

public class Lidar
{
    public Lidar()
    {
    }

    Dictionary<CardinalDirection, int>  DirectionOffsets = new Dictionary<CardinalDirection, int>
        {
            {CardinalDirection.North, 0},
            {CardinalDirection.East, 1},
            {CardinalDirection.South, 2},
            {CardinalDirection.West, 3}
        };

    public LidarSamples GetLidarDistances(CardinalDirection currentHeading)
    {
        LidarSamples calculatedSamples = new LidarSamples();
        
        int[] distances = new int[] { API.LidarFront() , API.LidarRight(), API.LidarBack(), API.LidarLeft() };
        int[] shiftedDistances = new int[distances.Length];
        
        int frontOffset = DirectionOffsets[currentHeading];

        for (int i = 0; i < distances.Length; i++)
        {
            shiftedDistances[(i+frontOffset) % distances.Length] = distances[i];
        }

        calculatedSamples.distanceNorth = shiftedDistances[0];
        calculatedSamples.distanceEast = shiftedDistances[1];
        calculatedSamples.distanceSouth = shiftedDistances[2];
        calculatedSamples.distanceWest = shiftedDistances[3];

        return calculatedSamples;
    }
    public class LidarSamples
    {
        public int distanceNorth { get; set; }
        public int distanceEast { get; set; }
        public int distanceSouth { get; set; }
        public int distanceWest { get; set; }
    }
}