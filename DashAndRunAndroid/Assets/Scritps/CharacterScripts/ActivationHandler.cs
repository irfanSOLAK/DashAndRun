using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationHandler : MonoBehaviour
{
    private void OnEnable()
    {
        if (GameObject.FindWithTag("SceneController"))
        {
            GameObject sceneController = GameObject.FindWithTag("SceneController");
            transform.position = sceneController.GetComponent<SpawnPoints>().SpawnTransform.position;
        }
        else
        {
            Debug.LogError("There is no Scene Controller");
        }
    }
}
