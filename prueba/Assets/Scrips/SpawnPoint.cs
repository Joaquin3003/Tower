using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Properties")]
    public float moveSpeed = 2f;
    public float horizontalLimit = 3f;
    public float initialHeightIncrement = 0.01f;  
    private float currentHeightIncrement = 0f;

    private int ingredientesContados = 0;
    
    public GameObject currentIngredient;

    [Header("Prefabs")]
    public GameObject[] prefabIngrediente;
    public GameObject finalIngredientPrefab;

    public bool isIngredientMoving = false;
    private bool isPaused = false;

    public Transform spawn;
    private bool isSpawningDisabled = false;
    private bool hasFinalIngredientSpawned = false;

    [Header("Score")]
    private ScoreManager scoreManager;
    private float originalMoveSpeed;
    public int ingredientesDestruidos = 0; // Contador de ingredientes destruidos
   
    private bool isSkillActive = false;
    public bool shouldReleaseIngredient = false;

    // Guardar la posición inicial del Spawn Point
    private Vector3 posicionInicial;

    void Start()
    {
        currentHeightIncrement = 0f;
        moveSpeed = originalMoveSpeed = 1f;        
        scoreManager = FindObjectOfType<ScoreManager>();
        
        posicionInicial = transform.position; // Guardamos la posición inicial del Spawn Point

        // Reiniciar contador de ingredientes destruidos
        ingredientesDestruidos = 0;
        //Invoke("VerificarYSpawnIngrediente", 0.2f);

        SpawnIngredient();
    }

    void Update()
    {
        if (LogicaBotones.juegoPausadoGlobal) return;
        if (!isPaused)
        {           
            transform.position += Vector3.right * moveSpeed * Time.deltaTime; // Movimiento del spawn point

            if (transform.position.x >= horizontalLimit) // Corrección del rebote en los límites
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

        // Movimiento del ingrediente si está siendo soltado
        if (currentIngredient != null && isIngredientMoving)
        {
            currentIngredient.transform.position = transform.position;
        }

        /*if (shouldReleaseIngredient && isIngredientMoving)
        {
            ReleaseIngredient();
        }*/

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

    public void SetSkillActive(bool active)
    {
        isSkillActive = active;

        if (!active)
        {
            shouldReleaseIngredient = false; // La habilidad termina, pero el ingrediente no se soltará automáticamente.
            Debug.Log("Habilidad desactivada, pero el ingrediente no se soltará.");
        }
    }

    public void FijarIngredienteEnSpawn()
    {
        if (currentIngredient != null)
        {
            Rigidbody2D rb = currentIngredient.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0;  // Evita que el ingrediente caiga solo
                //rb.velocity = Vector2.zero;  // Detiene cualquier movimiento
            }

            currentIngredient.transform.position = transform.position; // Mantiene el ingrediente en el spawn
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
        if (ingredient == null) return;

        Debug.Log("Ingrediente aterrizó y se reinicia el proceso.");
        //currentIngredient = null;
        isIngredientMoving = false;

        SpawnIngredient(); // Generamos el siguiente ingrediente
    }

    /*public IEnumerator SpawnNextIngredient()
    {
        yield return new WaitForSeconds(0.1f);
        SpawnIngredient();
    }*/

    public void SpawnIngredient()
    {
        Debug.Log("[SpawnIngredient] Llamado. currentIngredient: " + currentIngredient + ", isIngredientMoving: " + isIngredientMoving);
        if (prefabIngrediente == null || prefabIngrediente.Length == 0)
        {
            Debug.LogError("❌ Error: No hay ingredientes asignados en el array.");
            return;
        }
        if (currentIngredient != null || isIngredientMoving) return;  // Si ya hay un ingrediente en movimiento, no generamos otro
        
        GameObject prefabToSpawn = prefabIngrediente[Random.Range(0, prefabIngrediente.Length)];
       
        if (currentIngredient == null)
        {
            currentIngredient = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        };
        

        // Instanciamos el nuevo ingrediente
       //Debug.Log("🔍 Spawn en: " + transform.position);
        //Debug.Log("🔍 Posicion Inicial: " + posicionInicial);
        
        FijarIngredienteEnSpawn();
        Rigidbody2D rb = currentIngredient.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;  // Impide que el ingrediente caiga hasta que se suelte            
        }
        
        LogicaBotones logicaBotones = FindObjectOfType<LogicaBotones>();
        if (logicaBotones != null)
        {
            logicaBotones.RegistrarIngrediente(currentIngredient);  // Registramos el ingrediente
        }

        isIngredientMoving = true;  // Marcamos que el ingrediente está en movimiento
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
            currentIngredient.GetComponent<Rigidbody2D>().gravityScale = 1;  // Permitimos que el ingrediente se mueva
            currentIngredient = null;  // Limpiamos el ingrediente actual

            ingredientesContados++;  // Contamos el ingrediente

            // Si hemos llegado a 9 ingredientes, actualizamos el incremento de altura
            if (ingredientesContados == 9)
            {
                currentHeightIncrement = initialHeightIncrement;
            }
            else if (ingredientesContados > 9)
            {
                transform.position += new Vector3(0, currentHeightIncrement, 0);  // Incrementamos la altura
            }
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
            //currentIngredient = null;
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

    public void SetShouldReleaseIngredient(bool value)
    {
        shouldReleaseIngredient = value;
        if (!isIngredientMoving) return;
    }
}