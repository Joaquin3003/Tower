using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI: MonoBehaviour
{
    public Text scoreText; // Referencia al elemento de texto en el canvas

    void Update()
    {
        scoreText.text = $" {ScoreManager.Instance.GetScore()}";
    }
}
