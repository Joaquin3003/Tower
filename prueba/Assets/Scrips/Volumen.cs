using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Volumen : MonoBehaviour
{
    public static Volumen instancia;
    public AudioSource musica;
    public Image imagenMute;

    public Slider slider;

    private AudioSource audioMusica;
    private bool MusicaActivado = true;

    void Start()
    {
        slider.onValueChanged.AddListener(ChangeSlider);;
        RevisarMute();
    }

    public void ChangeSlider(float valor)
    {
        if (musica == null) return;
        musica.volume = valor;
        PlayerPrefs.SetFloat("VolumenAudio", valor);
        PlayerPrefs.Save();
        RevisarMute();
    }

    private void RevisarMute()
    {
        if (musica == null) return;
        imagenMute.enabled = slider.value == 0;
    }
}
