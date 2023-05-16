using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    GridManager grid;
    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponent<GridManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AStar(Vector3 startVector, Vector3 endVector)
    {
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        Node startNode = grid.getNodeFromWorldPosition(startVector);
        Node endNode = grid.getNodeFromWorldPosition(endVector);
        openList.Add(startNode);

        while(openList.Count > 0)
        {
            Node currentNode = getNodeWithLowestCost(openList);
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == endNode)
            {
                return;
                //parent path must be given
            }

            else
            {
                foreach(Node neighbour in grid.getNeighbours(currentNode))
                {
                    if(!neighbour.walkable || closedList.Contains(neighbour))
                    {
                        //go to next neighbour
                    }
                    else
                    {
                        //
                    }
                }
            }
        }



    }

    private Node getNodeWithLowestCost(List<Node> nodes)
    {
        if (nodes.Count == 0) return null;
        Node currentNode = nodes[0];
        for(int x=1; x<nodes.Count; x++)
        {
            if(nodes[x].getf_cost() < currentNode.getf_cost())
            {
                currentNode = nodes[x];
            }
        }
        return currentNode;
    }
}
