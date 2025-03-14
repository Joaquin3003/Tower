using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ingredientes : MonoBehaviour
{
    public SpawnPoint spawner;
    private bool hasCollided = false;
    private bool gameOverTriggered = false;  // Cambi� a no est�tico
    private GameOver gameOverManager;


    void Start()
    {
        gameOverTriggered = false;
        gameOverManager = FindObjectOfType<GameOver>();

        if (gameOverManager == null)
        {
            Debug.LogError("GameOver NO encontrado en la escena.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ingredientes") && !hasCollided)
        {
            hasCollided = true;
            spawner.OnIngredientLanded(gameObject); // PASAMOS EL INGREDIENTE COMO PAR�METRO

            float offset = Mathf.Abs(transform.position.x - collision.transform.position.x);
            int puntos = (offset < 0.1f) ? 15 : 10;
            ScoreManager.Instance.AddScore(puntos);
        }

        if (collision.gameObject.CompareTag("Base") && !hasCollided)
        {
            hasCollided = true;
            spawner.OnIngredientLanded(gameObject); // PASAMOS EL INGREDIENTE COMO PAR�METRO

            float offset = Mathf.Abs(transform.position.x - collision.transform.position.x);
            int puntos = (offset < 0.1f) ? 15 : 10;
            ScoreManager.Instance.AddScore(puntos);
        }

        if (collision.gameObject.CompareTag("Borde") && !gameOverTriggered)
        {
            // Incrementamos el contador de ingredientes destruidos
            spawner.ingredientesDestruidos++;

            // Comprobamos si ya se han destruido m�s de 4 ingredientes
            if (spawner.ingredientesDestruidos >= 4)
            {
                gameOverTriggered = true;
                spawner.DisableSpawning();
                spawner.SpawnFinalIngredient();
                ScoreManager.Instance.StopCounting();

                // Restar una vida cuando se destruyen 3 ingredientes
                if (LifeManager.Instance != null)
                {
                    LifeManager.Instance.PerderIngrediente(); // Restar una vida por colisi�n con el borde
                }
            }

            Destroy(gameObject);  // Destruir el ingrediente independientemente de cu�ntos ingredientes se hayan destruido
        }

        if (collision.gameObject.CompareTag("IngredienteFinal"))
        {
            Debug.Log("Ingrediente final colision�. Activando Game Over...");
            StartCoroutine(ActivateGameOver());
        }
    }

    /*void OnBecameInvisible()
    {
        GameObject panelMenu = GameObject.Find("PanelMenuPrincipal"); // Busca el panel en la escena

        if (panelMenu == null || !panelMenu.activeSelf) // Si no est� en el men�, destruye el ingrediente
        {
            if (LifeManager.Instance != null)
            {
                LifeManager.Instance.PerderIngrediente();
            }
            Destroy(gameObject);
        }
    }*/

    private IEnumerator ActivateGameOver()
    {
        yield return new WaitForSeconds(0.1f); // Espera para dar tiempo al �ltimo ingrediente a caer
        FindObjectOfType<GameOver>().MostrarGameOver();
    }

}