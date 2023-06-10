using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    public GridManager grid;



    // Start is called before the first frame update
    void Start()
    {
        //grid = GetComponent<GridManager>();
    }


    public List<Node> AStar(Vector3 startVector, Vector3 endVector)
    {
        Debug.Log("A Star");
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        Node startNode = grid.getNodeFromWorldPosition(startVector);
        Debug.Log(startNode.worldPosition);
        Node endNode = grid.getNodeFromWorldPosition(endVector);
        //Debug.Log("test");
        openList.Add(startNode);
        //Debug.Log("test2");

        while (openList.Count > 0)
        {
            //Debug.Log("while-loop");
            Node currentNode = getNodeWithLowestCost(openList);
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == endNode)
            {
                List<Node> path = getCalculatedPath(startNode, endNode);
                return path;
                //parent path must be given
            }

            else
            {
                //Debug.Log("currentNode =");
                foreach (Node neighbour in grid.getNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedList.Contains(neighbour))
                    {
                        //go to next neighbour
                    }
                    else
                    {
                        int newPathCost = currentNode.g_cost + getDistanceBetweenNodes(currentNode, neighbour);
                        if (newPathCost < neighbour.g_cost || !openList.Contains(neighbour))
                        {
                            neighbour.g_cost = newPathCost;
                            neighbour.h_cost = getDistanceBetweenNodes(neighbour, endNode);
                            neighbour.parent = currentNode;
                            if (!openList.Contains(neighbour))
                            {
                                openList.Add(neighbour);
                            }
                        }
                    }
                }
            }
        }
        Debug.Log("Fehler in AStar");
        return null;

    }

    private List<Node> getCalculatedPath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node activeNode = endNode;
        while (activeNode != startNode)
        {
            path.Add(activeNode);
            activeNode = activeNode.parent;
        }
        path.Reverse();
        //Debug.Log("finished");
        grid.path = path;
        foreach (Node a in path)
        {
            Debug.Log("Path from startNode: " + startNode.worldPosition + " NEW Node: x: " + a.gridPosX + " , y: " + a.gridPosY + " , WorldPosition: " + a.worldPosition);
        }
        return path;
    }

    private int getDistanceBetweenNodes(Node node1, Node node2)
    {
        int distanceX = Mathf.Abs(node1.gridPosX - node2.gridPosX);
        int distanceY = Mathf.Abs(node1.gridPosY - node2.gridPosY);
        if (distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }
        else
        {
            return 14 * distanceX + 10 * (distanceY - distanceX);
        }
    }

    private Node getNodeWithLowestCost(List<Node> nodes)
    {
        if (nodes.Count == 0) return null;
        Node currentNode = nodes[0];
        for (int x = 1; x < nodes.Count; x++)
        {
            if (nodes[x].getf_cost() < currentNode.getf_cost())
            {
                currentNode = nodes[x];
            }
        }
        return currentNode;
    }
}
