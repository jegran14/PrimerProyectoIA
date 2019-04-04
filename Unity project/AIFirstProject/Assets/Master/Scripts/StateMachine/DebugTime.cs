using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTime : MonoBehaviour
{
    [Range(0, 1)]
    public float timeScale = 1f;

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeScale;
    }
}
