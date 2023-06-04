using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class BinaryRoom : Room
{
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
    public List<Corridor> corridors { get; private set; } = new List<Corridor>();

    //Splitting
    private bool horizontalSplit;
    private bool verticalSplit;
    

    public BinaryRoom(RoomCoords coords, RoomDesign design) : base(coords, design)
    {
        horizontalSplit = false;
        verticalSplit = false;

        leftRoom = null;
        rightRoom = null;

        if (coords.GetHeight() < design.minHeight || coords.GetWidth() < design.minWidth)
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
        {
            allRooms.Add(this);
        }

        if (leftRoom != null)
            allRooms.AddRange(leftRoom.getAllRooms());
        if (rightRoom != null)
            allRooms.AddRange(rightRoom.getAllRooms());

        return allRooms;
    }

    public bool isConnectedTo(Room room)
    {
        foreach(Corridor corridor in corridors)
        {
            if(corridor.Connects(room)) return true;
        }
        return false;
    }

    #region show room
    public override void SetActive(bool active)
    {
        isActive = active;
        RoomContainer.SetActive(false);
        foreach (Corridor corridor in corridors)
        {
            corridor.SetActive(active);
        }
    }

    public void Trim()
    {
        if (IsLeaf())
        {
            if (design.trimTilesIsRandom)
                design.setRandomTrimTiles();
            coords.left += Mathf.Min(design.trimTiles.left, coords.GetWidth() - design.minWidth);
            coords.right -= Mathf.Min(design.trimTiles.right, coords.GetWidth() - design.minWidth);
            coords.top -= Mathf.Min(design.trimTiles.top, coords.GetHeight() - design.minHeight);
            coords.bottom += Mathf.Min(design.trimTiles.bottom, coords.GetHeight() - design.minHeight);

            //check if room is still too large
            if (coords.GetWidth() > design.maxWidth)
            {
                int difference = coords.GetWidth() - design.maxWidth;
                coords.left += Mathf.CeilToInt(difference / 2f);
                coords.right -= Mathf.FloorToInt(difference / 2f);
            }
            if (coords.GetHeight() > design.maxHeight)
            {
                int difference = coords.GetHeight() - design.maxHeight;
                coords.bottom += Mathf.CeilToInt(difference / 2f);
                coords.top -= Mathf.FloorToInt(difference / 2f);
            }
        }
        if (leftRoom != null)
        {
            leftRoom.Trim();
        }
        if (rightRoom != null)
        {
            rightRoom.Trim();
        }
    }
    public override GameObject Instantiate()
    {
        if (IsLeaf())
        {
            RoomContainer = new GameObject("Room");
            RoomContainer.transform.position = getPositionInWorldCoords(coords.getCenterPosition());
            foreach (Corridor corridor in corridors)
            {
                corridor.Instantiate();//?.transform.SetParent(RoomContainer.transform, true);
            }
            instantiateFloor().transform.SetParent(RoomContainer.transform, true);
            instantiateWalls().transform.SetParent(RoomContainer.transform, true);
            instantiateCeiling().transform.SetParent(RoomContainer.transform, true);
            instantiatePillars().transform.SetParent(RoomContainer.transform, true);
            return RoomContainer;
        }
        else
        {
            leftRoom.Instantiate();
            rightRoom.Instantiate();
            return null;
        }
    }


    private GameObject instantiateWalls()
    {
        GameObject WallContainer = new GameObject("Wall");
        for (int x = coords.left; x <= coords.right; x++)
        {
            List<GameObject> wall = new List<GameObject> { design.wallPrefab };
            if (x % design.torchPadding == 0)
                wall.Add(design.torchPrefab);
            for (int y = 0; y < design.wallHeight; y++)
            {
                if(y > 0 || corridors.Where(c => c.coords.top == coords.bottom - 1 && c.coords.ContainsX(x)).ToList().Count == 0)
                    addObjects(wall, WallContainer, new Vector3(x, y, coords.bottom));
                if(y > 0 || corridors.Where(c => c.coords.bottom == coords.top + 1 && c.coords.ContainsX(x)).ToList().Count == 0)
                    addObjects(wall, WallContainer, new Vector3(x, y, coords.top), Quaternion.Euler(0, 180, 0));
            }
        }
        for (int z = coords.bottom; z <= coords.top; z++)
        {
            List<GameObject> wall = new List<GameObject> { design.wallPrefab };
            if (z % design.torchPadding == 0)
                wall.Add(design.torchPrefab);
            for (int y = 0; y < design.wallHeight; y++)
            {
                if (y > 0 || corridors.Where(c => c.coords.right == coords.left - 1 && c.coords.ContainsY(z)).ToList().Count == 0)
                    addObjects(wall, WallContainer, new Vector3(coords.left, y, z), Quaternion.Euler(0, 90, 0));
                if (y > 0 || corridors.Where(c => c.coords.left == coords.right + 1 && c.coords.ContainsY(z)).ToList().Count == 0)
                    addObjects(wall, WallContainer, new Vector3(coords.right, y, z), Quaternion.Euler(0, -90, 0));
            }
        }
        return WallContainer;
    }

    private GameObject instantiatePillars()
    {
        GameObject pillarContainer = new GameObject("Floor");
        for (int x = coords.left + 2; x <= coords.right - 2; x += design.pillarPadding)
        {
            for (int z = coords.bottom + 2; z <= coords.top - 2; z += design.pillarPadding)
            {
                if(Random.value > .5f)
                    addObject(design.pillarPrefab, pillarContainer, new Vector3(x, 0, z), Quaternion.Euler(0, Random.Range(0, 360), 0));
            }
        }
        return pillarContainer;
    }
    #endregion

    #region Splitting
    public bool Split()
    {
        // Attempt random split
        float rand = Random.value;
        if (rand < 0.5f && coords.GetWidth() >= 2 * design.minWidth)
        {
            VerticalSplit();
            return true;
        }
        else if (coords.GetHeight() >= 2 * design.minHeight )
        {
            HorizontalSplit();
            return true;
        }

        // Force split if theres too much space.
        if (coords.GetWidth() > design.maxWidth)
        {
            VerticalSplit();
            return true;
        }

        if (coords.GetHeight() > design.maxHeight)
        {
            HorizontalSplit();
            return true;
        }

        return false;
    }

    public void VerticalSplit()
    {
        int splitLocation = Random.Range(coords.left + design.minWidth - 1, coords.right - design.minWidth + 1);
        leftRoom = new BinaryRoom(coords.Copy().setRight(splitLocation), design);
        rightRoom = new BinaryRoom(coords.Copy().setLeft(splitLocation + 1), design);
        verticalSplit = true;
    }

    public void HorizontalSplit()
    {
        int splitLocation = Random.Range(coords.bottom + design.minHeight - 1, coords.top - design.minHeight + 1);
        leftRoom = new BinaryRoom(coords.Copy().setTop(splitLocation), design);
        rightRoom = new BinaryRoom(coords.Copy().setBootom(splitLocation + 1), design);
        horizontalSplit = true;
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
            for (int y = coords.bottom + design.corridorMargin; y <= coords.top - design.corridorMargin; y++)
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
            for (int y = coords.bottom + design.corridorMargin; y <= coords.top - design.corridorMargin; y++)
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
            for (int x = coords.left + design.corridorMargin; x <= coords.right - design.corridorMargin; x++)
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
            for (int x = coords.left + design.corridorMargin; x <= coords.right - design.corridorMargin; x++)
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
                Debug.LogWarning("Rooms couldn't be connected");
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
    public void AddCorridorToNeighbor(BinaryRoom neighbor, DIRECTION dir)
    {
        RoomCoords corridorCoords = new RoomCoords();
        switch (dir)
        {
            case DIRECTION.LEFT:
                corridorCoords = new RoomCoords(
                    neighbor.coords.right + 1,
                    coords.left - 1,
                    Mathf.Min(coords.top, neighbor.coords.top) - design.corridorMargin,
                    Mathf.Max(coords.bottom, neighbor.coords.bottom) + design.corridorMargin
                    );
                break;
            case DIRECTION.RIGHT:
                corridorCoords = new RoomCoords(
                    coords.right + 1,
                    neighbor.coords.left - 1,
                    Mathf.Min(coords.top, neighbor.coords.top) - design.corridorMargin,
                    Mathf.Max(coords.bottom, neighbor.coords.bottom) + design.corridorMargin
                    );
                break;
            case DIRECTION.TOP:
                corridorCoords = new RoomCoords(
                    Mathf.Max(coords.left, neighbor.coords.left) + design.corridorMargin,
                    Mathf.Min(coords.right, neighbor.coords.right) - design.corridorMargin,
                    neighbor.coords.bottom - 1,
                    coords.top + 1
                    );
                break;
            case DIRECTION.BOTTOM:
                corridorCoords = new RoomCoords(
                    Mathf.Max(coords.left, neighbor.coords.left) + design.corridorMargin,
                    Mathf.Min(coords.right, neighbor.coords.right) - design.corridorMargin,
                    coords.bottom - 1,
                    neighbor.coords.top + 1
                    );
                break;
        }
        //limit corridor size
        if (dir == DIRECTION.TOP || dir == DIRECTION.BOTTOM)
        {
            int difference = Random.Range(corridorCoords.GetWidth() - design.maxCorridorThickness, corridorCoords.GetWidth());
            if (difference > 0) 
            {
                corridorCoords.left += Mathf.CeilToInt(difference / 2f);
                corridorCoords.right -= Mathf.FloorToInt(difference / 2f);
            }
        }
        else if(dir == DIRECTION.LEFT || dir == DIRECTION.RIGHT)
        {
            int difference = Random.Range(corridorCoords.GetHeight() - design.maxCorridorThickness, corridorCoords.GetHeight());
            if (difference > 0)
            {
                corridorCoords.bottom += Mathf.CeilToInt(difference / 2f);
                corridorCoords.top -= Mathf.FloorToInt(difference / 2f);
            }
        }
        Corridor corridor = new Corridor(corridorCoords, design, dir, (this, neighbor));
        neighbor.corridors.Add(corridor);
        corridors.Add(corridor);
    }
    #endregion

}
