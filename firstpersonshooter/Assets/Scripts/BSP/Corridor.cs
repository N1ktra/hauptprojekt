using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor : Room
{
    public Corridor(RoomCoords coords, RoomDesign design) : base(coords, design) { }

    private bool instantiated = false;
    public override GameObject Instantiate()
    {
        if (instantiated)
        {
            //Debug.Log("corridor has already been instantiated");
            return null;
        }
        GameObject corridorContainer = new GameObject("Corridor");
        instantiateFloor().transform.SetParent(corridorContainer.transform, true);
        instantiated = true;
        return corridorContainer;
    }
}
