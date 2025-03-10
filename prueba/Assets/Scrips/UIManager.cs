using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI ShadowScoreText;
    public TextMeshProUGUI ShadowHighScoreText;

    void Update()
    {
        scoreText.text = " " + ScoreManager.Instance.GetScore();
        ShadowScoreText.text = " " + ScoreManager.Instance.GetScore();

        highScoreText.text = " " + ScoreManager.Instance.GetHighScore();
        ShadowHighScoreText.text = " " + ScoreManager.Instance.GetHighScore();
    }
}