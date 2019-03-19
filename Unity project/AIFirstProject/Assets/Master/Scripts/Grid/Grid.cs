using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadious;

    private GridNode[,] grid;

    private void OnDrawGizmos()
    {
        Vector3 upperLeftCorner = transform.position - new Vector3(16f/2f, 0f, 9f/2f);
        for(int x = 0; x < 16; x++)
            for(int y = 0; y < 9; y++)
            {
                Vector3 position = upperLeftCorner + new Vector3(x, transform.position.y, y);
                Gizmos.DrawCube(position, new Vector3(0.9f, 0.5f, 0.9f));
            }
    }
}
