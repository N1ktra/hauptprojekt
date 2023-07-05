using System;
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
        grid = GameObject.Find("Grid").GetComponent<GridManager>();
    }


    public Node searchWalkableNeighbour(Node node)
    {
        //check Knoten oberhalb
        RaycastHit hit;
        List<Node> neighbours = grid.getNeighbours(node);
        Debug.Log(neighbours);
        foreach(Node n in neighbours)
        {
            //Knoten oberhalb
            if (n.gridPosX == node.gridPosX && n.gridPosY == node.gridPosY+1 && !Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 2, grid.unwalkableMask))
            {
                return n;
            }
            //Knoten unterhalb
            if (n.gridPosX == node.gridPosX && n.gridPosY == node.gridPosY-1 && !Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hit, 2, grid.unwalkableMask))
            {
                return n;
            }
            //Knoten links
            if (n.gridPosX == node.gridPosX-1 && n.gridPosY == node.gridPosY && !Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 2, grid.unwalkableMask))
            {
                return n;
            }
            //Knoten rechts
            if (n.gridPosX == node.gridPosX + 1 && n.gridPosY == node.gridPosY && !Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 2, grid.unwalkableMask))
            {
                return n;
            }
            //Knoten oben links
            if (n.gridPosX == node.gridPosX -1 && n.gridPosY == node.gridPosY+1 && !Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-1,0,1)), out hit, 2.4f, grid.unwalkableMask))
            {
                return n;
            }
            //Knoten oben rechts
            if (n.gridPosX == node.gridPosX + 1 && n.gridPosY == node.gridPosY + 1 && !Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(1, 0, 1)), out hit, 2.4f, grid.unwalkableMask))
            {
                return n;
            }
            //Knoten unten rechts
            if (n.gridPosX == node.gridPosX + 1 && n.gridPosY == node.gridPosY - 1 && !Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(1, 0, -1)), out hit, 2.4f, grid.unwalkableMask))
            {
                return n;
            }
            //Knoten unten links
            if (n.gridPosX == node.gridPosX - 1 && n.gridPosY == node.gridPosY - 1 && !Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-1, 0, -1)), out hit, 2.4f, grid.unwalkableMask))
            {
                return n;
            }
        }
        return null;
    }


    public List<Node> AStar(Vector3 startVector, Vector3 endVector)
    {
        try { 
        //Debug.Log("A Star mit GegnerPosition:" + startVector + " , PlayerPosition: " + endVector);
        Node startNode = grid.getNodeFromWorldPosition(startVector);
        //Debug.Log("StartKnoten: " + startNode.worldPosition);
        if (startNode == null)
        {
            Debug.LogWarning("FEHLER getNodeFromWorldPosition");
            return null;
        }
        Node endNode = grid.getNodeFromWorldPosition(endVector);

        //check ob Gegner in unwalkable steht
        //falls ja, dann suche benachbarten walkable Node und berechne von dort AStar
        
        if (!startNode.walkable)
        {
            Debug.Log("NOT WALKABLE");
            startNode = searchWalkableNeighbour(startNode);
            if(startNode == null)
            {
                Debug.LogWarning("FEHLER searchWalkableNeighbour");
                return null;
            }
        }
        
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        openList.Add(startNode);

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
                        //not walkable node ist Player Position
                        if(!neighbour.walkable && neighbour == grid.getPlayerNode())
                        {
                            neighbour.parent = currentNode;
                            List<Node> path = getCalculatedPath(startNode, currentNode);
                            return path;
                        }


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
        Debug.LogWarning("Fehler in AStar");
        return null; //leere Liste 
        }
        catch(NullReferenceException) { Debug.LogWarning("NullReferenceException"); return null; }
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
        grid.paths.Add(path);
        foreach (Node a in path)
        {
            //Debug.Log("Path from startNode: " + startNode.worldPosition + " NEW Node: x: " + a.gridPosX + " , y: " + a.gridPosY + " , WorldPosition: " + a.worldPosition);
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
