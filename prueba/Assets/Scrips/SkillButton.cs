using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public Sprite[] cooldownSprites; // Array de sprites para el botón
    public Button skillButton;       // Referencia al botón
    public float cooldownTime = 30f; // Tiempo de enfriamiento en segundos

    private float cooldownTimer = 0f; // Temporizador interno
    private bool isCoolingDown = false; // Controla si el botón está en enfriamiento
    private Image buttonImage;        // Imagen del botón

    public float skillDuration = 5f; // Duración del efecto de la habilidad
    private SpawnPoint spawnPoint;   // Referencia al SpawnPoint

    void Start()
    {
        // Buscar automáticamente el SpawnPoint en la escena
        spawnPoint = FindObjectOfType<SpawnPoint>();

        if (spawnPoint == null)
        {
            Debug.LogError("No se encontró el SpawnPoint en la escena.");
        }

        // Verifica las referencias
        if (cooldownSprites == null || cooldownSprites.Length == 0)
        {
            Debug.LogError("Los sprites de cooldown no están asignados.");
        }

        if (skillButton == null)
        {
            Debug.LogError("El botón no está asignado.");
        }
        else
        {
            buttonImage = skillButton.GetComponent<Image>();
            if (buttonImage == null)
            {
                Debug.LogError("El componente Image del botón no está asignado.");
            }
        }

        ResetButtonSprite();
        skillButton.onClick.AddListener(ActivateSkill); // Asocia el evento de clic
    }

    void Update()
    {
        if (isCoolingDown)
        {
            // Incrementa el temporizador
            cooldownTimer += Time.deltaTime;

            // Calcula el índice del sprite basado en el progreso del enfriamiento
            int spriteIndex = Mathf.Clamp((int)(cooldownTimer / cooldownTime * cooldownSprites.Length), 0, cooldownSprites.Length - 1);

            // Actualiza el sprite del botón
            if (buttonImage != null)
            {
                buttonImage.sprite = cooldownSprites[spriteIndex];
            }

            // Verifica si el enfriamiento ha terminado
            if (cooldownTimer >= cooldownTime)
            {
                EndCooldown();
            }
        }
    }

    // Lógica para activar la habilidad
    private void ActivateSkill()
    {
        if (isCoolingDown) return; // No permite usar la habilidad si está en enfriamiento

        Debug.Log("Habilidad activada! Deteniendo el SpawnPoint por " + skillDuration + " segundos.");

        if (spawnPoint != null)
        {
            spawnPoint.DetenerSpawnPorTiempo(skillDuration); // Detiene el SpawnPoint por la duración de la habilidad
        }

        StartCooldown();
    }

    // Inicia el enfriamiento
    private void StartCooldown()
    {
        isCoolingDown = true;
        cooldownTimer = 0f;
        skillButton.interactable = false; // Desactiva el clic mientras esté en enfriamiento
    }

    // Finaliza el enfriamiento
    private void EndCooldown()
    {
        isCoolingDown = false;
        skillButton.interactable = true; // Reactiva el clic
        ResetButtonSprite();
    }

    // Restablece el sprite al inicial (habilidad lista para usar)
    private void ResetButtonSprite()
    {
        if (cooldownSprites.Length > 0 && buttonImage != null)
        {
            buttonImage.sprite = cooldownSprites[cooldownSprites.Length - 1]; // Último sprite
        }
    }
}
