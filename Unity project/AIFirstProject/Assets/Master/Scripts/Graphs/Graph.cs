using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Graph
{
	List<Edge>	edges = new List<Edge>();
	List<Node>	nodes = new List<Node>();
	List<Node> pathList = new List<Node>();   
	
	public Graph(){}
	
	public void AddNode(GameObject id, bool removeRenderer = true, bool removeCollider = true, bool removeId = true)
	{
		Node node = new Node(id);
		nodes.Add(node);
		
		// Eliminar colliders, mesh renderer y texto de los gameObjects considerados nodos
		if(removeCollider)
			GameObject.Destroy(id.GetComponent<Collider>());
		if(removeRenderer)
			GameObject.Destroy(id.GetComponent<Renderer>());
		if(removeId)
		{
			TextMesh[] textms = id.GetComponentsInChildren<TextMesh>() as TextMesh[];

        	foreach (TextMesh tm in textms)	
				GameObject.Destroy(tm.gameObject);
		}
	}
	
    // Asignar Arcos (Edges) de un nodo a otro 
	public void AddEdge(GameObject fromNode, GameObject toNode)
	{
		Node from = findNode(fromNode); // busca los nodos en la lista nodes
		Node to = findNode(toNode);
		
		if(from != null && to != null)
		{
            // Si no son nulos, añade el arco a la lista del grafo y actualiza las conexiones del propio nodo
			Edge e = new Edge(from, to);
			edges.Add(e);
			from.edgelist.Add(e);
		}	
	}
	
    // Buscar un nodo del grafo
	Node findNode(GameObject id)
	{
		foreach (Node n in nodes) 
		{
			if(n.getId() == id)
				return n;
		}
		return null;
	}
	
	
	public int getPathLength()
	{
		return pathList.Count;	
	}

    public int getNodesLength()
    {
        return nodes.Count;
    }

    public GameObject getPathPoint(int index)
	{
		return pathList[index].id;
	}
	
	public void printPath()
	{
		foreach(Node n in pathList)
		{	
			Debug.Log(n.id.name);	
		}
	}
	
	// Algoritmo de pathfinding A estrella (Astar) 
	public bool AStar(GameObject startId, GameObject endId)
	{
	  	Node start = findNode(startId);
	  	Node end = findNode(endId);
	  
	  	if(start == null || end == null)
	  	{
	  		return false;	
	  	}

        // Para las open y closed lists podríamos usar un BinaryHeap por eficiencia 
        // aunque sería menos práctico y costoso de implementar

        List<Node>	open = new List<Node>(); // lista de nodos que deben de checkearse
	  	List<Node>	closed = new List<Node>(); // lista de nodos ya checkeados
	  	float tentative_g_score= 0;
	  	bool tentative_is_better;
	  	
	  	start.g = 0; // Coste del Movimiento g
	  	start.h = distance(start,end); // Valor de la función Heurística h
	  	start.f = start.h; // Coste Total f
	  	open.Add(start);
	  	
	  	while(open.Count > 0)
	  	{
            // Consideramos el nodo con más bajo coste total f
	  		int i = lowestF(open);
			Node thisnode = open[i];

            // Comparamos dicho nodo con el nodo destino y reconstruimos el camino
			if(thisnode.id == endId)
			{
				reconstructPath(start,end);
				return true;	
			} 	
			
			open.RemoveAt(i);
			closed.Add(thisnode);
			
            // Consideramos los nodos vecinos recorriendo los arcos del nodo actual
			Node neighbour;
			foreach(Edge e in thisnode.edgelist)
			{
				neighbour = e.endNode;
				neighbour.g = thisnode.g + distance(thisnode,neighbour);
				
				if (closed.IndexOf(neighbour) > -1)
					continue;
				
                // Valor provisional del vecino
				tentative_g_score = thisnode.g + distance(thisnode, neighbour);

				if(open.IndexOf(neighbour) == -1)
				{
					open.Add(neighbour);
					tentative_is_better = true;	
				}
				else if (tentative_g_score < neighbour.g)
				{
					tentative_is_better = true;	
				}
				else
					tentative_is_better = false;
					
                // Actualizar valores del vecino cuando valor provisional vecino es mejor
                // y redirigir su camino al nodo actual
				if(tentative_is_better)
				{
					neighbour.cameFrom = thisnode;
					neighbour.g = tentative_g_score;
					neighbour.h = distance(thisnode,end);
					neighbour.f = neighbour.g + neighbour.h;	
				}
			}
  	
	  	}
		
		return false;	
	}
	
    // Método para reconstruir el camino generado por el algoritmo de pathfinding 
	public void reconstructPath(Node startId, Node endId)
	{
		pathList.Clear();
		pathList.Add(endId);
		
		var p = endId.cameFrom;
		while(p != startId && p != null)
		{
			pathList.Insert(0,p);
			p = p.cameFrom;	
		}
		pathList.Insert(0,startId);
	}
	
    // Método que devuelve la distancia entre dos nodos
    float distance(Node a, Node b)
    {
	  float dx = a.xPos - b.xPos;
	  float dy = a.yPos - b.yPos;
	  float dz = a.zPos - b.zPos;
	  float dist = dx*dx + dy*dy + dz*dz;
	  return( dist );
    }

    // Método para encontrar el nodo de la lista con menor coste total f
    int lowestF(List<Node> l)
    {
	  float lowestf = 0;
	  int count = 0;
	  int iteratorCount = 0;
	  	  
	  for (int i = 0; i < l.Count; i++)
	  {
	  	if(i == 0)
	  	{	
	  		lowestf = l[i].f;
	  		iteratorCount = count;
	  	}
	  	else if( l[i].f <= lowestf )
	  	{
	  		lowestf = l[i].f;
	  		iteratorCount = count;	
	  	}
	  	count++;
	  }
	  return iteratorCount;
    }
    
    // Método para dibujar líneas de apoyo con el trazado del grafo
    public void debugDraw()
    {
      	// Dibuja los Arcos de los nodos en una línea roja
    	for (int i = 0; i < edges.Count; i++)
	  	{
    		Debug.DrawLine(edges[i].startNode.id.transform.position, edges[i].endNode.id.transform.position, Color.red);
    		
	  	}
	  	// Dibuja la dirección como una raya azul sobre la misma línea del Arco
	  	for (int i = 0; i < edges.Count; i++)
	  	{
	  		Vector3 to = (edges[i].startNode.id.transform.position - edges[i].endNode.id.transform.position) * 0.05f;
    		Debug.DrawRay(edges[i].endNode.id.transform.position, to, Color.blue);
    	}
    }
	
}
