using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor : Room
{
    public Corridor(Coords coords, Vector3 tileSize) : base(coords, tileSize) { }

    private bool instantiated = false;
    public override GameObject Instantiate(GameObject floorPrefab, GameObject wallPrefab)
    {
        if (instantiated)
        {
            //Debug.Log("corridor has already been instantiated");
            return null;
        }
        GameObject corridorContainer = new GameObject("Corridor");
        instantiateFloor(floorPrefab).transform.SetParent(corridorContainer.transform, true);
        instantiated = true;
        return corridorContainer;
    }
}
