using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public GameObject tilePrefab;
    public Vector3 tileSize;

    public int left { get; protected set; }
    public int right { get; protected set; }
    public int top { get; protected set; }
    public int bottom { get; protected set; }

    protected int GetWidth()
    {
        return right - left + 1;
    }

    protected int GetHeight()
    {
        return top - bottom + 1;
    }

    public Room(int width, int height, GameObject tilePrefab) : this(0, width, height, 0, tilePrefab) { }

    public Room(int left, int right, int top, int bottom, GameObject tilePrefab)
    {
        this.left = left;
        this.right = right;
        this.top = top;
        this.bottom = bottom;
        this.tilePrefab = tilePrefab;
        tileSize = tilePrefab.GetComponent<Renderer>().bounds.size;
    }

    public virtual GameObject CreateRoom()
    {
        GameObject roomContainer = new GameObject("Room");
        Color debugColor = Random.ColorHSV();

        for (int x = left; x <= right; x++)
        {
            for (int y = bottom; y <= top; y++)
            {

                GameObject tile = GameObject.Instantiate(tilePrefab);
                
                //Zu testzwecken:
                tile.GetComponent<SpriteRenderer>().material.color = debugColor;

                tile.transform.position = new Vector3(x * tileSize.x, y * tileSize.y, 0);
                tile.transform.SetParent(roomContainer.transform, true);
            }
        }

        return roomContainer;
    }

}

