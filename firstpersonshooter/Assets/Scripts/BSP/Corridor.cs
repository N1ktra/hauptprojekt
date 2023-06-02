using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor : Room
{
    public Corridor(RoomCoords coords, RoomDesign design, DIRECTION direction) : base(coords, design) 
    {
        this.direction = direction;
    }
    public DIRECTION direction;
    private bool isInstantiated = false;
    public override GameObject Instantiate()
    {
        if (isInstantiated)
        {
            //Debug.Log("corridor has already been instantiated");
            return null;
        }
        GameObject corridorContainer = new GameObject("Corridor");
        instantiateFloor().transform.SetParent(corridorContainer.transform, true);
        instantiateWalls().transform.SetParent(corridorContainer.transform, true);
        isInstantiated = true;
        return corridorContainer;
    }

    public GameObject instantiateWalls()
    {
        GameObject wallContainer = new GameObject("Wall");
        if(direction == DIRECTION.TOP || direction == DIRECTION.BOTTOM)
        {
            for (int z = coords.bottom; z <= coords.top; z++)
            {
                for (int y = 0; y < design.wallHeight; y++)
                {
                    addObject(design.wallPrefab, wallContainer, new Vector3(coords.left, y, z), Quaternion.Euler(0, 90, 0));
                    addObject(design.wallPrefab, wallContainer, new Vector3(coords.right, y, z), Quaternion.Euler(0, -90, 0));
                }
            }
        }
        if (direction == DIRECTION.LEFT || direction == DIRECTION.RIGHT)
        {
            for (int x = coords.left; x <= coords.right; x++)
            {
                for (int y = 0; y < design.wallHeight; y++)
                {
                    addObject(design.wallPrefab, wallContainer, new Vector3(x, y, coords.bottom));
                    addObject(design.wallPrefab, wallContainer, new Vector3(x, y, coords.top), Quaternion.Euler(0, 180, 0));
                }
            }
        }
        return wallContainer;
    }
}
