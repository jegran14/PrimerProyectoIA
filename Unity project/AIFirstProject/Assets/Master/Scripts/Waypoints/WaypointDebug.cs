using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaypointDebug : MonoBehaviour {
    
	void RenameWPs(GameObject overlook)
	{
        GameObject[] wps;
	    wps = GameObject.FindGameObjectsWithTag("waypoint"); 
	    int i = 1;
	    foreach (GameObject wp in wps)  
	    { 
	     	if(wp != overlook)
	     	{
                wp.transform.position = new Vector3(wp.transform.position.x, 0.5f, wp.transform.position.z);
                wp.name = "WP" + string.Format("{0:000}",i); 
	     		i++; 
	     	} 
	    }	
	}

	void OnDestroy()
	{
		RenameWPs(this.gameObject);
	}

	// Use this for initialization
	void Start () { 
		if(this.transform.parent.gameObject.name != "WayPoint") return;
		RenameWPs(null);
	}
	
	// Update is called once per frame
	void Update () {
		this.GetComponent<TextMesh>().text = this.transform.parent.gameObject.name;
	}
    
}
