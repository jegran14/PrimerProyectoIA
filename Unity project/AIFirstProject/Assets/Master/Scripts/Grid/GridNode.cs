using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : IHeapItem<GridNode>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int movementPenalty;

    public int gCost; //el peso de este nodo hasta el nodo inicial
    public int hCost; //el peso para llegar hasta el nodo final, el objetivo
    public GridNode parent; //the node that comes before this one in the pathfinding method
    int heapIndex;

    public GridNode(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty)
    {
        walkable = _walkable;
        worldPosition = _worldPos;

        gridX = _gridX;
        gridY = _gridY;
        movementPenalty = _penalty;
    }

    public int fCost //la suma de todos los pesos, y el que decidirá cual es el nodo más ligero (el del camino mas corto)
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }

        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(GridNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);

        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare; throw new System.NotImplementedException();
    }
}
