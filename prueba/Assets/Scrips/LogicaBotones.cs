using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogicaBotones : MonoBehaviour
{
    public GameObject panelJuego;
    public GameObject panelMenuPrincipal;
    public GameObject panelPausa;
    public GameObject panelGameOver;

    public GameObject boton1;
    public GameObject boton2;
    private bool botonesVisibles = false;

    private bool juegoPausado = false;
    public static bool juegoPausadoGlobal = false;

    private List<GameObject> ingredientesInstanciados = new List<GameObject>();

    public SpawnPoint spawnPoint;

    public void Jugar()
    {
        Time.timeScale = 1f;
        juegoPausadoGlobal = false;
        ReiniciarIngredientes();

        SpawnPoint spawnPoint = FindObjectOfType<SpawnPoint>(true);
        if (spawnPoint != null)
        {
            spawnPoint.gameObject.SetActive(true);
            spawnPoint.ResetearSpawnPoint();
        }
        else
        {
            Debug.LogWarning("SpawnPoint no encontrado. No se pudo reiniciar.");
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
            ScoreManager.Instance.StartCounting();
        }

        if (LifeManager.Instance != null)
        {
            LifeManager.Instance.ReiniciarContador();
        }

        ReiniciarJuego(); // Jugar ahora hace lo mismo que ReiniciarJuego(), pero mostrando el panel correcto
        if (panelMenuPrincipal != null) panelMenuPrincipal.SetActive(false); // Cerrar menú principal
        if (panelJuego != null) panelJuego.SetActive(true);
        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(false);
    }

    public void MenuPrincipal()
    {
        Time.timeScale = 1f;
        juegoPausadoGlobal = false;

        ReiniciarIngredientes();

        SpawnPoint spawnPoint = FindObjectOfType<SpawnPoint>(true);
        if (spawnPoint != null)
        {
            spawnPoint.gameObject.SetActive(true);
            spawnPoint.ResetearSpawnPoint();
        }
        else
        {
            Debug.LogWarning("SpawnPoint no encontrado. No se pudo reiniciar.");
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
            ScoreManager.Instance.StartCounting();
        }

        if (LifeManager.Instance != null)
        {
            LifeManager.Instance.ReiniciarContador();
        }

        // OCULTAR EL GAME OVER AL VOLVER AL MENÚ
        if (panelGameOver != null) panelGameOver.SetActive(false);

        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelMenuPrincipal != null) panelMenuPrincipal.SetActive(true);
        if (panelJuego != null) panelJuego.SetActive(false);

        Debug.Log("Regresando al menú principal sin eliminar ingredientes.");
    }

    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        juegoPausadoGlobal = false;

        ReiniciarIngredientes(); // Borrar ingredientes

        SpawnPoint spawnPoint = FindObjectOfType<SpawnPoint>(true);
        if (spawnPoint != null)
        {
            spawnPoint.gameObject.SetActive(true);
            spawnPoint.ResetearSpawnPoint(); // Resetear el SpawnPoint
            spawnPoint.StopPauseMovement();  // Asegurar que no esté pausado
        }
        else
        {
            Debug.LogWarning("SpawnPoint no encontrado. No se pudo reiniciar.");
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
            ScoreManager.Instance.StartCounting();
        }

        if (LifeManager.Instance != null)
        {
            LifeManager.Instance.ReiniciarContador();
        }

        // Cerrar otros paneles y activar el juego
        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelMenuPrincipal != null) panelMenuPrincipal.SetActive(false);
        if (panelJuego != null) panelJuego.SetActive(true);

        Skill skill = FindObjectOfType<Skill>();
        if (skill != null)
        {
            skill.ResetSkill();
        }

        SkillButton skillButton = FindObjectOfType<SkillButton>();
        if (skillButton != null)
        {
            skillButton.ResetSkillButton();
        }

        Debug.Log("Juego reiniciado correctamente.");
    }

    void ReiniciarIngredientes()
    {
        // Eliminar solo los ingredientes instanciados
        foreach (GameObject ingrediente in ingredientesInstanciados)
        {
            if (ingrediente != null)
            {
                Destroy(ingrediente);
            }
        }

        foreach (GameObject IngredienteFinal in ingredientesInstanciados)
        {
            if (IngredienteFinal != null)
            {
                Destroy(IngredienteFinal);
            }
        }

        ingredientesInstanciados.Clear(); // Limpiar la lista después de borrar los ingredientes
        Debug.Log("Ingredientes instanciados eliminados.");
    }

    // Método para registrar ingredientes en la lista
    public void RegistrarIngrediente(GameObject ingrediente)
    {
        ingredientesInstanciados.Add(ingrediente);
    }

    private SpawnPoint FindInactiveSpawnPoint()
    {
        SpawnPoint[] allSpawnPoints = Resources.FindObjectsOfTypeAll<SpawnPoint>();
        foreach (SpawnPoint sp in allSpawnPoints)
        {
            if (!sp.gameObject.activeInHierarchy) // Si está desactivado en la escena
            {
                return sp; // Lo devuelve
            }
        }
        return null; // No encontrado
    }

    public void Pausar()
    {
        if (panelPausa != null) panelPausa.SetActive(true);
        Time.timeScale = 0f;
        juegoPausado = true;
        juegoPausadoGlobal = true;
        Debug.Log("Juego pausado.");
    }

    public void Reanudar()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
        Time.timeScale = 1f;
        juegoPausado = false;
        juegoPausadoGlobal = false;
        Debug.Log("Juego reanudado.");
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Salió del juego.");
    }

    public void ToggleBotones()
    {
        botonesVisibles = !botonesVisibles;
        boton1.SetActive(botonesVisibles);
        boton2.SetActive(botonesVisibles);
    }
}