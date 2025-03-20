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
        // Activa la habilidad y pausa el movimiento del SpawnPoint
        if (spawnPoint != null)
        {
            spawnPoint.StartPauseMovement();
        }

        // Desactiva temporalmente el botón y establece el enfriamiento
        if (skillButton != null)
        {
            skillButton.interactable = false;
        }

        isCooldown = true;

        // Espera el tiempo de duración de la habilidad
        yield return new WaitForSeconds(skillDuration);

        // Asegurar que el último ingrediente se suelta correctamente
        GameObject ingredienteActual = GameObject.FindGameObjectWithTag("ingredientes"); // Busca el ingrediente por etiqueta

        if (ingredienteActual != null)
        {
            ingredienteActual.transform.parent = null; // Desvincular del SpawnPoint

            // Verifica si tiene el script antes de llamar a sus métodos
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
            Debug.LogWarning("No se encontró ningún ingrediente para soltar.");
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
            skillButton.interactable = true; // Reactivar el botón de la habilidad
        }

        if (spawnPoint != null)
        {
            spawnPoint.StopPauseMovement();  // Asegurar que el SpawnPoint no esté pausado
            spawnPoint.ResetearSpawnPoint(); // Restablecer el SpawnPoint
        }

        Debug.Log("Habilidad y SpawnPoint reiniciados completamente.");
    }

    private bool PuedeActivarHabilidad()
    {
        return spawnPoint != null && spawnPoint.PuedeActivarHabilidad();
    }
}
