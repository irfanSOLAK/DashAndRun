using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndHandler : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameBehaviour.Instance.Notifications.PostNotification(GameState.Finished);
        }
    }
}
