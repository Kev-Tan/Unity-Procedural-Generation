using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// X top, Z left;

public class Path
{
    private List<GameObject> path = new List<GameObject>();
    private List<GameObject> topTiles = new List<GameObject>();
    private List<GameObject> bottomTiles = new List<GameObject>();

    private int radius, currentTileIndex;
    private bool hasReachedX, hasReachedZ;
    private GameObject startingTile, endingTile;

    public List<GameObject> GetGeneratedPath => path;
    public GameObject GetStartingTile => startingTile;
    public GameObject GetEndingTile => endingTile;

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

    void SetTileColor(GameObject tile, Color color)
    {
        Renderer renderer = tile.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }




    private bool AssignAndCheckStartingAndEndingTile()
    {
        var xIndex = Random.Range(0, topTiles.Count - 1);
        var zIndex = Random.Range(0, bottomTiles.Count);

        startingTile = topTiles[xIndex];
        endingTile = bottomTiles[zIndex];

        SetTileColor(startingTile, Color.green);  // START
        SetTileColor(endingTile, Color.red);      // END

        return startingTile != null && endingTile != null;
    }

    bool FurtherRandomization(int prevDirection, ref GameObject currentTile)
    {

        bool doRandomize = Random.value > 0.2f;
        int verticalSteps = Random.Range(3, 8);

        if (!doRandomize) return false;


        for (int i = 0; i < verticalSteps; i++)
        {
            int currentIndex = WorldGeneration.GeneratedTiles.IndexOf(currentTile);

            if (prevDirection == 0) // moving UP
            {
                int nextIndex = currentIndex + radius;

                //Stop if out of bounds
                Debug.Log("Randomize: Generate additional path upward");
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

            // Clamp the offset to prevent going out of bounds
            int currentZ = WorldGeneration.GeneratedTiles.IndexOf(currentTile) % radius;
            int maxLeftMoves = radius - 1 - currentZ; // How many tiles are to the left
            randomizedStartOffset = Mathf.Min(randomizedStartOffset, maxLeftMoves);

            for (int i = 0; i < randomizedStartOffset; i++)
                MoveLeft(ref currentTile);

            hasReachedX = false;
            hasReachedZ = false; 

            int globalSafety = 0;

            while (!hasReachedX && !hasReachedZ)
            {
                globalSafety++;
                if (globalSafety > 1000)
                {
                    Debug.LogError("Global safety break - infinite loop detected");
                    break;
                }

                Debug.Log("Inside while loop");
                // ----- X MOVEMENT -----
                var safetyX = 0;
                while (!hasReachedX)
                {
                    safetyX++;
                    if (safetyX > 100)
                    {
                        Debug.LogError("Safety X break");
                        break;
                    }

                    if (currentTile.transform.position.x > endingTile.transform.position.x)
                    {
                        MoveDown(ref currentTile);
                        Debug.Log("Move down");
                    }
                    else if (currentTile.transform.position.x < endingTile.transform.position.x)
                    {
                        MoveUp(ref currentTile);
                        Debug.Log("Move up");
                    }
                    else
                    {
                        hasReachedX = true;
                    }
                }

                // ----- Z MOVEMENT -----
                int counter = 0;
                int safetyZ = 0;

                while (!hasReachedZ)
                {
                    safetyZ++;
                    if (safetyZ > 100)
                    {
                        Debug.LogError("Safety Z break");
                        break;
                    }

                    int dir = Random.Range(0, 2);

                    if (counter > 3 && FurtherRandomization(dir, ref currentTile))
                    {
                        // 🔁 GO BACK TO X LOOP
                        hasReachedX = false;
                        counter = 0;
                        int randomNum = Random.Range(5, 7);
                        
                        // Check bounds before moving left
                        int currentIndex = WorldGeneration.GeneratedTiles.IndexOf(currentTile);
                        int currentZPos = currentIndex % radius;
                        int maxMoves = radius - 1 - currentZPos;
                        randomNum = Mathf.Min(randomNum, maxMoves);
                        
                        for (int i = 0; i < randomNum; i++)
                        {
                            MoveLeft(ref currentTile);  
                        }
                        break;
                    }

                    if (currentTile.transform.position.z > endingTile.transform.position.z)
                    {
                        MoveRight(ref currentTile);
                        Debug.Log("Move right");
                        counter++;
                    }
                    else if (currentTile.transform.position.z < endingTile.transform.position.z)
                    {
                        MoveLeft(ref currentTile);
                        Debug.Log("Move left");
                        counter++;
                    }
                    else if (currentTile.transform.position.z == endingTile.transform.position.z)
                        hasReachedZ = true;
                }

                // Break if both safety checks triggered
                if (safetyZ > 100 || safetyX > 100)
                    break;
            }
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