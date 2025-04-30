using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GameplayController : MonoBehaviour
{
    [Header("Propiedades")]
    public static GameplayController instance;
    public Spawner spawner;
    public CameraScript cameraScript;
    private int moveCount;
    public float cameraHeight = 2.0f;
    public bool isFrozen = false;
    public int skillDuration = 5;

    [Header("Vidas")]
    public int lives = 3;
    public Text vidasUI;

    [Header("Puntos")]
    public int score = 0;
    public int highScore = 0;
    public TextMeshProUGUI currentScore;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI gameOverScore;

    [HideInInspector]
    public Ingredient currentIngredient;

    public GameObject gameOverMenu;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        spawner.SpawnIngredient();
        UpdateLivesUI();
        LoadHighScore();
        UpdateScoreUI();
    }

    // Update is called once per frame
    void Update()
    {
        DetectInput();
    }
    void DetectInput()
    {
        // Para mouse (PC)
        if (Input.GetMouseButtonDown(0) && currentIngredient != null)
        {
            if (!IsPointerOverUI())
            {
                currentIngredient.SoltarIngrediente();
            }
            else
            {
                Debug.Log("Clic bloqueado porque fue en la UI");
            }
        }

        // Para touch (mobile)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {

                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                if (currentIngredient != null)
                {
                    currentIngredient.SoltarIngrediente();
                }
            }
        }
    }
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
        }

        return EventSystem.current.IsPointerOverGameObject();
    }
    public void SpawnNewIngredient()
    {
        Invoke("NewIngredient", 1f);
    }

    void NewIngredient()
    {
        spawner.SpawnIngredient();
    }

    public void MoveCamera()
    {
        moveCount++;

        if(moveCount == 3)
        {
            moveCount = 0;
            cameraScript.targetPos.y += cameraHeight;
        }
    }
    public void LoseLife()
    {
        lives--;
        UpdateLivesUI();

        Debug.Log("Vidas restantes: " + lives);
        if (lives < 1)
        {
            GameOver();
        }
    }
    private void UpdateLivesUI()
    {
        if (vidasUI != null)
        {
            vidasUI.text = lives.ToString();
        }
    }
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }
    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }
    private void UpdateScoreUI()
    {
        if (currentScore != null)
            currentScore.text = score.ToString();

        if (highScoreText != null)
            highScoreText.text = highScore.ToString();
    }
    public void ActivateFreeze()
    {
        isFrozen = true;
        Invoke("DeactivateFreeze", skillDuration);
    }

    private void DeactivateFreeze()
    {
        isFrozen = false;
    }

    private void GameOver()
    {
        Debug.Log("¡Fin del Juego!");
        gameOverMenu.SetActive(true);
        Debug.Log("Panel GO Activado");
        Time.timeScale = 0f;  
        gameOverScore.text = score.ToString();
    }

    public void RestartGame()
    {
        Debug.Log("¡Juego terminado!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
