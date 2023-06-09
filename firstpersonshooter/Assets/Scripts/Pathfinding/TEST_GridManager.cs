using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_GridManager : MonoBehaviour
{

    public Vector2 gridSize;
    public float nodeRadius;
    private float nodeLength; //simply 2*nodeRadius
    private Node[,] grid;
    public LayerMask unwalkableMask;
    private int gridAmountX, gridAmountY; //amount of nodes in X-Dimension / y-dimension

    public List<Node> path;
    public Transform p1;
    public bool showGizmos;

    // Start is called before the first frame update
    private void Start()
    {
        nodeLength = 2 * nodeRadius;
        gridAmountX = Mathf.RoundToInt(gridSize.x / nodeLength);
        gridAmountY = Mathf.RoundToInt(gridSize.y / nodeLength);
        CreateGrid();
    }

   
    private void CreateGrid()
    {
        grid = new Node[gridAmountX, gridAmountY];
        Vector3 worldBottomLeft = transform.position            //center of grid
            - new Vector3(1, 0, 0) * gridSize.x / 2             //go to left end
            - new Vector3(0, 0, 1) * gridSize.y / 2;            //go to bottom


        for(int x = 0; x < gridAmountX; x++)
        {
            for (int y = 0; y < gridAmountY; y++)
            {
                //get WorldPosition for each Node
                Vector3 worldPosition = worldBottomLeft + new Vector3(1, 0, 0) * (x * nodeLength + nodeRadius) 
                                                        + new Vector3(0, 0, 1) * (y * nodeLength + nodeRadius);
                //check if walkable or obstacle
                bool walkable = !(Physics.CheckSphere(worldPosition, nodeRadius, unwalkableMask));

                //create Node in grid
                grid[x, y] = new Node(walkable, worldPosition, x, y);
            }
        }
    }

    public void test()
    {
        Debug.Log("Test");
    }


    //gets a Vector from the "real world" and returns the node from the grid. The node where the "real world" Vector is in.
    public Node getNodeFromWorldPosition(Vector3 worldPos)
    {
        //verschiebe World Nullpunkt auf Grid Nullpunkt
        Vector3 newWorldPos = new Vector3(worldPos.x + gridSize.x/2, worldPos.y, worldPos.z + gridSize.y/2 );

        float prozentualerWegX = newWorldPos.x / gridSize.x;
        //Debug.Log("proz X: " + prozentualerWegX.ToString());
        int gridX = Mathf.RoundToInt(prozentualerWegX * (gridAmountX-1));

        float prozentualerWegY = newWorldPos.z / gridSize.y;
        //Debug.Log("proz Y: " + prozentualerWegY.ToString());
        int gridY = Mathf.RoundToInt(prozentualerWegY * (gridAmountY-1));

        //Debug.Log("Position im Gird: x: " + gridX + " y: " + gridY);
        return grid[gridX,gridY];
    }

    //returns a List of the neighbour nodes from the grid, so the 8 nodes around activeNode
    public List<Node> getNeighbours(Node activeNode)
    {
        List<Node> neighbours = new List<Node>();

        for(int x=-1; x<=1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x!=0 || y != 0)  //do not add the activeNode itself
                {
                    int nx = activeNode.gridPosX + x;
                    int ny = activeNode.gridPosY + y;
                    if(nx >= 0 && nx < gridAmountX && ny >= 0 && ny < gridAmountY)  //check if we are still in the grid
                    {
                        neighbours.Add(grid[nx, ny]);
                    }
                }
            }
        }

        return neighbours;
    }


    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, 1, gridSize.y));

            if (grid != null)
            {
                Node testNode = getNodeFromWorldPosition(p1.position);
                foreach (Node a in grid)
                {
                    if (a.walkable) { Gizmos.color = Color.cyan; }
                    else { Gizmos.color = Color.black; }
                    if (path != null)
                    {
                        if (path.Contains(a))
                        {
                            Gizmos.color = Color.yellow;
                        }
                    }
                    if (a == testNode) { Gizmos.color = Color.red; }
                    Gizmos.DrawCube(a.worldPosition, new Vector3(1, 1, 1) * (nodeLength - 0.3f));
                }
            }
        }
        
    }
}
