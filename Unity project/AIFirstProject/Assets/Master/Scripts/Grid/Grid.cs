using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    /* to set the level, don't forget to put the obstacles in the obstcles layer
     * and the gridWorldSizes manually*/
    public Transform player;

    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize; //desde el editor, fer el tamany de la caixa tan gran com vuigues de superficie per a calcular el A*(tamny del nivell)
    public float nodeRadius;//how much space each individual node covers

    private float nodeDiameter;
    private int gridSizeX, gridSizeY;
    private GridNode[,] grid;

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }


    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); //how many nodes we can fit in the X
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new GridNode[gridSizeX, gridSizeY];


        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2; 
        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask)); //checks if the point collides with the unwakable mask, true if it does
                grid[x, y] = new GridNode(walkable, worldPoint, x, y); //adds the currrent point to the grid
            }
        }
    }

    public GridNode NodeFromWorldPoint(Vector3 worldPosition)//used to find the node for a specific position in the world, like the node where the player is
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public List<GridNode> path;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null)
        {
            GridNode playerNode = NodeFromWorldPoint(player.transform.position);
            foreach(GridNode n in grid)//visualization of the grid when start is pressed
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                if (playerNode == n)
                    Gizmos.color = Color.blue;
                if (path != null)
                    if (path.Contains(n))
                        Gizmos.color = Color.black;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }

    public List<GridNode> GetNeighbours(GridNode node) //basically, we find the nodes adjacents to this one
    {
        List<GridNode> neighbours = new List<GridNode>();

        //search in a 3 by 3 block 
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;//we skip this iteration because the is the current node

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }
}
