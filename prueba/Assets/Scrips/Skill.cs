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
        // Solo permite activar la habilidad si no está en enfriamiento
        if (!isCooldown)
        {
            StartCoroutine(HandleSkill());
        }
    }

    private IEnumerator HandleSkill()
    {
        // Activa la habilidad
        if (spawnPoint != null)
        {
            spawnPoint.StartPauseMovement(); // Detiene el movimiento
        }

        // Desactiva temporalmente el botón y establece el enfriamiento
        if (skillButton != null)
        {
            skillButton.interactable = false;
        }

        isCooldown = true;

        // Espera el tiempo de duración de la habilidad
        yield return new WaitForSeconds(skillDuration);

        // Restaura el movimiento
        if (spawnPoint != null)
        {
            spawnPoint.StopPauseMovement(); // Restaura el movimiento
        }

        // Espera el tiempo de enfriamiento
        yield return new WaitForSeconds(cooldownTime - skillDuration);

        // Habilidad lista para usar de nuevo
        isCooldown = false;
        if (skillButton != null)
        {
            skillButton.interactable = true;
        }
    }
}
