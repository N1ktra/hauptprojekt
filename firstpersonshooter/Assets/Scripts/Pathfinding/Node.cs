using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridPosX;
    public int gridPosY;

    public int g_cost; //distance from starting node to this node
    public int h_cost; //distance from this node to end node (approximated)
                        //f_cost = g_cost + h_cost

    public Node(bool walkable_, Vector3 worldPos, int gridPosX_, int gridPosY_)
    {
        walkable = walkable_;
        worldPosition = worldPos;
        gridPosX = gridPosX_;
        gridPosY = gridPosY_;
    }

    public int getf_cost()
    {
        return g_cost + h_cost;
        
    }
}
