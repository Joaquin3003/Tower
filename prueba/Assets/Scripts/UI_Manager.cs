using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    public GameObject pauseMenu; // Panel de pausa
    public GameObject controlesButton;
    public GameObject creditosButton;
    private bool botonesVisibles = false;
    public bool isPaused = false;

    public void TogglePause()
    {
        isPaused= true;
        Debug.Log("Juego pausado.");
        if (isPaused)
        {
            Time.timeScale = 0f; // Pausa el juego
            pauseMenu.SetActive(true);
        }
        else
        {
            Reanudar();
        }
    }
    public void Reanudar()
    {
        isPaused = false;
        Time.timeScale = 1f; // Reanudar el juego
        pauseMenu.SetActive(false);
        Debug.Log("Juego reanudado.");
    }
    public void Reiniciar()
    {
        Time.timeScale = 1f; // Asegurar que el tiempo vuelve a la normalidad
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recarga la escena actual
    }
    public void Jugar()
    {
        //Time.timeScale = 1f; // Asegurar que el tiempo vuelve a la normalidad
        UnityEngine.SceneManagement.SceneManager.LoadScene("Juego");
    }
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Asegurar que el tiempo vuelve a la normalidad
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public void ToggleOpciones()
    {
        botonesVisibles = !botonesVisibles;
        controlesButton.SetActive(botonesVisibles);
        creditosButton.SetActive(botonesVisibles);
    }

    public void OnFreezeButtonPressed()
    {
        if(GameplayController.instance != null)
        {
            GameplayController.instance.ActivateFreeze();
        }
        else
        {
            Debug.LogError("GameplayController no está inicializado.");
        }
    }
}
