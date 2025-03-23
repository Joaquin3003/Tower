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
        // Eliminar todos los ingredientes instanciados en la lista
        foreach (GameObject ingrediente in ingredientesInstanciados)
        {
            if (ingrediente != null)
            {
                Destroy(ingrediente);
            }
        }

        // Obtener la cámara principal
        Camera camara = Camera.main;

        // Buscar y eliminar solo los "IngredienteFinal" visibles en pantalla
        GameObject[] ingredientesFinales = GameObject.FindGameObjectsWithTag("IngredienteFinal");
        foreach (GameObject ingredienteFinal in ingredientesFinales)
        {
            if (ingredienteFinal != null && EsVisibleEnPantalla(ingredienteFinal, camara))
            {
                Destroy(ingredienteFinal);
            }
        }

        ingredientesInstanciados.Clear(); // Limpiar la lista después de borrar los ingredientes
        Debug.Log("Se eliminaron los ingredientes visibles, pero se dejó el de referencia.");
    }

    // Función para comprobar si un objeto es visible en la cámara
    bool EsVisibleEnPantalla(GameObject objeto, Camera camara)
    {
        if (camara == null) return false;

        Vector3 puntoEnPantalla = camara.WorldToViewportPoint(objeto.transform.position);
        return (puntoEnPantalla.x >= 0 && puntoEnPantalla.x <= 1 && puntoEnPantalla.y >= 0 && puntoEnPantalla.y <= 1);
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