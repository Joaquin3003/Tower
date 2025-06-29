using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighscoreMenu : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;

    void Start()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = highScore.ToString();
    }
}
