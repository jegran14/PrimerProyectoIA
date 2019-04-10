using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnterDoor : MonoBehaviour
{
    private LevelChanger sceneScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            sceneScript = GameObject.Find("SceneChanger").GetComponent<LevelChanger>();

            if (tag == "red") sceneScript.levelSelection = 1;

            if (tag == "green") sceneScript.levelSelection = 2;

            if (tag == "blue") sceneScript.levelSelection = 3;

            if (tag == "golden") sceneScript.victory = true;    
            
            sceneScript.FadeToBlack();
        }
    }
}
