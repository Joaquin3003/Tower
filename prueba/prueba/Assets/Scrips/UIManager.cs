using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    void Update()
    {
        scoreText.text = " " + ScoreManager.Instance.GetScore();

        highScoreText.text = " " + ScoreManager.Instance.GetHighScore();
    }
}