using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringer : MonoBehaviour
{
    private NotificationManager notification;

    private void Start()
    {
        notification = GameBehaviour.Instance.Notifications;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("PlayerGround"))
        {
            notification.PostNotification(HealtSystemManager.Hit);
        }
    }
}
