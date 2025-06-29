using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOptionsPanel : MonoBehaviour
{
    void OnEnable()
    {
        
        var am = FindObjectOfType<AudioManager>();//cuando este panel se active en la escena:
        if (am != null)
        {
            am.AssignUI();          //rebusca los botones y reconecta OnClick
            am.UpdateAudioState();  //actualizaa los sprites al estado correcto
        }
        else
        {
            Debug.LogWarning("AudioManager no encontrado desde AudioOptionsPanel.");
        }
    }
}
