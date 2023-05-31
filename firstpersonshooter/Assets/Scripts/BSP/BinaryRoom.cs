using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public class BinaryRoom : Room
{
    [Header("Width / Height")]
    private static int minWidth = 8;
    private static int maxWidth = 20;
    private static int minHeight = 8;
    private static int maxHeight = 20;
    private static int trimTiles = 1;

    [Header("Corridors")]
    private static int corridorMargin = 1;
    private static int minCorridorThickness = 2;
    public List<Corridor> corridors { get; private set; } = new List<Corridor>();

    [Header("Splitting")]
    private bool horizontalSplit;
    private bool verticalSplit;
    public BinaryRoom leftRoom { get; private set; }
    public BinaryRoom rightRoom { get; private set; }

    public BinaryRoom(Coords coords, GameObject tilePrefab) : base(coords, tilePrefab)
    {
        horizontalSplit = false;
        verticalSplit = false;

        leftRoom = null;
        rightRoom = null;

        if (coords.GetHeight() < minHeight || coords.GetWidth() < minWidth)
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
        if (rand < 0.5f && coords.GetWidth() >= 2 * minWidth)
        {
            VerticalSplit();
            return true;
        }
        else if (coords.GetHeight() >= 2 * minHeight)
        {
            HorizontalSplit();
            return true;
        }

        // Force split if theres too much space.
        if (coords.GetWidth() > maxWidth)
        {
            VerticalSplit();
            return true;
        }

        if (coords.GetHeight() > maxHeight)
        {
            HorizontalSplit();
            return true;
        }

        return false;
    }

    private void VerticalSplit()
    {
        int splitLocation = Random.Range(coords.left + minWidth - 1, coords.right - minWidth + 1);
        leftRoom = new BinaryRoom(coords.withRight(splitLocation), tilePrefab);
        rightRoom = new BinaryRoom(coords.withLeft(splitLocation + 1), tilePrefab);
        verticalSplit = true;
    }

    private void HorizontalSplit()
    {
        int splitLocation = Random.Range(coords.bottom + minHeight - 1, coords.top - minHeight + 1);
        leftRoom = new BinaryRoom(coords.withTop(splitLocation), tilePrefab);
        rightRoom = new BinaryRoom(coords.withBottom(splitLocation + 1), tilePrefab);
        horizontalSplit = true;
    }
    #endregion

    #region show room
    public override GameObject Instantiate()
    {
        if (IsLeaf())
        {
            foreach(Corridor corridor in corridors)
            {
                corridor.Instantiate();
            }
            return base.Instantiate();
        }
        else
        {
            leftRoom.Instantiate();
            rightRoom.Instantiate();
            return null;
        }
    }

    public void Trim()
    {
        coords.left += trimTiles;
        coords.right -= trimTiles;
        coords.top -= trimTiles;
        coords.bottom += trimTiles;

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
    /// <summary>
    /// returns all positions where a corridor can be added to the right
    /// </summary>
    /// <returns></returns>
    public List<(int, BinaryRoom)> GetRightConnections()
    {
        List<(int, BinaryRoom)> connections = new List<(int, BinaryRoom)>();

        if (!IsLeaf())
        {
            if (rightRoom != null)
                connections.AddRange(rightRoom.GetRightConnections());
            if (horizontalSplit && leftRoom != null)
                connections.AddRange(leftRoom.GetRightConnections());
        }
        else
        {
            for (int y = coords.bottom + corridorMargin + trimTiles; y <= coords.top - corridorMargin - trimTiles; y++)
            {
                connections.Add((y, this));
            }
        }
        return connections;
    }
    /// <summary>
    /// returns all positions where a corridor can be added to the left
    /// </summary>
    /// <returns></returns>
    public List<(int, BinaryRoom)> GetLeftConnections()
    {
        List<(int, BinaryRoom)> connections = new List<(int, BinaryRoom)>();

        if (!IsLeaf())
        {
            if (leftRoom != null)
                connections.AddRange(leftRoom.GetLeftConnections());
            if (horizontalSplit && rightRoom != null)
                connections.AddRange(rightRoom.GetLeftConnections());
        }
        else
        {
            for (int y = coords.bottom + corridorMargin + trimTiles; y <= coords.top - corridorMargin - trimTiles; y++)
            {
                connections.Add((y, this));
            }
        }
        return connections;
    }
    /// <summary>
    /// returns all positions where a corridor can be added to the top
    /// </summary>
    /// <returns></returns>
    public List<(int, BinaryRoom)> GetTopConnections()
    {
        List<(int, BinaryRoom)> connections = new List<(int, BinaryRoom)>();

        if (!IsLeaf())
        {
            if (rightRoom != null)
                connections.AddRange(rightRoom.GetTopConnections());
            if (verticalSplit && leftRoom != null)
                connections.AddRange(leftRoom.GetTopConnections());
        }
        else
        {
            for (int x = coords.left + corridorMargin + trimTiles; x <= coords.right - corridorMargin - trimTiles; x++)
            {
                connections.Add((x, this));
            }
        }
        return connections;
    }
    /// <summary>
    /// returns all positions where a corridor can be added to the bottom
    /// </summary>
    /// <returns></returns>
    public List<(int, BinaryRoom)> GetBottomConnections()
    {
        List<(int, BinaryRoom)> connections = new List<(int, BinaryRoom)>();

        if (!IsLeaf())
        {
            if (leftRoom != null)
                connections.AddRange(leftRoom.GetBottomConnections());
            if (verticalSplit && rightRoom != null)
                connections.AddRange(rightRoom.GetBottomConnections());
        }
        else
        {
            for (int x = coords.left + corridorMargin + trimTiles; x <= coords.right - corridorMargin - trimTiles; x++)
            {
                connections.Add((x, this));
            }
        }
        return connections;
    }
    public List<(BinaryRoom, DIRECTION)> neighborList = new List<(BinaryRoom, DIRECTION)>();
    public enum DIRECTION {LEFT, RIGHT, TOP, BOTTOM};
    public void addNeighborRoom(BinaryRoom room, DIRECTION direction)
    {
        neighborList.Add((room, direction));
    }
    public void createNeighborList()
    {
        if (IsLeaf())
            return;

        if (leftRoom != null)
        {
            leftRoom.createNeighborList();
        }
        if (rightRoom != null)
        {
            rightRoom.createNeighborList();
        }
        if (leftRoom != null && rightRoom != null)
        {
            if (verticalSplit)
            {
                var leftPoints = leftRoom.GetRightConnections();
                var rightPoints = rightRoom.GetLeftConnections();
                var neighborRooms = leftPoints.Join(
                    rightPoints,
                    leftPoint => leftPoint.Item1,
                    rightPoint => rightPoint.Item1,
                    (leftPoint, rightPoint) => (leftPoint.Item2, rightPoint.Item2)
                    );
                Debug.Log(neighborRooms);
                foreach(var roomPair in neighborRooms)
                {
                    roomPair.Item1.addNeighborRoom(roomPair.Item2, DIRECTION.RIGHT);
                    roomPair.Item2.addNeighborRoom(roomPair.Item1, DIRECTION.LEFT);
                }
            }
            else 
            {
                var leftPoints = leftRoom.GetTopConnections();
                var rightPoints = rightRoom.GetBottomConnections();
                var neighborRooms = leftPoints.Join(
                    rightPoints,
                    leftPoint => leftPoint.Item1,
                    rightPoint => rightPoint.Item1,
                    (leftPoint, rightPoint) => (leftPoint.Item2, rightPoint.Item2)
                    );
                Debug.Log(neighborRooms);
                foreach (var roomPair in neighborRooms)
                {
                    roomPair.Item1.addNeighborRoom(roomPair.Item2, DIRECTION.TOP);
                    roomPair.Item2.addNeighborRoom(roomPair.Item1, DIRECTION.BOTTOM);
                }
            }
        }
    }

    /// <summary>
    /// Fügt Korridore hinzu (Davor muss jedoch einmal createNeighborList() aufgerufen worden sein)
    /// </summary>
    public void AddCorridors()
    {
        if (IsLeaf())
        {
            foreach(var neighbor in neighborList)
            {
                Room room = neighbor.Item1;
                DIRECTION dir = neighbor.Item2;
                Coords corridorCoords = new Coords();
                switch (dir)
                {
                    case DIRECTION.LEFT:
                        corridorCoords = new Coords(
                            room.coords.right + 1,
                            coords.left - 1,
                            Mathf.Min(coords.top, room.coords.top) - trimTiles - corridorMargin,
                            Mathf.Max(coords.bottom, room.coords.bottom) + trimTiles + corridorMargin
                            );
                        break;
                    case DIRECTION.RIGHT:
                        corridorCoords = new Coords(
                            coords.right + 1,
                            room.coords.left - 1,
                            Mathf.Min(coords.top, room.coords.top) - trimTiles - corridorMargin,
                            Mathf.Max(coords.bottom, room.coords.bottom) + trimTiles + corridorMargin
                            );
                        break;
                    case DIRECTION.TOP:
                        corridorCoords = new Coords(
                            Mathf.Max(coords.left, room.coords.left) + trimTiles + corridorMargin,
                            Mathf.Min(coords.right, room.coords.right) - trimTiles - corridorMargin,
                            room.coords.bottom - 1,
                            coords.top + 1
                            );
                        break;
                    case DIRECTION.BOTTOM:
                        corridorCoords = new Coords(
                            Mathf.Max(coords.left, room.coords.left) + trimTiles + corridorMargin,
                            Mathf.Min(coords.right, room.coords.right) - trimTiles - corridorMargin,
                            coords.bottom - 1,
                            room.coords.top + 1
                            );
                        break;
                }
                corridors.Add(new Corridor(corridorCoords, tilePrefab));
            }
        }

        if (leftRoom != null)
        {
            leftRoom.AddCorridors();
        }
        if (rightRoom != null)
        {
            rightRoom.AddCorridors();
        }

    }

    /// <summary>
    /// returns the intersection between the connections of the two areas we want to connect
    /// </summary>
    /// <param name="points"></param>
    /// <returns>the min/max points of the group in a Vector2</returns>
    //private List<(int min, int max)> GetIntersectionGroups(List<int> points)
    //{
    //    List<(int min, int max)> groups = new List<(int, int)>();

    //    bool firstTime = true;
    //    (int min, int max) currentGroup = (0, 0);
    //    for (int i = 0; i < points.Count; i++)
    //    {
    //        int num = points[i];

    //        if (firstTime || points[i - 1] != points[i] - 1)
    //        {
    //            if (!firstTime)
    //            {
    //                groups.Add(currentGroup);
    //            }

    //            firstTime = false;
    //            currentGroup = (num, num);
    //        }
    //        else
    //        {
    //            currentGroup.max += 1;
    //        }
    //    }

    //    if (!firstTime)
    //        groups.Add(currentGroup);

    //    return groups.Where(g => g.max - g.min >= minCorridorThickness).ToList();
    //}

    //public void AddCorridors()
    //{
        //if (IsLeaf())
        //    return;

        //if (leftRoom != null)
        //{
        //    leftRoom.AddCorridors();
        //}
        //if (rightRoom != null)
        //{
        //    rightRoom.AddCorridors();
        //}

        //if (leftRoom != null && rightRoom != null)
        //{
        //    if (verticalSplit)
        //    {
        //        var positions = leftRoom.GetRightConnections().Intersect(rightRoom.GetLeftConnections()).ToList();
        //        var groups = GetIntersectionGroups(positions);
        //        if (groups.Count > 0)
        //        {
        //            var pair = groups[Random.Range(0, groups.Count)];
        //            Coords corridorCoords = new Coords(leftRoom.coords.right + 1, rightRoom.coords.left - 1, pair.max, pair.min);
        //            corridor = new Corridor(corridorCoords, tilePrefab);
        //        }
        //    }
        //    else
        //    {
        //        var positions = leftRoom.GetTopConnections().Intersect(rightRoom.GetBottomConnections()).ToList();
        //        var groups = GetIntersectionGroups(positions);
        //        if (groups.Count > 0)
        //        {
        //            var pair = groups[Random.Range(0, groups.Count)];
        //            Coords corridorCoords = new Coords(pair.min, pair.max, rightRoom.coords.bottom - 1, leftRoom.coords.top + 1);
        //            corridor = new Corridor(corridorCoords, tilePrefab);
        //        }
        //    }
        //}
    //}
    #endregion
}
