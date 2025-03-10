using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverPanel;

    void Start()
    {
        Time.timeScale = 1; // Asegura que el tiempo vuelva a la normalidad al iniciar
        gameOverPanel.SetActive(false);
    }

    public void MostrarGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void MenuPrincipal()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MenuPrincipal");

        // Antes de reiniciar el puntaje, verificamos si es el más alto
        int currentScore = ScoreManager.Instance.GetScore();
        ScoreManager.Instance.CheckAndSaveHighScore(currentScore);

        ScoreManager.Instance.ResetScore();
        ScoreManager.Instance.StartCounting();  // Reinicia la cuenta de puntos
    }
}
