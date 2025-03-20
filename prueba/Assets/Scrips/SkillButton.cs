using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public Sprite restartSprite; // Imagen para cuando el juego se reinicie
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
        if (skillButton != null)
        {
            buttonImage = skillButton.GetComponent<Image>();
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
            cooldownTimer += Time.deltaTime;

            int spriteIndex = Mathf.Clamp((int)(cooldownTimer / cooldownTime * cooldownSprites.Length), 0, cooldownSprites.Length - 1);

            if (buttonImage != null)
            {
                buttonImage.sprite = cooldownSprites[spriteIndex];
            }

            if (cooldownTimer >= cooldownTime)
            {
                EndCooldown();
            }
        }
    }

    // L�gica para activar la habilidad
    private void ActivateSkill()
    {
        if (isCoolingDown) return;

        if (spawnPoint != null)
        {
            spawnPoint.DetenerSpawnPorTiempo(skillDuration);
        }

        StartCooldown();
    }

    private void StartCooldown()
    {
        isCoolingDown = true;
        cooldownTimer = 0f;
        skillButton.interactable = false;
    }

    private void EndCooldown()
    {
        isCoolingDown = false;
        skillButton.interactable = true;
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

    public void ResetSkillButton()
    {
        isCoolingDown = false;   // Se detiene el cooldown
        cooldownTimer = 0f;      //  Se resetea el temporizador
        skillButton.interactable = true;

        if (buttonImage != null)
        {
            if (restartSprite != null)
            {
                buttonImage.sprite = restartSprite; //  Imagen que quieres mostrar tras el reinicio
            }
            else if (cooldownSprites.Length > 0)
            {
                buttonImage.sprite = cooldownSprites[cooldownSprites.Length - 1]; // �ltimo sprite (habilidad cargada)
            }
        }

        Debug.Log("Bot�n de habilidad reiniciado correctamente.");
    }
}
