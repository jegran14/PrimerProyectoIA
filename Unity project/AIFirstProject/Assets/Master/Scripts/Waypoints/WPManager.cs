using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Estableceremos a partir de un struct las conexiones de los waypoints 
// que van a actuar como nodos del grafo
[System.Serializable]
public struct Link
{
    public enum Direction {UNI, BI};
    public GameObject node1;
    public GameObject node2;
    public Direction dir;
}

public class WPManager : MonoBehaviour
{
    public GameObject[] waypoints;  // lista de waypoints que vamos a meter al grafo
    public Link[] links;            // lista de conexiones entre cada par de waypoints
    public Graph graph = new Graph();

    void Start()
    {
        if(waypoints.Length > 0)
        {
            // Añadimos todos los waypoints al grafo de la escena
            foreach(GameObject wp in waypoints)
            {
                graph.AddNode(wp);
            }

            // Asociamos las conexiones de los waypoints que hemos definido
            // a los Edges de los nodos del grafo
            foreach (Link l in links)
            {
                graph.AddEdge(l.node1, l.node2);

                // En caso de ser una conexión Bidireccional, 
                // se añadirá el Edge en sentido contrario
                if (l.dir == Link.Direction.BI)
                    graph.AddEdge(l.node2, l.node1);
            }
        }
    }

    void Update()
    {
        // Método del grafo para dibujar los arcos que conectan los nodos
        graph.debugDraw();
    }
}
