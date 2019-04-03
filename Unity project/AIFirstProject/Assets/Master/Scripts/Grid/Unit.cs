using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform target;
    public float speed = 3; //** passar a public
    public float turnDst = 5; //**
    // Vector3[] path; 
   // int targetIndex; 
    const float minPathUpdateTime = .2f; //**
    const float pathUpdateMoveThreshold = .5f; //** es la distancia minima que es deu de haver menejat el target per a updatear el path
    public float turnSpeed = 3;//**
    Path path; //**

    void Start()
    {
        StartCoroutine(UpdatePath()); //**
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful) //** tota canviada
    {
        if (pathSuccessful)
        {
            path = new Path(waypoints, transform.position, turnDst); //**
            //targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator UpdatePath()//** nova funció, every frame it sees if the target waypoint has changed position and updates if its greater than the treshold
    {

        if (Time.timeSinceLevelLoad < .3f)//this is to correct the issue that happens when we first open the editor
        {
            yield return new WaitForSeconds(.3f);
        }
        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                targetPosOld = target.position;
            }
        }
    }

    IEnumerator FollowPath()//** se ha canviat com funciona
    {
        bool followingPath = true;
        int pathIndex = 0;
        transform.LookAt(path.lookPoints[0]);


        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z); 
            //check if the unit has passed the next boundary
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                //rotate the unit a little bit towards the look point and move the unit forward

                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
            }

            yield return null;

        }
    }

    public void OnDrawGizmos()//** tota canviada
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }
}
