using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    private List<GameObject> path = new List<GameObject>();
    private List<GameObject> topTiles = new List<GameObject>();
    private List<GameObject> bottomTiles = new List<GameObject>();

    private int radius, currentTileIndex;
    private bool hasReachedX, hasReachedZ;
    private GameObject startingTile, endingTile;

    public List<GameObject> GetGeneratedPath => path;

    public Path(int worldRadius)
    {
        this.radius = worldRadius;
    }

    public void AssignTopAndBottomTiles(int z, GameObject tile)
    {
        if (z == 0)
            topTiles.Add(tile);

        if (z == radius - 1)
            bottomTiles.Add(tile);

        Debug.Log("Added Tile");
    }

    private bool AssignAndCheckStartingAndEndingTile()
    {
        var xIndex = Random.Range(0, topTiles.Count - 1);
        var zIndex = Random.Range(0, bottomTiles.Count);

        startingTile = topTiles[xIndex];
        endingTile = bottomTiles[zIndex];

        return startingTile != null && endingTile != null;
    }

    bool FurtherRandomization(int prevDirection, ref GameObject currentTile)
    {
        Debug.Log("Further Randomization Check");
        Debug.Log("Prev direction is " + prevDirection);

        bool doRandomize = Random.value > 0.2f;
        int verticalSteps = Random.Range(3, 5);

        if (!doRandomize) return false;
        Debug.Log("Randomize more");


        for (int i = 0; i < verticalSteps; i++)
        {
            int currentIndex = WorldGeneration.GeneratedTiles.IndexOf(currentTile);

            if (prevDirection == 0) // moving UP
            {
                int nextIndex = currentIndex + radius;

                //Stop if out of bounds
                Debug.Log("Generate additional path upward");
                if (nextIndex >= WorldGeneration.GeneratedTiles.Count)
                {
                    Debug.Log("Violate rule");
                    break;
                }
                MoveUp(ref currentTile);
            }
            else if (prevDirection == 1) // moving DOWN
            {
                int nextIndex = currentIndex - radius;

                Debug.Log("Generate additional path downward");
                //Stop if out of bounds
                if (nextIndex < 0)
                {
                    Debug.Log("Violate rule");
                    break;
                }

                MoveDown(ref currentTile);
            }
        }

        return true;
    }



    public void GeneratePath()
    {
        if (AssignAndCheckStartingAndEndingTile())
        {
            GameObject currentTile = startingTile;

            /*Randomize the number of left move taken by path during its first start */
            int randomizedStartOffset = Random.Range(3, 5);

            for (int i = 0; i < randomizedStartOffset; i++)
                MoveLeft(ref currentTile);

            var safteyBreakX = 0;
            int prevMovement = -1;

            while (!hasReachedX)
            {
                safteyBreakX++;
                if (safteyBreakX > 100)
                    break;

                //Store previous direction of vertical movement
                // 0 -> Move up
                // 1 -> Move down

                // We move vertically on our xAxis
                if (currentTile.transform.position.x > endingTile.transform.position.x)
                {
                    MoveDown(ref currentTile);
                    prevMovement = 1;
                }
                else if (currentTile.transform.position.x < endingTile.transform.position.x)
                {
                    MoveUp(ref currentTile);
                    prevMovement = 0;
                }
                else
                {
                    Debug.Log("X Axis Reached");
                    if (!FurtherRandomization(prevMovement, ref currentTile))
                        hasReachedX = true;
                }
            }

            var safteyBreakZ = 0;
            while (!hasReachedZ)
            {
                safteyBreakZ++;
                if (safteyBreakZ > 100)
                    break;

                // We move horizontally on our zAxis
                if (currentTile.transform.position.z > endingTile.transform.position.z)
                    MoveRight(ref currentTile);
                else if (currentTile.transform.position.z < endingTile.transform.position.z)
                    MoveLeft(ref currentTile);
                else
                    hasReachedZ = true;
            }
            path.Add(endingTile);
        }
    }

    private void MoveDown(ref GameObject currentTile)
    {
        path.Add(currentTile);
        currentTileIndex = WorldGeneration.GeneratedTiles.IndexOf(currentTile);
        int n = currentTileIndex - radius;
        currentTile = WorldGeneration.GeneratedTiles[n];
    }

    private void MoveUp(ref GameObject currentTile)
    {
        path.Add(currentTile);
        currentTileIndex = WorldGeneration.GeneratedTiles.IndexOf(currentTile);
        int n = currentTileIndex + radius;
        currentTile = WorldGeneration.GeneratedTiles[n];
    }
    private void MoveLeft(ref GameObject currentTile)
    {
        path.Add(currentTile);
        currentTileIndex = WorldGeneration.GeneratedTiles.IndexOf(currentTile);
        currentTileIndex++;
        currentTile = WorldGeneration.GeneratedTiles[currentTileIndex];
    }
    private void MoveRight(ref GameObject currentTile)
    {
        path.Add(currentTile);
        currentTileIndex = WorldGeneration.GeneratedTiles.IndexOf(currentTile);
        currentTileIndex--;
        currentTile = WorldGeneration.GeneratedTiles[currentTileIndex];
    }
}