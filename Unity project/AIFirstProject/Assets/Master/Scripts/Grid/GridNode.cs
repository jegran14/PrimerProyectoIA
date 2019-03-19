using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
    public bool walkable;
    public Vector3 worldPosition;

    public GridNode(bool _walkable, Vector3 _worldPos)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
    }
}
