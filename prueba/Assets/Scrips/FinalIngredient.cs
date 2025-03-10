using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalIngredient : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ingredientes")) // Si toca otro ingrediente
        {
            Debug.Log("El ingrediente final colisionó con la torre. Fin del juego.");
            FindObjectOfType<GameOver>().MostrarGameOver();
        }
        if (collision.gameObject.CompareTag("Borde"))
        {

            Destroy(gameObject);

        }
        if (collision.gameObject.CompareTag("Base"))
        {

            Destroy(gameObject);

        }
    }
}
