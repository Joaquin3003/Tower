using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonSound : MonoBehaviour
{
    public static ButtonSound instancia;
    public AudioSource sonido;
    public Image imagenSonido;

    public Slider slider;
    public Button botonSubirVolumen;
    public Button botonBajarVolumen;

    public AudioClip clickAudio;
    private bool sonidoActivado = true;

    [SerializeField] private List<Button> botonesPermitidos = new List<Button>();  // Lista de botones permitidos

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);

            // Asegurarse de que AudioSource existe en este objeto.
            sonido = GetComponent<AudioSource>();
            if (sonido == null)
            {
                sonido = gameObject.AddComponent<AudioSource>();
                Debug.LogWarning("No había AudioSource, se ha creado uno nuevo.");
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Para cuando se recarga la escena

        // Cargar el volumen desde PlayerPrefs
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenSonido", 0.5f);
        slider.value = volumenGuardado;
        sonido.volume = volumenGuardado;

        slider.onValueChanged.AddListener(ChangeSlider);
        botonSubirVolumen.onClick.AddListener(SubirVolumenSonido);
        botonBajarVolumen.onClick.AddListener(BajarVolumenSonido);

        // Agregar eventos a los botones
        AgregarEventosABotones();
        RevisarMuteSonido();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Asegurar que la lista de botones se actualice al recargar la escena
        AgregarEventosABotones();
    }

    public void SubirVolumenSonido()
    {
        ChangeSlider(1f);
    }

    public void BajarVolumenSonido()
    {
        ChangeSlider(0f);
    }

    public void ChangeSlider(float valor)
    {
        if (sonido != null)
        {
            sonido.volume = valor;
            PlayerPrefs.SetFloat("VolumenSonido", valor);
            PlayerPrefs.Save();
            RevisarMuteSonido();
        }
    }

    private void RevisarMuteSonido()
    {
        if (imagenSonido != null)
        {
            imagenSonido.enabled = slider.value == 0;
        }
    }

    private void AgregarEventosABotones()
    {
        Button[] botones = FindObjectsOfType<Button>(true);  // Buscar todos los botones

        foreach (Button btn in botones)
        {
            // Evitar agregar el mismo botón varias veces
            if (!botonesPermitidos.Contains(btn))
            {
                botonesPermitidos.Add(btn);
                btn.onClick.RemoveAllListeners(); // Eliminar listeners anteriores
                btn.onClick.AddListener(ReproducirSonido); // Agregar el nuevo listener
            }
        }
    }

    public void ReproducirSonido()
    {
        if (sonidoActivado && sonido != null && clickAudio != null)
        {
            sonido.PlayOneShot(clickAudio);
        }
    }
}