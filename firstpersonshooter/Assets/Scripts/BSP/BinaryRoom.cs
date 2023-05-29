using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryRoom : Room
{
    private static int minWidth = 8;
    private static int maxWidth = 20;
    private static int minHeight = 8;
    private static int maxHeight = 20;

    private static int trimTiles = 1;
    private static int corridorMargin = 1;
    private static int minCorridorThickness = 2;

    private bool horizontalSplit;
    private bool verticalSplit;

    public BinaryRoom leftRoom { get; private set; }
    public BinaryRoom rightRoom { get; private set; }

    public BinaryRoom(int width, int height, GameObject tilePrefab) : this(0, width, height, 0, tilePrefab) { }

    public BinaryRoom(int left, int right, int top, int bottom, GameObject tilePrefab) : base(left, right, top, bottom, tilePrefab)
    {
        horizontalSplit = false;
        verticalSplit = false;

        leftRoom = null;
        rightRoom = null;

        if (GetHeight() < minHeight || GetWidth() < minWidth)
            Debug.LogError("Room is too small");
    }

    public bool IsLeaf()
    {
        return leftRoom == null && rightRoom == null;
        //return (horizontalSplit == false) && (verticalSplit == false);
    }

    #region Splitting
    public bool Split()
    {
        // Attempt random split
        float rand = Random.value;
        if (rand < 0.5f && GetWidth() >= 2 * minWidth)
        {
            VerticalSplit();
            return true;
        }
        else if (GetHeight() >= 2 * minHeight)
        {
            HorizontalSplit();
            return true;
        }

        // Force split if theres too much space.
        if (GetWidth() > maxWidth)
        {
            VerticalSplit();
            return true;
        }

        if (GetHeight() > maxHeight)
        {
            HorizontalSplit();
            return true;
        }

        return false;
    }

    private void VerticalSplit()
    {
        int splitLocation = Random.Range(left + minWidth - 1, right - minWidth + 1);
        leftRoom = new BinaryRoom(left, splitLocation, top, bottom, tilePrefab);
        rightRoom = new BinaryRoom(splitLocation + 1, right, top, bottom, tilePrefab);
        verticalSplit = true;
    }

    private void HorizontalSplit()
    {
        int splitLocation = Random.Range(bottom + minHeight - 1, top - minHeight + 1);
        leftRoom = new BinaryRoom(left, right, splitLocation, bottom, tilePrefab);
        rightRoom = new BinaryRoom(left, right, top, splitLocation + 1, tilePrefab);
        horizontalSplit = true;
    }
    #endregion

    #region show room
    public override GameObject CreateRoom()
    {
        if (IsLeaf())
        {
            return base.CreateRoom();
        }
        else
        {
            leftRoom.CreateRoom();
            rightRoom.CreateRoom();
            return null;
        }
    }

    public void Trim()
    {
        left += trimTiles;
        right -= trimTiles;
        top -= trimTiles;
        bottom += trimTiles;

        if (leftRoom != null)
        {
            leftRoom.Trim();
        }
        if (rightRoom != null)
        {
            rightRoom.Trim();
        }
    }
    #endregion

    #region Corridors
    public List<int> GetRightConnections()
    {
        List<int> connections = new List<int>();

        if (!IsLeaf())
        {
            if (rightRoom != null)
                connections.AddRange(rightRoom.GetRightConnections());
            if (horizontalSplit && leftRoom != null)
                connections.AddRange(leftRoom.GetRightConnections());
        }
        else
        {
            for (int y = bottom + corridorMargin; y <= top - corridorMargin; y++)
            {
                connections.Add(y);
            }
        }
        return connections;
    }
    public List<int> GetLeftConnections()
    {
        List<int> connections = new List<int>();

        if (!IsLeaf())
        {
            if (rightRoom != null)
                connections.AddRange(rightRoom.GetLeftConnections());
            if (horizontalSplit && leftRoom != null)
                connections.AddRange(leftRoom.GetLeftConnections());
        }
        else
        {
            for (int y = bottom + corridorMargin; y <= top - corridorMargin; y++)
            {
                connections.Add(y);
            }
        }
        return connections;
    }
    public List<int> GetTopConnections()
    {
        List<int> connections = new List<int>();

        if (!IsLeaf())
        {
            if (rightRoom != null)
                connections.AddRange(rightRoom.GetTopConnections());
            if (horizontalSplit && leftRoom != null)
                connections.AddRange(leftRoom.GetTopConnections());
        }
        else
        {
            for (int y = bottom + corridorMargin; y <= top - corridorMargin; y++)
            {
                connections.Add(y);
            }
        }
        return connections;
    }
    public List<int> GetBottomConnections()
    {
        List<int> connections = new List<int>();

        if (!IsLeaf())
        {
            if (rightRoom != null)
                connections.AddRange(rightRoom.GetBottomConnections());
            if (horizontalSplit && leftRoom != null)
                connections.AddRange(leftRoom.GetBottomConnections());
        }
        else
        {
            for (int y = bottom + corridorMargin; y <= top - corridorMargin; y++)
            {
                connections.Add(y);
            }
        }
        return connections;
    }
    #endregion
}
