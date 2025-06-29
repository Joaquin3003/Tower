using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource uiSoundSource;
    public AudioClip loseLifeSound;
    public AudioClip beerSound;
    public AudioClip fallIngredientSound;

    [Header("Músicas por escena")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    private string currentSceneName = "";


    [Header("Estado actual")]
    private bool musicOn = true, soundOn = true;

    [Header("Sprites")]
    public Sprite spriteOnNormal, spriteOnPressed;    
    public Sprite spriteOffNormal, spriteOffPressed;

    private Image musicOnImage, musicOffImage, soundOnImage, soundOffImage; //referencias

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Start()
    {
        LoadSettings();
        AssignUI();           // primera asignación en el menú
        UpdateAudioState();
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        AssignUI();           // Al volver al menú o cargar cualquier escena
        UpdateAudioState();
        ChangeMusicForScene(s.name);

    }

    public void AssignUI()
    {
        
        void SetupButton(string name, ref Image imgRef, UnityEngine.Events.UnityAction<bool> callback, bool param)
        {
            var go = GameObject.Find(name);
            if (go == null) return;

            
            var btn = go.GetComponent<Button>(); //busco el Button para reconectar OnClick
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => { callback(param); });
            }

            
            imgRef = go.GetComponent<Image>(); //busco el Imageen el GO, si no, en sus hijos
            if (imgRef == null)
                imgRef = go.GetComponentInChildren<Image>();
        }

        SetupButton("MusicOnButton", ref musicOnImage, ToggleMusic, true);
        SetupButton("MusicOffButton", ref musicOffImage, ToggleMusic, false);
        SetupButton("SoundOnButton", ref soundOnImage, ToggleSound, true);
        SetupButton("SoundOffButton", ref soundOffImage, ToggleSound, false);
    }

    public void ToggleMusic(bool on)
    {
        musicOn = on;
        PlayerPrefs.SetInt("musicOn", musicOn ? 1 : 0);
        UpdateAudioState();
        if (musicOn && musicSource.clip != null && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void ToggleSound(bool on)
    {
        soundOn = on;
        PlayerPrefs.SetInt("soundOn", soundOn ? 1 : 0);
        UpdateAudioState();
    }

    public void UpdateAudioState()
    {
        //mutear / desmutear
        if (musicSource != null) musicSource.mute = !musicOn;
        if (uiSoundSource != null) uiSoundSource.mute = !soundOn;

        //actualizacion de los  sprites (si es que existen)
        if (musicOnImage != null) musicOnImage.sprite = musicOn ? spriteOnPressed : spriteOnNormal;
        if (musicOffImage != null) musicOffImage.sprite = !musicOn ? spriteOffPressed : spriteOffNormal;
        if (soundOnImage != null) soundOnImage.sprite = soundOn ? spriteOnPressed : spriteOnNormal;
        if (soundOffImage != null) soundOffImage.sprite = !soundOn ? spriteOffPressed : spriteOffNormal;
    }

    private void LoadSettings()
    {
        musicOn = PlayerPrefs.GetInt("musicOn", 1) == 1;
        soundOn = PlayerPrefs.GetInt("soundOn", 1) == 1;
    }

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (!soundOn || clip == null) return;

        GameObject tempGO = new GameObject("TempAudio");
        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = volume;
        aSource.Play();

        Destroy(tempGO, clip.length);
    }

    private void ChangeMusicForScene(string sceneName)
    {
        if (sceneName == currentSceneName) return; // Ya está en esta escena
        currentSceneName = sceneName;

        AudioClip selectedClip = null;

        if (sceneName == "Menu") selectedClip = menuMusic;
        else if (sceneName == "Juego") selectedClip = gameMusic;

        if (selectedClip != null && musicSource.clip != selectedClip)
        {
            musicSource.clip = selectedClip;
            musicSource.loop = true;
            if (musicOn)
                musicSource.Play();
        }
    }
}
