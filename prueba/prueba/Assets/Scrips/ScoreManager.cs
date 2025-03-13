using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    private int score = 0;  // Puntuación actual
    private bool isCounting = true; // Determina si el puntaje debe sumarse
    private int highScore = 0; // Puntaje más alto

    void Awake()
    {
        // Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Evita que se destruya al cambiar de escena
        }
        else
        {
            Destroy(gameObject);
        }

        // Cargar el puntaje más alto guardado
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void AddScore(int points)
    {
        if (isCounting)
        {
            score += points;

            // Si se supera el puntaje más alto, actualizarlo
            if (score > highScore)
            {
                highScore = score;
                PlayerPrefs.SetInt("HighScore", highScore);
                PlayerPrefs.Save(); // Guardar el nuevo puntaje más alto
            }
        }
    }

    public void CheckAndSaveHighScore(int currentScore)
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.Save();
        }
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
        return highScore;
    }

    public int GetScore()
    {
        return score;
    }

    public void StopCounting()
    {
        isCounting = false;
    }

    public void StartCounting()
    {
        isCounting = true;
    }

    public void ResetScore()
    {
        score = 0; // Solo reinicia el puntaje actual, no el máximo
    }
}
