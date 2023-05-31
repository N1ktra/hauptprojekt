using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class BinaryRoom : Room
{
    public enum DIRECTION { LEFT, RIGHT, TOP, BOTTOM };
    //Datastructure
    public BinaryRoom leftRoom { get; private set; }
    public BinaryRoom rightRoom { get; private set; }
    private HashSet<(BinaryRoom, DIRECTION)> neighborList = new HashSet<(BinaryRoom, DIRECTION)>();
    private HashSet<BinaryRoom> _allRooms = new HashSet<BinaryRoom>();
    public HashSet<BinaryRoom> allRooms 
    { 
        get 
        {
            if (_allRooms.Count == 0)
                _allRooms = getAllRooms();
            return _allRooms;
        } 
    }

    //Width / Height
    private int minWidth = 10;
    private int maxWidth = 25;
    private int minHeight = 10;
    private int maxHeight = 25;
    private int trimTiles = 1;

    //Corridors
    private int corridorMargin = 1;
    private int maxCorridorThickness = 5;
    public List<Corridor> corridors { get; private set; } = new List<Corridor>();

    //Splitting
    private bool horizontalSplit;
    private bool verticalSplit;
    

    public BinaryRoom(Coords coords, Vector3 tileSize) : base(coords, tileSize)
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
    }

    private HashSet<BinaryRoom> getAllRooms()
    {
        HashSet<BinaryRoom> allRooms = new HashSet<BinaryRoom>();
        if (IsLeaf())
            allRooms.Add(this);

        if (leftRoom != null)
            allRooms.AddRange(leftRoom.getAllRooms());
        if (rightRoom != null)
            allRooms.AddRange(rightRoom.getAllRooms());

        return allRooms;
    }

    #region Splitting
    public bool Split()
    {
        // Attempt random split
        float rand = Random.value;
        if (rand < 0.5f && coords.GetWidth() >= 2 * minWidth + 4 * trimTiles)
        {
            VerticalSplit();
            return true;
        }
        else if (coords.GetHeight() >= 2 * minHeight + 4 * trimTiles)
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
        int splitLocation = Random.Range(coords.left + minWidth + 2 * trimTiles - 1, coords.right - minWidth - 2 * trimTiles + 1);
        leftRoom = new BinaryRoom(coords.withRight(splitLocation), tileSize);
        rightRoom = new BinaryRoom(coords.withLeft(splitLocation + 1), tileSize);
        verticalSplit = true;
    }

    private void HorizontalSplit()
    {
        int splitLocation = Random.Range(coords.bottom + minHeight + 2 * trimTiles - 1, coords.top - minHeight - 2 * trimTiles + 1);
        leftRoom = new BinaryRoom(coords.withTop(splitLocation), tileSize);
        rightRoom = new BinaryRoom(coords.withBottom(splitLocation + 1), tileSize);
        horizontalSplit = true;
    }
    #endregion

    #region show room
    public override GameObject Instantiate(GameObject floorPrefab, GameObject wallPrefab)
    {
        if(IsLeaf())
        {
            GameObject roomContainer = new GameObject("Room");
            foreach (Corridor corridor in corridors)
            {
                corridor.Instantiate(floorPrefab, wallPrefab)?.transform.SetParent(roomContainer.transform, true);
            }
            instantiateFloor(floorPrefab).transform.SetParent(roomContainer.transform, true);
            instantiateWalls(wallPrefab).transform.SetParent(roomContainer.transform, true);
            return roomContainer;
        }
        else
        {
            leftRoom.Instantiate(floorPrefab, wallPrefab);
            rightRoom.Instantiate(floorPrefab, wallPrefab);
            return null;
        }
    }


    private GameObject instantiateWalls(GameObject wallPrefab)
    {
        int wallHeight = 2;
        GameObject WallContainer = new GameObject("Wall");
        Vector3 wallTileSize = new Vector3(4, 3, 1);
        //bootom Wall
        for (int x = coords.left; x <= coords.right; x++)
        {
            bool isCorridor = false;
            foreach(Corridor corridor in corridors)
            {
                if (corridor.coords.top == coords.bottom - 1  && (corridor.coords.left <= x && x <= corridor.coords.right))
                {
                    isCorridor = true;
                    break;
                }
            }
            if (!isCorridor)
            {
                for (int y = 0; y < wallHeight; y++)
                {
                    GameObject tile = GameObject.Instantiate(wallPrefab);
                    tile.transform.position = new Vector3(x * tileSize.x, y * wallTileSize.y, tileSize.z * coords.bottom);
                    tile.transform.SetParent(WallContainer.transform, true);
                }
            }
        }
        //top Wall
        for (int x = coords.left; x <= coords.right; x++)
        {
            bool isCorridor = false;
            foreach (Corridor corridor in corridors)
            {
                if (corridor.coords.bottom == coords.top + 1 && (corridor.coords.left <= x && x <= corridor.coords.right))
                {
                    isCorridor = true;
                    break;
                }
            }
            if (!isCorridor)
            {
                for (int y = 0; y < wallHeight; y++)
                {
                    GameObject tile = GameObject.Instantiate(wallPrefab);
                    tile.transform.position = new Vector3(x * tileSize.x, y * wallTileSize.y, tileSize.z * (coords.top + 1));
                    tile.transform.SetParent(WallContainer.transform, true);
                }
            }
        }
        //left Wall
        for (int z = coords.bottom; z <= coords.top; z++)
        {
            bool isCorridor = false;
            foreach (Corridor corridor in corridors)
            {
                if (corridor.coords.right == coords.left - 1 && (corridor.coords.bottom <= z && z <= corridor.coords.top))
                {
                    isCorridor = true;
                    break;
                }
            }
            if (!isCorridor)
            {
                for (int y = 0; y < wallHeight; y++)
                {
                    GameObject tile = GameObject.Instantiate(wallPrefab);
                    tile.transform.rotation = Quaternion.Euler(0, 90, 0);
                    tile.transform.position = new Vector3(tileSize.x * coords.left, y * wallTileSize.y, tileSize.z * z);
                    tile.transform.SetParent(WallContainer.transform, true);
                }
            }
        }
        //right Wall
        for (int z = coords.bottom; z <= coords.top; z++)
        {
            bool isCorridor = false;
            foreach (Corridor corridor in corridors)
            {
                if (corridor.coords.left == coords.right + 1 && (corridor.coords.bottom <= z && z <= corridor.coords.top))
                {
                    isCorridor = true;
                    break;
                }
            }
            if (!isCorridor)
            {
                for (int y = 0; y < wallHeight; y++)
                {
                    GameObject tile = GameObject.Instantiate(wallPrefab);
                    tile.transform.rotation = Quaternion.Euler(0, -90, 0);
                    tile.transform.position = new Vector3(tileSize.x * coords.right, y * wallTileSize.y, tileSize.z * z);
                    tile.transform.SetParent(WallContainer.transform, true);
                }
            }
        }
        return WallContainer;
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
                    ).Distinct();
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
                    ).Distinct();
                foreach (var roomPair in neighborRooms)
                {
                    roomPair.Item1.addNeighborRoom(roomPair.Item2, DIRECTION.TOP);
                    roomPair.Item2.addNeighborRoom(roomPair.Item1, DIRECTION.BOTTOM);
                }
            }
        }
    }

    /// <summary>
    /// Fügt Korridore hinzu (Neighbor Liste muss vorher erstellt worden sein)
    /// </summary>
    public (BinaryRoom startRoom, BinaryRoom endRoom) AddCorridors()
    {
        HashSet<BinaryRoom> visitedRooms = new HashSet<BinaryRoom>();
        //choose random starting Room
        BinaryRoom startRoom = allRooms.ElementAt(Random.Range(0, allRooms.Count));
        BinaryRoom currentRoom = startRoom;
        visitedRooms.Add(currentRoom);

        int iterations = 0;
        while(visitedRooms.Count < allRooms.Count)
        {
            if(iterations >= 1000)
            {
                Debug.LogWarning("Maximum number of iterations reached!");
                return (null, null);
            }
            //choose random neighbor
            //if all neighbors have already been visited
            if(visitedRooms.Intersect(currentRoom.neighborList.Select(_ => _.Item1)).Count() == currentRoom.neighborList.Count)
            {
                //go to a previously visited room
                var possibleRooms = allRooms.Where(_ => _.neighborList.Select(_ => _.Item1).Contains(currentRoom));
                currentRoom = possibleRooms.ElementAt(Random.Range(0, possibleRooms.Count()));
            }
            else
            {
                var possibleNeighbors = currentRoom.neighborList.Where(_ => !visitedRooms.Contains(_.Item1));
                var neighbor = possibleNeighbors.ElementAt(Random.Range(0, possibleNeighbors.Count()));
                BinaryRoom room = neighbor.Item1;
                DIRECTION dir = neighbor.Item2;
                if (!visitedRooms.Contains(room))
                {
                    currentRoom.AddCorridorToNeighbor(room, dir);
                    visitedRooms.Add(room);
                    currentRoom = room;
                }
            }
            iterations++;
        }
        return (startRoom, currentRoom);

    }
    public void AddCorridorToNeighbor(BinaryRoom room, DIRECTION dir)
    {
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
        //limit corridor size
        while (corridorCoords.GetWidth() - 2 > maxCorridorThickness || corridorCoords.GetHeight() - 2> maxCorridorThickness)
        {
            if (corridorCoords.GetWidth() - 2 > maxCorridorThickness)
            {
                corridorCoords.left++;
                corridorCoords.right--;
            }
            if (corridorCoords.GetHeight() - 2 > maxCorridorThickness)
            {
                corridorCoords.bottom++;
                corridorCoords.top--;
            }
        }
        Corridor corridor = new Corridor(corridorCoords, tileSize); //TODO: Schauen ob der Korridor doppelt exisitiert
        room.corridors.Add(corridor);
        corridors.Add(corridor);
    }
    #endregion

}
