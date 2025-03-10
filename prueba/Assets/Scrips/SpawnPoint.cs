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

    // Nueva variable: Guardar la posición inicial del Spawn Point
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
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;

            if (transform.position.x > horizontalLimit || transform.position.x < -horizontalLimit)
            {
                moveSpeed *= -1;
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

    // Nuevo método para detectar ingredientes destruidos
    public void OnIngredientDestroyed(GameObject ingredient)
    {
        if (ingredient.CompareTag("Ingrediente"))
        {
            Debug.Log("Destruyendo ingrediente...");
            ingredientesDestruidos++;  // Incrementar el contador de ingredientes destruidos

            // Llamamos a la función que maneja la vida y destrucción
            if (LifeManager.Instance != null)
            {
                LifeManager.Instance.PerderIngrediente(); // Restar una vida al perder un ingrediente
                Debug.Log("Vidas restantes: " + LifeManager.Instance.GetIngredientesRestantes()); // Ahora se puede llamar sin errores
            }
            else
            {
                Debug.LogError("LifeManager.Instance es NULL. No se puede restar una vida.");
            }

            // Si se han destruido 3 ingredientes o más, activar el ingrediente final
            if (ingredientesDestruidos >= 3 && !hasFinalIngredientSpawned)
            {
                Debug.Log("SE ACTIVARÁ EL INGREDIENTE FINAL");
                SpawnFinalIngredient();
            }
        }
    }

    // Asegurar que los ingredientes sean destruidos correctamente
    public void OnIngredientLanded(GameObject ingredient)
    {
        if (ingredient.CompareTag("IngredienteFinal"))
        {
            Debug.Log("Ingrediente final ha aterrizado. Activando Game Over.");
            FindObjectOfType<GameOver>().MostrarGameOver();
            return;
        }

        currentIngredient = null;  // Limpiar el ingrediente actual
        StartCoroutine(SpawnNextIngredient()); // Empezar el spawn después de 0.5 segundos
    }

    private IEnumerator SpawnNextIngredient()
    {
        yield return new WaitForSeconds(0.1f); // Esperar 0.5 segundos antes de spawnear el siguiente ingrediente
        SpawnIngredient(); // Crear un nuevo ingrediente
    }

    public void SpawnIngredient()
    {
        if (currentIngredient != null) return; // No spawnar si ya hay un ingrediente en pantalla

        int randomIndex = Random.Range(0, ingredientes.Length); // Seleccionar un ingrediente aleatorio
        currentIngredient = Instantiate(ingredientes[randomIndex], transform.position, Quaternion.identity);
        currentIngredient.GetComponent<Rigidbody2D>().isKinematic = true; // Dejar el ingrediente inmóvil por ahora

        currentIngredient.transform.position += new Vector3(0, 0, currentIngredient.transform.position.y * 0.01f);

        // Registrar el ingrediente en LogicaBotones
        LogicaBotones logicaBotones = FindObjectOfType<LogicaBotones>();
        if (logicaBotones != null)
        {
            logicaBotones.RegistrarIngrediente(currentIngredient);
        }

        isIngredientMoving = true; // Hacer que el ingrediente comience a moverse
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
        moveSpeed = 0f; // Detener el movimiento

        yield return new WaitForSeconds(tiempo);

        isPaused = false;
        moveSpeed = originalMoveSpeed; // Restaurar la velocidad original correctamente
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

    // Nuevo método: Reiniciar posición del Spawn Point y contador de ingredientes
    public void ResetearSpawnPoint()
    {
        gameObject.SetActive(true); // Reactivar el SpawnPoint si estaba desactivado

        transform.position = posicionInicial; // Restaurar la posición inicial
        ingredientesContados = 0; // Reiniciar contador
        hasFinalIngredientSpawned = false; // Resetear el flag del ingrediente final
        isSpawningDisabled = false; // Asegurar que el spawn esté habilitado
        ingredientesDestruidos = 0; // Reiniciar el contador de ingredientes destruidos

        // Eliminar el ingrediente actual si existe
        if (currentIngredient != null)
        {
            Destroy(currentIngredient);
            currentIngredient = null;
        }

        // FORZAR UN NUEVO INGREDIENTE
        StartCoroutine(ForzarSpawn());

        Debug.Log("Spawn Point reiniciado. Contador de ingredientes destruidos en 0.");
    }

    private IEnumerator ForzarSpawn()
    {
        yield return new WaitForSeconds(0.2f); // Pequeña espera para asegurar que el reset se complete
        SpawnIngredient();
    }

    void OnEnable()
    {
        StartCoroutine(EsperarYVerificarIngrediente());
    }

    IEnumerator EsperarYVerificarIngrediente()
    {
        yield return new WaitForSeconds(0.2f); // Esperar 0.2 segundos

        if (currentIngredient == null) // Si no hay ingrediente, generamos uno nuevo
        {
            SpawnIngredient();
            Debug.Log("No había ingrediente en el SpawnPoint. Se generó uno nuevo.");
        }
    }

    void VerificarYSpawnIngrediente()
    {
        if (currentIngredient == null) // Si no hay ingrediente, generar uno nuevo
        {
            SpawnIngredient();
            Debug.Log("No había ingrediente en el SpawnPoint. Se generó uno nuevo.");
        }
    }
}