using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    public Vector2 gridSize;
    public float nodeRadius;
    private float nodeLength; //simply 2*nodeRadius
    private Node[,] grid;
    public LayerMask unwalkableMask;
    private int gridSizeX, gridSizeY; //amount of nodes in X-Dimension / y-dimension


    // Start is called before the first frame update
    private void Start()
    {
        nodeLength = 2 * nodeRadius;
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeLength);
        gridSizeY = Mathf.RoundToInt(gridSize.y / nodeLength);
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position            //center of grid
            - new Vector3(1, 0, 0) * gridSize.x / 2             //go to left end
            - new Vector3(0, 0, 1) * gridSize.y / 2;            //go to bottom


        for(int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                //get WorldPosition for each Node
                Vector3 worldPosition = worldBottomLeft + new Vector3(1, 0, 0) * (x * nodeLength + nodeRadius) 
                                                        + new Vector3(0, 0, 1) * (y * nodeLength + nodeRadius);
                //check if walkable or obstacle
                bool walkable = !(Physics.CheckSphere(worldPosition, nodeRadius, unwalkableMask));

                //create Node in grid
                grid[x, y] = new Node(walkable, worldPosition);
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, 1, gridSize.y));

        if (grid != null)
        {

            foreach( Node a in grid)
            {
                if (a.walkable) { Gizmos.color = Color.cyan; }
                else { Gizmos.color = Color.black; }
                Gizmos.DrawCube(a.worldPosition, new Vector3(1,1,1) * (nodeLength - 0.3f));
            }
        }
    }
}
