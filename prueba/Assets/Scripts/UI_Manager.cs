using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UI_Manager : MonoBehaviour
{
    public GameObject pauseMenu; // Panel de pausa
    public GameObject controlesButton;
    public GameObject creditosButton;
    public GameObject exitButton;
    private bool botonesVisibles = false;
    public bool isPaused = false;


    private void Start()
    {
        #if UNITY_WEBGL
            if(exitButton != null)
                exitButton.SetActive(false);
        #endif

    }
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
        Time.timeScale = 1f; //reanudo el juego
        pauseMenu.SetActive(false);
        Debug.Log("Juego reanudado.");
    }
    public void Reiniciar()
    {
        Time.timeScale = 1f; //se asegura que el tiempo vuelve a la normalidad
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //Recargamos la escena actual
    }
    public void Jugar()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Juego");
    }
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public void ToggleOpciones()
    {
        botonesVisibles = !botonesVisibles;
        controlesButton.SetActive(botonesVisibles);
        creditosButton.SetActive(botonesVisibles);

        if (botonesVisibles)
        {
            
            var am = FindObjectOfType<AudioManager>();//Una vez que Opciones es visible entra al If
            if (am != null)
            {
                am.AssignUI();          //se reasigna los botones e imágenes
                am.UpdateAudioState();  //se actualiza sprites y mute/unmute
            }
            else
            {
                Debug.LogWarning("AudioManager no encontrado al abrir Opciones.");
            }
        }
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

    [DllImport("__Internal")]
    private static extern void ReloadPage();

    [DllImport("__Internal")]
    private static extern void ExitFullscreen();

    public void ExitGame()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ExitFullscreen(); // sale del modo fullscreen
        ReloadPage();     // recarga la pestaña actual
#endif
    }
}
