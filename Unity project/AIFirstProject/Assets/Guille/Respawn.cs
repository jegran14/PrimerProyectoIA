using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private GameObject player;
    private Transform respawnPoint;

    void Start()
    {
        respawnPoint = GetComponent<Transform>();
        player = GameObject.Find("Player");
        RespawnPlayer();
    }

    void RespawnPlayer()
    {
        Instantiate(player, respawnPoint.position, respawnPoint.rotation);
    }

}
