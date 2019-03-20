using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIController : MonoBehaviour
{
    public float viewRadius = 1f;

    public Transform[] wayPointList;
    [HideInInspector] public int nextWayPoint;

    public abstract void MoveTo(Vector3 point); //Move character, every controller moves the character as they think they should
}
