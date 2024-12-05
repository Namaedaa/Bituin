using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class TeamRAstar : MonoBehaviour
{
    /*public Transform seeker;
    public Transform target;*/

    public int heuristic1;
    public int heuristic2;

    public int diagonalCost = 14;

    internal Node targetNode;
    internal Node checkNode;

    NodeGraph nodegraph;
    // Start is called before the first frame update
    void Start()
    {
        nodegraph = GetComponent<NodeGraph>();
    }

    // Update is called once per frame
    void Update()
    {
        //this.FindPath(seeker.position,target.position);
    }

    public void FindPath(Vector3 starting_position, Vector3 ending_position, Action<List<Node>> callback)
    {
        nodegraph.InitializeGraph();

        Node startNode = nodegraph.NodeFromWorldPoint(starting_position);

        // Search for the closest walkable node on the target's X grid
        bool foundGround = false;
        targetNode = null;
        checkNode = nodegraph.NodeFromWorldPoint(ending_position);

        if (!checkNode.isWalkable)
        {
            //Debug.Log("Clicked node is not walkable");

            for (int y = 1; y < nodegraph.gridSizeY / 2; y++)
            {
                if (checkNode.gridY - y > 0)
                {     
                    if (nodegraph.nodes[checkNode.gridX, checkNode.gridY - y].isWalkable)
                    {
                        targetNode = nodegraph.nodes[checkNode.gridX, checkNode.gridY - y];
                        foundGround = true;
                    }
                    else if (!nodegraph.nodes[checkNode.gridX, checkNode.gridY - y].isWalkable && foundGround)
                        break;
                }

                if (checkNode.gridY + y < nodegraph.gridSizeY)
                {
                    if (nodegraph.nodes[checkNode.gridX, checkNode.gridY + y].isWalkable)
                    {
                        if (!foundGround)
                            targetNode = nodegraph.nodes[checkNode.gridX, checkNode.gridY + y];
                        else
                            break;
                        foundGround = true;
                    }
                }
            }
        }
        else
        {
            //Debug.Log("Clicked node is walkable");

            for (int y = 0; y < nodegraph.gridSizeY / 2; y++)
            {
                if (checkNode.gridY - y > 0)
                {
                    if (!nodegraph.nodes[checkNode.gridX, checkNode.gridY - y].isWalkable)
                    {
                        targetNode = nodegraph.nodes[checkNode.gridX, checkNode.gridY - y + 1];
                        foundGround = true;
                        break;
                    }
                }
            }
        }


        // No walkable path to target's X grid
        if (targetNode == null || !foundGround)
        {
            Debug.Log("No walkable!");
            callback(null);
            return; 
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for(int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                RetracePath(startNode, targetNode,callback);
                return;
            }

            foreach(Node neighbour in nodegraph.GetNeighbours(currentNode))
            {
                if(!neighbour.isWalkable || closedSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode,neighbour);

                /*int newMovementCostToNeighbour = currentNode.gCost +
                (neighbour.gridX != currentNode.gridX && neighbour.gridY != currentNode.gridY ? diagonalCost : horiVeriCost) +
                heuristic1 * Mathf.Abs(neighbour.gridY - currentNode.gridY);*/

                //int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if ( newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode; 

                    if(!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        callback(null);
        return;
    }

    Node checkClickedNode(Node node)
    {
        if(node.isWalkable)
        {
            if (nodegraph.nodes[node.gridX, node.gridY + 1].isWalkable && !nodegraph.nodes[node.gridX, node.gridY - 1].isWalkable)
            {
                return node;
            }
            else if (nodegraph.nodes[node.gridX, node.gridY - 1].isWalkable && !nodegraph.nodes[node.gridX, node.gridY + 1].isWalkable)
            {
                return nodegraph.nodes[node.gridX, node.gridY - 1];
            }
        }

        return null;
    }

    void RetracePath(Node startNode, Node endNode, Action<List<Node>> callback)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        nodegraph.path = path;

        callback(path);
        return;
    }

    int GetDistance(Node a, Node b)
    {
        int distanceX = Mathf.Abs(a.gridX - b.gridX);
        int distanceY = Mathf.Abs(a.gridY - b.gridY);

        int heuristicCost = heuristic1 * (distanceX + distanceY) + heuristic2 * Mathf.Max(0, a.gridY - b.gridY);

        // Add diagonal cost based on the number of diagonal moves
        int diagonalMoves = Mathf.Min(distanceX, distanceY);
        int remainingMoves = Mathf.Abs(distanceX - distanceY);
        heuristicCost += diagonalMoves * diagonalCost + remainingMoves * heuristic1;


        return 0;
    }
}
