using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    public SpawnPoint spawnPoint; // Referencia al SpawnPoint
    public Button skillButton;    // Bot�n en la UI para activar la habilidad
    public float skillDuration = 5f; // Duraci�n de la habilidad en segundos
    public float cooldownTime = 30f; // Tiempo de enfriamiento en segundos

    private bool isCooldown = false; // Indica si la habilidad est� en enfriamiento

    void Start()
    {
        // Verifica que las referencias est�n asignadas
        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint no est� asignado en Skill.");
        }

        if (skillButton != null)
        {
            skillButton.onClick.AddListener(ActivateSkill); // Asocia el bot�n a la habilidad
        }
        else
        {
            Debug.LogError("SkillButton no est� asignado en Skill.");
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
            Debug.Log("Habilidad bloqueada porque el ingrediente ya cay� o no hay ingrediente.");
            return;
        }

        Debug.Log("Habilidad ACTIVADA correctamente.");
        StartCoroutine(HandleSkill());
    }

    private IEnumerator HandleSkill()
    {
        // Activa la habilidad y pausa el movimiento del SpawnPoint
        if (spawnPoint != null)
        {
            spawnPoint.StartPauseMovement();
        }

        // Desactiva temporalmente el bot�n y establece el enfriamiento
        if (skillButton != null)
        {
            skillButton.interactable = false;
        }

        isCooldown = true;

        // Espera el tiempo de duraci�n de la habilidad
        yield return new WaitForSeconds(skillDuration);

        // Asegurar que el �ltimo ingrediente se suelta correctamente
        GameObject ingredienteActual = GameObject.FindGameObjectWithTag("ingredientes"); // Busca el ingrediente por etiqueta

        if (ingredienteActual != null)
        {
            ingredienteActual.transform.parent = null; // Desvincular del SpawnPoint

            // Verifica si tiene el script antes de llamar a sus m�todos
            Ingredientes ingredienteScript = ingredienteActual.GetComponent<Ingredientes>();
            if (ingredienteScript != null)
            {
                ingredienteScript.SoltarIngrediente(); // Detiene el seguimiento del SpawnPoint
            }
            else
            {
                Debug.LogWarning("El ingrediente encontrado no tiene el script 'Ingrediente'.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontr� ning�n ingrediente para soltar.");
        }

        // Reactivar el movimiento del SpawnPoint
        if (spawnPoint != null)
        {
            spawnPoint.StopPauseMovement();
        }

        // Espera el tiempo de enfriamiento restante
        yield return new WaitForSeconds(cooldownTime - skillDuration);

        // Habilidad lista para usar de nuevo
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
            skillButton.interactable = true; // Reactivar el bot�n de la habilidad
        }

        if (spawnPoint != null)
        {
            spawnPoint.StopPauseMovement();  // Asegurar que el SpawnPoint no est� pausado
            spawnPoint.ResetearSpawnPoint(); // Restablecer el SpawnPoint
        }

        Debug.Log("Habilidad y SpawnPoint reiniciados completamente.");
    }

    private bool PuedeActivarHabilidad()
    {
        return spawnPoint != null && spawnPoint.PuedeActivarHabilidad();
    }
}
