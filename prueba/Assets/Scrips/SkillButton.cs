using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public Sprite[] cooldownSprites; // Array de sprites para el bot�n
    public Button skillButton;       // Referencia al bot�n
    public float cooldownTime = 30f; // Tiempo de enfriamiento en segundos

    private float cooldownTimer = 0f; // Temporizador interno
    private bool isCoolingDown = false; // Controla si el bot�n est� en enfriamiento
    private Image buttonImage;        // Imagen del bot�n

    public float skillDuration = 5f; // Duraci�n del efecto de la habilidad
    private SpawnPoint spawnPoint;   // Referencia al SpawnPoint

    void Start()
    {
        // Buscar autom�ticamente el SpawnPoint en la escena
        spawnPoint = FindObjectOfType<SpawnPoint>();

        if (spawnPoint == null)
        {
            Debug.LogError("No se encontr� el SpawnPoint en la escena.");
        }

        // Verifica las referencias
        if (cooldownSprites == null || cooldownSprites.Length == 0)
        {
            Debug.LogError("Los sprites de cooldown no est�n asignados.");
        }

        if (skillButton == null)
        {
            Debug.LogError("El bot�n no est� asignado.");
        }
        else
        {
            buttonImage = skillButton.GetComponent<Image>();
            if (buttonImage == null)
            {
                Debug.LogError("El componente Image del bot�n no est� asignado.");
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

            // Calcula el �ndice del sprite basado en el progreso del enfriamiento
            int spriteIndex = Mathf.Clamp((int)(cooldownTimer / cooldownTime * cooldownSprites.Length), 0, cooldownSprites.Length - 1);

            // Actualiza el sprite del bot�n
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

    // L�gica para activar la habilidad
    private void ActivateSkill()
    {
        if (isCoolingDown) return; // No permite usar la habilidad si est� en enfriamiento

        Debug.Log("Habilidad activada! Deteniendo el SpawnPoint por " + skillDuration + " segundos.");

        if (spawnPoint != null)
        {
            spawnPoint.DetenerSpawnPorTiempo(skillDuration); // Detiene el SpawnPoint por la duraci�n de la habilidad
        }

        StartCooldown();
    }

    // Inicia el enfriamiento
    private void StartCooldown()
    {
        isCoolingDown = true;
        cooldownTimer = 0f;
        skillButton.interactable = false; // Desactiva el clic mientras est� en enfriamiento
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
            buttonImage.sprite = cooldownSprites[cooldownSprites.Length - 1]; // �ltimo sprite
        }
    }
}
