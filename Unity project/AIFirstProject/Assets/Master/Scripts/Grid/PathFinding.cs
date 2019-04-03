using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class PathFinding : MonoBehaviour
{

    Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    public void FindPath(PathRequest request, Action<PathResult> callback)//Is now a corutine
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        GridNode startNode = grid.NodeFromWorldPoint(request.pathStart); //the set of nodes to be evaluated
        GridNode targetNode = grid.NodeFromWorldPoint(request.pathEnd); //the set of nodes already evaluated
        startNode.parent = startNode;

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<GridNode> openSet = new Heap<GridNode>(grid.MaxSize);
            HashSet<GridNode> closedSet = new HashSet<GridNode>();
            openSet.Add(startNode); //the star position, normally the one where the player is

            while (openSet.Count > 0)
            {

                GridNode currentnode = openSet.RemoveFirst();
                closedSet.Add(currentnode);

                if (currentnode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (GridNode neighbour in grid.GetNeighbours(currentnode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))//if neighbour is not traversable or if it is in CLOSED
                    {
                        continue;//skip to the next 
                    }

                    int newMovementCostToNeighbour = currentnode.gCost + GetDistance(currentnode, neighbour) + neighbour.movementPenalty;
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) //if it comes up that the new calculation is lesser than the one done before, we change the value to show this (we are acceding this node from a shorter path, and this is now the currently optimal for the neighboar node -> we update it)
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentnode;

                        if (!openSet.Contains(neighbour))//now we have to recalculate this node, even if we alredy did, because the weight has chan+ged
                            openSet.Add(neighbour);

                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }

        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
            pathSuccess = waypoints.Length > 0;
        }
        callback(new PathResult(waypoints, pathSuccess, request.callback));

    }

    Vector3[] RetracePath(GridNode startNode, GridNode endNode)
    {
        List<GridNode> path = new List<GridNode>();
        GridNode currentNode = endNode;

        while (currentNode != startNode) //recrorremos el path al reves, desde el final, para ir de padre en padre
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints); //ponemos el path bien 
        return waypoints;
    }

    Vector3[] SimplifyPath(List<GridNode> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    int GetDistance(GridNode nodeA, GridNode nodeB)//get the distance between two given nodes
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY); // 14 = al pes de distancia en diagonal, 10 = al pes de menejarse horizontal o vertical
        return 14 * dstX + 10 * (dstY - dstX);
    }
}