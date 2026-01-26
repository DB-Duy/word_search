using System.Collections;
using DG.Tweening;
using Shared.Utilities.SharedBehaviour;
using UnityEngine;
using UnityEngine.UI;

namespace Shared.View.Halo
{
    public class DimmingEffect : SharedMonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Image mainImage; // The full-screen image (image1)

        [SerializeField] private Image targetImage; // The smaller image used as the mask (image2)

        [Header("Effect Settings")] [SerializeField]
        private float fillSpeed = 0.5f; // Speed of the fill effect

        private Material fillMaterial; // Material dynamically retrieved from mainImage
        private Coroutine fillCoroutine; // Reference to the running coroutine

        void Start()
        {
            // Validate references
            if (mainImage == null || targetImage == null)
            {
                Debug.LogError("Main Image or Target Image is not assigned.");
                return;
            }

            // Retrieve the material from the main image
            fillMaterial = mainImage.material;

            if (fillMaterial == null)
            {
                Debug.LogError("Main Image does not have a material assigned.");
                return;
            }

            // Assign the target image's texture to the shader's _MaskTex
            Sprite targetSprite = targetImage.sprite;
            if (targetSprite != null)
            {
                fillMaterial.SetTexture("_MaskTex", targetSprite.texture);
            }
            else
            {
                Debug.LogWarning("Target Image does not have a sprite assigned. The mask will not work.");
            }

            // Initialize the fill amount to 0
            fillMaterial.SetFloat("_FillAmount", 0);
        }

        public void StartFill()
        {
            // Stop any ongoing fill effect before starting a new one
            if (fillCoroutine != null)
            {
                StopCoroutine(fillCoroutine);
            }

            fillCoroutine = StartCoroutine(FillEffectCoroutine());
        }

        public void ResetFill()
        {
            if (fillCoroutine != null)
            {
                StopCoroutine(fillCoroutine);
            }

            // Reset the fill amount to 0
            fillMaterial.SetFloat("_FillAmount", 0);
        }

        private IEnumerator FillEffectCoroutine()
        {
            float fillAmount = 0;

            while (fillAmount < 1)
            {
                fillAmount += Time.deltaTime * fillSpeed;
                fillMaterial.SetFloat("_FillAmount", fillAmount);
                Debug.Log("FillEffectCoroutine" + fillAmount);
                yield return null;
            }

            // Ensure the final value is exactly 1
            fillMaterial.SetFloat("_FillAmount", 1);
        }

        public void UpdateTargetImage(Image newTargetImage)
        {
            if (newTargetImage != null)
            {
                targetImage = newTargetImage;
                Sprite targetSprite = targetImage.sprite;

                if (targetSprite != null)
                {
                    fillMaterial.SetTexture("_MaskTex", targetSprite.texture);
                }
            }
        }

#if UNITY_EDITOR
        public override void GUIEditor()
        {
            if (GUILayout.Button("Show"))
                StartFill();
        }
#endif
    }
}