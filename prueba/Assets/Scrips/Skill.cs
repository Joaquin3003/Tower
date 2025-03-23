using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    public SpawnPoint spawnPoint; // Referencia al SpawnPoint
    public Button skillButton;    // Botón en la UI para activar la habilidad
    public float skillDuration = 5f; // Duración de la habilidad en segundos
    public float cooldownTime = 30f; // Tiempo de enfriamiento en segundos

    private bool isCooldown = false; // Indica si la habilidad está en enfriamiento

    void Start()
    {
        // Verifica que las referencias estén asignadas
        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint no está asignado en Skill.");
        }

        if (skillButton != null)
        {
            skillButton.onClick.AddListener(ActivateSkill); // Asocia el botón a la habilidad
        }
        else
        {
            Debug.LogError("SkillButton no está asignado en Skill.");
        }
    }

    public void ActivateSkill()
    {
        if (isCooldown)
        {
            Debug.Log("Habilidad en enfriamiento. No se puede activar.");
            return;
        }

        if (!PuedeActivarHabilidad())
        {
            Debug.Log("Habilidad bloqueada porque el ingrediente ya cayó o no hay ingrediente.");
            return;
        }

        Debug.Log("Habilidad ACTIVADA correctamente.");
        StartCoroutine(HandleSkill());
    }

    private IEnumerator HandleSkill()
    {
        if (spawnPoint != null)
        {
            spawnPoint.StartPauseMovement();
        }

        if (skillButton != null)
        {
            skillButton.interactable = false;
        }

        isCooldown = true;
        yield return new WaitForSeconds(skillDuration);

        if (spawnPoint != null)
        {
            spawnPoint.StopPauseMovement();
        }

        yield return new WaitForSeconds(cooldownTime - skillDuration);

        isCooldown = false;
        if (skillButton != null)
        {
            skillButton.interactable = true;
        }
    }

    public void ResetSkill()
    {
        StopAllCoroutines(); // Detener cualquier corutina activa de la habilidad

        isCooldown = false;

        if (skillButton != null)
        {
            skillButton.interactable = true; // Reactivar el botón de la habilidad
        }

        if (spawnPoint != null)
        {
            spawnPoint.StopPauseMovement();  // Asegurar que el SpawnPoint no esté pausado
            spawnPoint.ResetearSpawnPoint(); // Restablecer el SpawnPoint
        }

        Debug.Log("Habilidad y SpawnPoint reiniciados completamente.");
    }

    public bool PuedeActivarHabilidad()
    {
        if (spawnPoint != null && spawnPoint.currentIngredient != null && spawnPoint.isIngredientMoving)
        {
            Debug.Log("Habilidad puede activarse: Ingrediente en Spawn y aún no ha sido soltado.");
            return true;
        }

        Debug.Log("Habilidad bloqueada: No hay ingrediente en el Spawn o ya fue soltado.");
        return false;
    }

    void TerminarHabilidad()
    {
        // Encuentra el último ingrediente en pantalla
        FinalIngredient ingredienteFinal = FindObjectOfType<FinalIngredient>();

        if (ingredienteFinal != null)
        {
            Rigidbody2D rb = ingredienteFinal.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.gravityScale = 1f;
                rb.constraints = RigidbodyConstraints2D.None;
            }
        }

        Debug.Log("Habilidad desactivada correctamente.");
    }
}
