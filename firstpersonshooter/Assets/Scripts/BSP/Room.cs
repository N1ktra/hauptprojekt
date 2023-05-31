using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Room
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
        /// <summary>
        /// returns the middle positions as world position
        /// </summary>
        /// <returns></returns>
        public Vector3 toWorldPosition(Vector3 tileSize)
        {
            return new Vector3((left + right) / 2 * tileSize.x, 0, (top + bottom) / 2 * tileSize.z);
        }
    }
    public Coords coords;

    public Vector3 tileSize;

    public Room(Coords coords, Vector3 tileSize)
    {
        this.coords = coords;
        this.tileSize = tileSize;
    }

    public abstract GameObject Instantiate(GameObject floorPrefab, GameObject wallPrefab);

    public GameObject instantiateFloor(GameObject floorPrefab)
    {
        GameObject floorContainer = new GameObject("Floor");
        Color debugColor = Random.ColorHSV();

        for (int x = coords.left; x <= coords.right; x++)
        {
            for (int y = coords.bottom; y <= coords.top; y++)
            {
                GameObject tile = GameObject.Instantiate(floorPrefab);
                tile.transform.position = new Vector3(x * tileSize.x, 0, y * tileSize.z);
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

