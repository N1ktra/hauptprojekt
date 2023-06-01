using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    /// <summary>
    /// returns the middle positions as world position
    /// </summary>
    /// <returns></returns>
    public Vector3 toWorldPosition(Vector3 tileSize)
    {
        return new Vector3((left + right) / 2 * tileSize.x, 0, (top + bottom) / 2 * tileSize.z);
    }
}
public struct RoomDesign
{
    public Vector3 tileSize;
    public GameObject floorPrefab;
    public GameObject wallPrefab;

    //Width / Height
    public int minWidth;
    public int maxWidth;
    public int minHeight;
    public int maxHeight;
    public bool trimTilesIsRandom;
    public (int left, int right, int top, int bottom) trimTiles;
    public int minTrimTiles;
    public int maxTrimTiles;

    //Corridors
    public int corridorMargin;
    public int maxCorridorThickness;

    public RoomDesign(Vector3 tileSize, GameObject floorPrefab, GameObject wallPrefab, 
        int minWidth, int maxWidth, int minHeight, int maxHeight, 
        bool trimTilesIsRandom, (int left, int right, int top, int bottom) trimTiles, int minTrimTiles, int maxTrimTiles, 
        int corridorMargin, int maxCorridorThickness)
    {
        this.tileSize = tileSize;
        this.floorPrefab = floorPrefab;
        this.wallPrefab = wallPrefab;
        this.minWidth = minWidth;
        this.maxWidth = maxWidth;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
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

    public abstract GameObject Instantiate();

    public GameObject instantiateFloor()
    {
        GameObject floorContainer = new GameObject("Floor");
        Color debugColor = UnityEngine.Random.ColorHSV();

        for (int x = coords.left; x <= coords.right; x++)
        {
            for (int y = coords.bottom; y <= coords.top; y++)
            {
                GameObject tile = GameObject.Instantiate(design.floorPrefab);
                tile.transform.position = new Vector3(x * design.tileSize.x, 0, y * design.tileSize.z);
                tile.transform.SetParent(floorContainer.transform, true);
                //Zu testzwecken:
                tile.GetComponent<Renderer>().material.color = debugColor;
            }
        }
        return floorContainer;
    }

    public void addObject(GameObject obj, Vector3 position)
    {
        GameObject.Instantiate(obj, position, Quaternion.identity);
    }

}

