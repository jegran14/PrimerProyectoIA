using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intermittence : MonoBehaviour
{  
    public bool initialShow;
    private bool show;

    public float waitingTime = 2.0f;
    private float lastTime = 0.0f;
    private float currentTime = 0.0f;

    private GameObject lightAlarm;
    private LightDetection script;
    private bool greenState;

    void Start()
    {
        show = initialShow;
        lightAlarm = this.gameObject.transform.GetChild(0).gameObject;
        lightAlarm.SetActive(show);
        script = lightAlarm.GetComponent<LightDetection>();
        greenState = true;
    }

    
    void Update()
    {
        greenState = script.greenState;
        currentTime += Time.deltaTime;

        if (!greenState)
        {
            lastTime = currentTime;
        }

        else if (currentTime - lastTime >= waitingTime)
        {
            show = !show;
            Blink();
            lastTime = currentTime;
        }
    }

    public void Blink()
    {
        lightAlarm.SetActive(show);
    }
}
