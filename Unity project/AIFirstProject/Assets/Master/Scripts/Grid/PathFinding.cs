using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class PathFinding : MonoBehaviour
{
    public Transform seeker, target;
    Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    void Update()
    {
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        GridNode startNode = grid.NodeFromWorldPoint(startPos); //the set of nodes to be evaluated
        GridNode targetNode = grid.NodeFromWorldPoint(targetPos); //the set of nodes already evaluated

        Heap<GridNode> openSet = new Heap<GridNode>(grid.MaxSize);
        HashSet<GridNode> closedSet = new HashSet<GridNode>();
        openSet.Add(startNode); //the star position, normally the one where the player is

        while (openSet.Count > 0)
        {
            GridNode node = openSet.RemoveFirst();
            /*for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost) //if the total cost to go to this node is less or equal to one in the openSet
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];//we add that node to the openSet
                }
            }

            openSet.Remove(node);*/
            closedSet.Add(node);

            if (node == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (GridNode neighbour in grid.GetNeighbours(node))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))//if neighbour is not traversable or if it is in CLOSED
                {
                    continue;//skip to the next 
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) //if it comes up that the new calculation is lesser than the one done before, we change the value to show this (we are acceding this node from a shorter path, and this is now the currently optimal for the neighboar node -> we update it)
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))//now we have to recalculate this node, even if we alredy did, because the weight has chan+ged
                        openSet.Add(neighbour);
                }
            }
        }
    }

    void RetracePath(GridNode startNode, GridNode endNode)
    {
        List<GridNode> path = new List<GridNode>();
        GridNode currentNode = endNode;

        while (currentNode != startNode) //recrorremos el path al reves, desde el final, para ir de padre en padre
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse(); //ponemos el path bien 

        grid.path = path;

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