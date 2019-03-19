using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardMovement : MonoBehaviour
{
    Transform goal;
    float speed = 5.0f;
    float accurancy = 2.0f;
    float rotSpeed = 2.0f;
    public GameObject wpManager;
    GameObject[] wps;
    GameObject currentNode;
    int currentWP = 0;
    int goalWP = 0;
    Graph g;

    bool patrulla = true;

    void Start()
    {
        // Recibir el grafo y waypoints del WaypointManager
        wps = wpManager.GetComponent<WPManager>().waypoints;
        g = wpManager.GetComponent<WPManager>().graph;
        currentNode = wps[1];
    }


    // MÉTODO PARA CAMBIAR RUTA DADO EL ID DEL NODO DESTINO
    public void GoToWaypoint(int idGoal)
    {
        // El idGoal es el índice del nodo destino respecto a la lista de waypoints
        // mientras que goalWP es el último índice del recorrido de nodos generados por pathfinding
        g.AStar(currentNode, wps[idGoal]);
        currentWP = 0;
        goalWP = g.getPathLength()-1;
    }

    private void LateUpdate()
    {
        Debug.Log("currentNode: " + currentNode);

        // IMPLEMENTACIÓN DEL MOVIMIENTO

        if (patrulla)
        {
            GoToWaypoint(6);
            patrulla = false;
        }
        else if (currentWP == goalWP) patrulla = true;

        // Si tienes un camino vacío o estás al final del camino no te tienes que mover
        if (g.getPathLength() == 0 || currentWP == g.getPathLength())
            return;

        // El nodo actual será el que más cerca tengamos en cada momento
        currentNode = g.getPathPoint(currentWP);
        
        // Si estamos lo suficientemente cerca del nodo actual, ir al siguiente
        if(Vector3.Distance(g.getPathPoint(currentWP).transform.position,
            transform.position) < accurancy)
        {
            currentWP++;
        }

        // Si todavía no hemos alcanzado el nodo actual
        if(currentWP < g.getPathLength())
        {
            goal = g.getPathPoint(currentWP).transform;
            Vector3 lookAtGoal = new Vector3(goal.position.x,
                this.transform.position.y, goal.transform.position.z);
            Vector3 direction = lookAtGoal - this.transform.position;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);

            this.transform.Translate(0,0,speed*Time.deltaTime); // Mover
        }

    }

    
}
