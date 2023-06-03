using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DIRECTION { LEFT, RIGHT, TOP, BOTTOM };
public struct RoomCoords
{
    public int left;
    public int right;
    public int top;
    public int bottom;

    public RoomCoords(int left, int right, int top, int bottom)
    {
        this.left = left;
        this.right = right;
        this.top = top; 
        this.bottom = bottom;
    }
    public RoomCoords(int width, int height) : this(0, width - 1, height - 1, 0) { }

    public RoomCoords Copy()
    {
        return this;
    }

    public int GetWidth()
    {
        return right - left + 1;
    }
    public int GetHeight()
    {
        return top - bottom + 1;
    }
    public RoomCoords setLeft(int left)
    {
        this.left = left;
        return this;
    }
    public RoomCoords setRight(int right)
    {
        this.right = right;
        return this;
    }
    public RoomCoords setTop(int top)
    {
        this.top = top;
        return this;
    }
    public RoomCoords setBootom(int bottom)
    {
        this.bottom = bottom;
        return this;
    }

    public Vector3 getCenterPosition()
    {
        return new Vector3((left + right) / 2, 0, (top + bottom) / 2);
    }

    public bool ContainsX(int value)
    {
        return left <= value && value <= right;
    }
    public bool ContainsY(int value)
    {
        return bottom <= value && value <= top;
    }
}
public struct RoomDesign
{
    //Prefabs
    public Vector3 tileSize;
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject corridorEntrancePrefab;
    public GameObject torchPrefab;
    public int torchPadding;

    //Width / Height
    public int minWidth;
    public int maxWidth;
    public int minHeight;
    public int maxHeight;
    public int wallHeight;
    public bool trimTilesIsRandom;
    public (int left, int right, int top, int bottom) trimTiles;
    public int minTrimTiles;
    public int maxTrimTiles;

    //Corridors
    public int corridorMargin;
    public int maxCorridorThickness;

    public RoomDesign(Vector3 tileSize,
        GameObject floorPrefab, GameObject wallPrefab, GameObject corridorEntrancePrefab, GameObject torchPrefab,
        int torchPadding,
        int minWidth, int maxWidth, int minHeight, int maxHeight, int wallHeight,
        bool trimTilesIsRandom, (int left, int right, int top, int bottom) trimTiles, int minTrimTiles, int maxTrimTiles,
        int corridorMargin, int maxCorridorThickness)
    {
        this.tileSize = tileSize;
        this.floorPrefab = floorPrefab;
        this.wallPrefab = wallPrefab;
        this.corridorEntrancePrefab = corridorEntrancePrefab;
        this.torchPrefab = torchPrefab;
        this.torchPadding = torchPadding;
        this.minWidth = minWidth;
        this.maxWidth = maxWidth;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        this.wallHeight = wallHeight;
        this.trimTilesIsRandom = trimTilesIsRandom;
        this.trimTiles = trimTiles;
        this.minTrimTiles = minTrimTiles;
        this.maxTrimTiles = maxTrimTiles;
        this.corridorMargin = corridorMargin;
        this.maxCorridorThickness = maxCorridorThickness;
    }
    public RoomDesign Copy()
    {
        return this;
    }

    public RoomDesign setTrimTiles((int left, int right, int top, int bottom) trimTiles)
    {
        this.trimTiles = trimTiles;
        return this;
    }

    public RoomDesign setRandomTrimTiles()
    {
        this.trimTiles.left = UnityEngine.Random.Range(minTrimTiles, maxTrimTiles);
        this.trimTiles.right = UnityEngine.Random.Range(minTrimTiles, maxTrimTiles);
        this.trimTiles.top = UnityEngine.Random.Range(minTrimTiles, maxTrimTiles);
        this.trimTiles.bottom = UnityEngine.Random.Range(minTrimTiles, maxTrimTiles);
        return this;
    }
}
public abstract class Room
{
    public RoomCoords coords;
    public RoomDesign design;

    public Room(RoomCoords coords, RoomDesign design)
    {
        this.coords = coords;
        this.design = design;
    }

    /// <summary>
    /// Instantiates an Object at the given Position (in Room coordinates)
    /// </summary>
    /// <param name="obj">The Object to instantiate</param>
    /// <param name="roomCoords">The position given in Room Coordinate Space</param>
    /// <param name="rotation">The Rotation of the object. Standard is Quaternion.identity</param>
    public GameObject addObject(GameObject obj, GameObject parent, Vector3 roomCoords, Quaternion? rotation = null)
    {
        Quaternion rot = rotation ?? Quaternion.identity;
        if (parent == null)
            return GameObject.Instantiate(obj, Vector3.Scale(roomCoords, design.tileSize), rot);
        else
            return GameObject.Instantiate(obj, Vector3.Scale(roomCoords, design.tileSize), rot, parent.transform);
    }
    public List<GameObject> addObjects(List<GameObject> objs, GameObject parent, Vector3 roomCoords, Quaternion? rotation = null)
    {
        List<GameObject> list = new List<GameObject>();
        foreach (GameObject obj in objs)
        {
            list.Add(addObject(obj, parent, roomCoords, rotation));
        }
        return list;
    }

    public abstract GameObject Instantiate();

    public GameObject instantiateFloor()
    {
        GameObject floorContainer = new GameObject("Floor");
        for (int x = coords.left; x <= coords.right; x++)
        {
            for (int z = coords.bottom; z <= coords.top; z++)
            {
                addObject(design.floorPrefab, floorContainer, new Vector3(x, 0, z));
            }
        }
        return floorContainer;
    }

    public GameObject instantiateCeiling()
    {
        GameObject ceilingContainer = new GameObject("Ceiling");
        for (int x = coords.left; x <= coords.right; x++)
        {
            for (int z = coords.bottom; z <= coords.top; z++)
            {
                addObject(design.floorPrefab, ceilingContainer, new Vector3(x, design.wallHeight, z), Quaternion.Euler(180, 0, 0));
            }
        }
        return ceilingContainer;
    }

}

