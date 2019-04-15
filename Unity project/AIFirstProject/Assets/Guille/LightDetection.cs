using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDetection : MonoBehaviour
{
    private AudioSource alarm;
    private Animator animator;

    public bool greenState = true;
    public float alarmRadius = 10f;
    public float distanceForShuttingOff = 7f;

    private LayerMask npcMask;

    void Start()
    {
        animator = GetComponent<Animator>();
        alarm = GetComponent<AudioSource>();

        npcMask = LayerMask.GetMask("NPC");

        alarm.loop = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            animator.SetTrigger("Detected"); // Pasa a rojo
            alarm.Play();
            AlarmGuards(other.transform);
            greenState = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            animator.SetTrigger("Caution"); // Pasa a amarillo
            StartCoroutine(CheckDistanceToTarget(other.transform));
        }
    }

    // Se invoca al final de la animación del amarillo
    public void EndDetection()
    {
        alarm.Stop();
        animator.SetTrigger("Disabled");
        greenState = true;
    }

    private void AlarmGuards(Transform target)
    {
        Collider[] agents = Physics.OverlapSphere(transform.position, alarmRadius, npcMask);

        foreach(Collider agent in agents)
        {
            AIController controller = agent.GetComponent<AIController>();
            controller.SetAlarm(target);
        }
    }

    private  IEnumerator CheckDistanceToTarget(Transform target)
    {
        bool targetInDistance = true;

        while (targetInDistance)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            
            if (distance > distanceForShuttingOff)
                targetInDistance = false;

            yield return null;
        }
           

        EndDetection();
    }
}
