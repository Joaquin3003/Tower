using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseLifeZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ingredientes"))
        {
            GameplayController.instance.LoseLife();
            Destroy(collision.gameObject);
        }
    }
}

