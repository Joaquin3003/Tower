using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpawnPoint : MonoBehaviour
{
    public GameObject[] ingredientes;
    public float moveSpeed = 2f;
    public float horizontalLimit = 3f;
    public float initialHeightIncrement = 0.01f;
    private float currentHeightIncrement = 0f;

    private int ingredientesContados = 0;
    public GameObject currentIngredient;
    private bool isIngredientMoving = false;
    private bool isPaused = false;

    public GameObject finalIngredientPrefab;
    private bool isSpawningDisabled = false;
    private bool hasFinalIngredientSpawned = false;

    private ScoreManager scoreManager;
    private float originalMoveSpeed;
    public int ingredientesDestruidos = 0; // Contador de ingredientes destruidos

    // Guardar la posición inicial del Spawn Point
    private Vector3 posicionInicial;

    void Start()
    {
        currentHeightIncrement = 0f;
        moveSpeed = originalMoveSpeed = 1f;
        SpawnIngredient();
        scoreManager = FindObjectOfType<ScoreManager>();

        // Guardamos la posición inicial del Spawn Point
        posicionInicial = transform.position;

        // Reiniciar contador de ingredientes destruidos
        ingredientesDestruidos = 0;
        Invoke("VerificarYSpawnIngrediente", 0.2f);
    }

    void Update()
    {
        if (LogicaBotones.juegoPausadoGlobal) return;

        if (!isPaused)
        {
            // Movimiento del spawn point
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;

            // Corrección del rebote en los límites
            if (transform.position.x >= horizontalLimit)
            {
                transform.position = new Vector3(horizontalLimit, transform.position.y, transform.position.z);
                moveSpeed = -Mathf.Abs(moveSpeed); // Asegurar que el movimiento sea negativo
            }
            else if (transform.position.x <= -horizontalLimit)
            {
                transform.position = new Vector3(-horizontalLimit, transform.position.y, transform.position.z);
                moveSpeed = Mathf.Abs(moveSpeed); // Asegurar que el movimiento sea positivo
            }
        }

        if (currentIngredient != null && isIngredientMoving)
        {
            currentIngredient.transform.position = new Vector3(
                transform.position.x,
                currentIngredient.transform.position.y,
                currentIngredient.transform.position.z
            );
        }

        if (Input.GetMouseButtonDown(0) && currentIngredient != null && isIngredientMoving)
        {
            if (!IsPointerOverUI())
            {
                ReleaseIngredient();
            }
            else
            {
                Debug.Log("Clic bloqueado porque fue en la UI");
            }
        }
    }

    public void OnIngredientDestroyed(GameObject ingredient)
    {
        if (ingredient.CompareTag("Ingrediente"))
        {
            Debug.Log("Destruyendo ingrediente...");
            ingredientesDestruidos++;

            if (LifeManager.Instance != null)
            {
                LifeManager.Instance.PerderIngrediente();
                Debug.Log("Vidas restantes: " + LifeManager.Instance.GetIngredientesRestantes());
            }
            else
            {
                Debug.LogError("LifeManager.Instance es NULL. No se puede restar una vida.");
            }

            if (ingredientesDestruidos >= 3 && !hasFinalIngredientSpawned)
            {
                Debug.Log("SE ACTIVARÁ EL INGREDIENTE FINAL");
                SpawnFinalIngredient();
            }
        }
    }

    public void OnIngredientLanded(GameObject ingredient)
    {
        if (ingredient.CompareTag("IngredienteFinal"))
        {
            Debug.Log("Ingrediente final ha aterrizado. Activando Game Over.");
            FindObjectOfType<GameOver>().MostrarGameOver();
            return;
        }

        currentIngredient = null;
        StartCoroutine(SpawnNextIngredient());
    }

    private IEnumerator SpawnNextIngredient()
    {
        yield return new WaitForSeconds(0.1f);
        SpawnIngredient();
    }

    public void SpawnIngredient()
    {
        if (currentIngredient != null) return;

        int randomIndex = Random.Range(0, ingredientes.Length);
        currentIngredient = Instantiate(ingredientes[randomIndex], transform.position, Quaternion.identity);
        currentIngredient.GetComponent<Rigidbody2D>().isKinematic = true;

        currentIngredient.transform.position += new Vector3(0, 0, currentIngredient.transform.position.y * 0.01f);

        LogicaBotones logicaBotones = FindObjectOfType<LogicaBotones>();
        if (logicaBotones != null)
        {
            logicaBotones.RegistrarIngrediente(currentIngredient);
        }

        isIngredientMoving = true;
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

    public void StartPauseMovement()
    {
        isPaused = true;
        moveSpeed = 0f;
        if (currentIngredient != null)
        {
            currentIngredient.GetComponent<Rigidbody2D>().isKinematic = true;
        }
    }

    public void StopPauseMovement()
    {
        isPaused = false;
        moveSpeed = originalMoveSpeed;
        if (currentIngredient != null)
        {
            currentIngredient.GetComponent<Rigidbody2D>().isKinematic = false;
        }
    }

    private void ReleaseIngredient()
    {
        isIngredientMoving = false;
        if (currentIngredient != null)
        {
            currentIngredient.GetComponent<Rigidbody2D>().isKinematic = false;
        }

        ingredientesContados++;

        if (ingredientesContados == 9)
        {
            currentHeightIncrement = initialHeightIncrement;
        }
        else if (ingredientesContados > 9)
        {
            transform.position += new Vector3(0, currentHeightIncrement, 0);
        }
    }

    public void DisableSpawning()
    {
        isSpawningDisabled = true;
        gameObject.SetActive(false);
    }

    public void SpawnFinalIngredient()
    {
        if (finalIngredientPrefab != null && !hasFinalIngredientSpawned)
        {
            hasFinalIngredientSpawned = true;
            Vector3 spawnPosition = transform.position + new Vector3(0, -1f, 0);
            Instantiate(finalIngredientPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public void DetenerSpawnPorTiempo(float tiempo)
    {
        StartCoroutine(DetenerSpawnCoroutine(tiempo));
    }

    private IEnumerator DetenerSpawnCoroutine(float tiempo)
    {
        isPaused = true;
        moveSpeed = 0f;

        yield return new WaitForSeconds(tiempo);

        isPaused = false;
        moveSpeed = originalMoveSpeed;
    }

    void ReiniciarIngredientes()
    {
        GameObject[] ingredientes = GameObject.FindGameObjectsWithTag("Ingrediente");

        foreach (GameObject ingrediente in ingredientes)
        {
            Destroy(ingrediente);
        }

        Debug.Log("Ingredientes reiniciados.");
    }

    public void ResetearSpawnPoint()
    {
        gameObject.SetActive(true);
        transform.position = posicionInicial;
        ingredientesContados = 0;
        hasFinalIngredientSpawned = false;
        isSpawningDisabled = false;
        ingredientesDestruidos = 0;

        if (currentIngredient != null)
        {
            Destroy(currentIngredient);
            currentIngredient = null;
        }

        StartCoroutine(ForzarSpawn());
        Debug.Log("Spawn Point reiniciado. Contador de ingredientes destruidos en 0.");
    }

    private IEnumerator ForzarSpawn()
    {
        yield return new WaitForSeconds(0.2f);
        SpawnIngredient();
    }

    void OnEnable()
    {
        StartCoroutine(EsperarYVerificarIngrediente());
    }

    IEnumerator EsperarYVerificarIngrediente()
    {
        yield return new WaitForSeconds(0.2f);

        if (currentIngredient == null)
        {
            SpawnIngredient();
            Debug.Log("No había ingrediente en el SpawnPoint. Se generó uno nuevo.");
        }
    }

    void VerificarYSpawnIngrediente()
    {
        if (currentIngredient == null)
        {
            SpawnIngredient();
            Debug.Log("No había ingrediente en el SpawnPoint. Se generó uno nuevo.");
        }
    }

    public bool PuedeActivarHabilidad()
    {
        // La habilidad solo puede activarse si hay un ingrediente en el Spawn y NO ha sido soltado
        if (currentIngredient != null && isIngredientMoving)
        {
            Debug.Log("Habilidad puede activarse: Ingrediente en Spawn y aún no ha sido soltado.");
            return true;
        }

        Debug.Log("Habilidad bloqueada: No hay ingrediente en el Spawn o ya fue soltado.");
        return false;
    }
}