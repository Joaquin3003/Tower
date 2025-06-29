using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonSound : MonoBehaviour
{
    public AudioClip clickSound;

    public void PlayClick()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlaySound(clickSound);
        }
    }
}