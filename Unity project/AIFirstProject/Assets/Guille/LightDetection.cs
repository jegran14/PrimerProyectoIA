using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDetection : MonoBehaviour
{
    private AudioSource alarm;
    private Animator animator;
    public bool greenState = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        alarm = GetComponent<AudioSource>();
        alarm.loop = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            animator.SetTrigger("Detected"); // Pasa a rojo
            alarm.Play();
            greenState = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            animator.SetTrigger("Caution"); // Pasa a amarillo
        }
    }

    // Se invoca al final de la animación del amarillo
    public void EndDetection()
    {
        alarm.Stop();
        greenState = true;
    }

}
