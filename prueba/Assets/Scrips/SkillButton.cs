using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public class SkillButton : MonoBehaviour
    {
        [Header("Componentes")]
        public Image abilityImage; // Imagen del botón

        [Header("Array del cooldown")]
        public Sprite[] cooldownSprites; // Array de sprites para el botón

        [Header("Parámetros de Cooldown")]
        public float fillTime = 5f; // Tiempo que tarda en llenarse
        private float timer = 0f;
        private bool isReady = false; // Indica si la habilidad está lista

        public float disabledAlpha = 0.5f; 
        public float enabledAlpha = 1f;

    void Start()
        {
            ResetAbility();
        }
        void Update()
        {
            if (!isReady)
            {
                timer += Time.deltaTime;
                int frameIndex = Mathf.FloorToInt((timer / fillTime) * cooldownSprites.Length);
                frameIndex = Mathf.Clamp(frameIndex, 0, cooldownSprites.Length - 1);
                abilityImage.sprite = cooldownSprites[frameIndex];

                if (timer >= fillTime)
                {
                    isReady = true;
                    SetButtonAlpha(enabledAlpha);
                }
            }
        }
        // Lógica para activar la habilidad
        public void UseAbility()
        {
            if (isReady)
            {
                GameplayController.instance.ActivateFreeze();
                ResetAbility();
            }
        }
        private void ResetAbility()
        {
            timer = 0f;
            isReady = false;
            abilityImage.sprite = cooldownSprites[0]; // Reinicia la animación
            SetButtonAlpha(disabledAlpha);
    }

    private void SetButtonAlpha(float alpha)
    {
        Color tempColor = abilityImage.color;
        tempColor.a = alpha;
        abilityImage.color = tempColor;
    }
}
