using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public struct Coords
    {
        public int left;
        public int right;
        public int top;
        public int bottom;

        public Coords(int left, int right, int top, int bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top; 
            this.bottom = bottom;
        }
        public Coords(int width, int height) : this(0, width - 1, height - 1, 0) { }

        public int GetWidth()
        {
            return right - left + 1;
        }
        public int GetHeight()
        {
            return top - bottom + 1;
        }
        public Coords withLeft(int left)
        {
            return new Coords(left, right, top, bottom);
        }
        public Coords withRight(int right)
        {
            return new Coords(left, right, top, bottom);
        }
        public Coords withTop(int top)
        {
            return new Coords(left, right, top, bottom);
        }
        public Coords withBottom(int bottom)
        {
            return new Coords(left, right, top, bottom);
        }
    }

    public GameObject tilePrefab;
    public Vector3 tileSize;

    public Coords coords;

    public Room(Coords coords, GameObject tilePrefab)
    {
        this.coords = coords;
        this.tilePrefab = tilePrefab;
        this.tileSize = tilePrefab.GetComponent<Renderer>().bounds.size;
    }

    public virtual GameObject Instantiate()
    {
        GameObject roomContainer = new GameObject("Room");
        Color debugColor = Random.ColorHSV();

        for (int x = coords.left; x <= coords.right; x++)
        {
            for (int y = coords.bottom; y <= coords.top; y++)
            {
                GameObject tile = GameObject.Instantiate(tilePrefab);
                tile.transform.position = new Vector3(x * tileSize.x, 0, y * tileSize.z);
                tile.transform.SetParent(roomContainer.transform, true);
                //Zu testzwecken:
                tile.GetComponent<Renderer>().material.color = debugColor;
            }
        }

        return roomContainer;
    }

}

