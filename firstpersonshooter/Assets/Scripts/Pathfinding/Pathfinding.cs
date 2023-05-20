using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    GridManager grid;

    public Transform p1, p2;


    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponent<GridManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Node test = grid.getNodeFromWorldPosition(p1.position);
        //Debug.Log(test.gridPosX.ToString());
        AStar(p1.position, p2.position);
    }

    void AStar(Vector3 startVector, Vector3 endVector)
    {
        //Debug.Log("A Star");
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>(); 
        Node startNode = grid.getNodeFromWorldPosition(startVector);
        Node endNode = grid.getNodeFromWorldPosition(endVector);
        //Debug.Log("test");
        openList.Add(startNode); 
        //Debug.Log("test2");

        while(openList.Count > 0)
        {
            //Debug.Log("while-loop");
            Node currentNode = getNodeWithLowestCost(openList);
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == endNode)
            {
                getCalculatedPath(startNode, endNode);
                return;
                //parent path must be given
            }

            else
            {
                //Debug.Log("currentNode =");
                foreach(Node neighbour in grid.getNeighbours(currentNode))
                {
                    if(!neighbour.walkable || closedList.Contains(neighbour))
                    {
                        //go to next neighbour
                    }
                    else
                    {
                        int newPathCost = currentNode.g_cost + getDistanceBetweenNodes(currentNode, neighbour);
                        if(newPathCost < neighbour.g_cost || !openList.Contains(neighbour)) 
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

    }

    private void getCalculatedPath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node activeNode = endNode;
        while(activeNode != startNode) 
        { 
            path.Add(activeNode);
            activeNode = activeNode.parent;
        }
        path.Reverse();
        //Debug.Log("finished");
        grid.path = path;
    }

    private int getDistanceBetweenNodes(Node node1, Node node2)
    {
        int distanceX = Mathf.Abs(node1.gridPosX - node2.gridPosX);
        int distanceY = Mathf.Abs(node1.gridPosY - node2.gridPosY);
        if(distanceX > distanceY)
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
