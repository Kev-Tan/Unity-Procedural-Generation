using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public static List<GameObject> GeneratedTiles = new List<GameObject>();

    [SerializeField] private GameObject tilePrefab;

    private int radius = 20;

    // Start is called before the first frame update
    void Start()
    {
        Path pathGenerator = new Path(radius);

        for(int x = 0; x < radius; x++)
        {
            for(int z = 0; z < radius; z++)
            {
                GameObject tile = Instantiate(tilePrefab,
                    new Vector3(x * 1.5f, 0, z * 1.5f),
                    Quaternion.identity);

                GeneratedTiles.Add(tile);
                pathGenerator.AssignTopAndBottomTiles(z, tile);
            }
        }

        //World Generated
        pathGenerator.GeneratePath();

        GameObject startTile = pathGenerator.GetStartingTile;
        GameObject endTile = pathGenerator.GetEndingTile;

        foreach(var pObject in pathGenerator.GetGeneratedPath)
        {
            // Don't hide the starting and ending tiles
            if (pObject != startTile && pObject != endTile)
            {
                pObject.SetActive(false);
            }
        }
    }
}