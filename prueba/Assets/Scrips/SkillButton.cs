using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public class SkillButton : MonoBehaviour
    {
        [Header("Componentes")]
        public Image abilityImage; // Imagen del botón
        public Image effectImage;

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
        
        public void UseAbility()
        {
            if (isReady)
            {
                GameplayController.instance.ActivateFreeze();
            effectImage.gameObject.SetActive(true);
            StartCoroutine(AnimateEffect());
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
    private IEnumerator AnimateEffect()
    {
        effectImage.transform.localScale = Vector3.zero;
        effectImage.gameObject.SetActive(true);

        float scaleDuration = 0.5f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one * 55;

        // Animar entrada (escalado)
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            effectImage.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        effectImage.transform.localScale = endScale;

        // Permanecer activo durante la duración de la habilidad
        yield return new WaitForSeconds(GameplayController.instance.skillDuration);

        // Ocultar el efecto
        effectImage.gameObject.SetActive(false);
    }
}
