using DG.Tweening;
using UnityEngine;

namespace Shared.View.Halo
{
    public class HaloFinger : MonoBehaviour
    {
        [SerializeField] private RectTransform fingerTransform; // RectTransform of the finger object
        [SerializeField] private CanvasGroup fingerCanvasGroup; // CanvasGroup for fade
        [SerializeField] private float fadeDuration = 0.8f; // Duration for fade-in/out
        [SerializeField] private float tapDistance = 20f; // Distance the finger moves for tapping
        [SerializeField] private float tapDuration = 0.2f; // Time for one tap
        [SerializeField] private int tapCount = 2; // Number of taps in the animation

        private Vector2 tapDirection = Vector2.down; // Default tap direction (down)
        private Sequence animationSequence;

        public void PlayFingerAnimation(RectTransform targetButton)
        {
            // Ensure CanvasGroup exists
            if (!fingerCanvasGroup)
            {
                fingerCanvasGroup = gameObject.GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
            }

            // Reset alpha
            fingerCanvasGroup.alpha = 0;

            // Adjust finger position and orientation
            AdjustFingerOrientation(targetButton);

            // Create the full sequence
            animationSequence = DOTween.Sequence();

            // Fade-in effect
            animationSequence.Append(fingerCanvasGroup.DOFade(1, fadeDuration));

            // Tap-tap animation
            for (int i = 0; i < tapCount; i++)
            {
                animationSequence.Append(fingerTransform.DOAnchorPos(tapDirection * tapDistance, tapDuration).SetEase(Ease.OutQuad)); // Move in tap direction
                animationSequence.Append(fingerTransform.DOAnchorPos(Vector2.zero, tapDuration).SetEase(Ease.InQuad)); // Return to original
            }

            // Loop fade after tap-tap
            animationSequence.AppendInterval(0.5f) // Pause briefly
                .Append(fingerCanvasGroup.DOFade(0, fadeDuration))
                .SetLoops(-1, LoopType.Restart); // Loop the sequence indefinitely
        }

        public void StopFingerAnimation()
        {
            // Stop the animation
            if (animationSequence != null)
            {
                animationSequence.Kill();
                animationSequence = null;
            }

            // Reset alpha
            fingerCanvasGroup.alpha = 0;
        }

        private void AdjustFingerOrientation(RectTransform targetButton)
        {
            // Position the finger near the target button
            Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, targetButton.position);

            // Determine direction and rotation based on position
            if (screenPosition.y > Screen.height / 2)
            {
                // Button is at the top of the screen
                tapDirection = Vector2.up;
                fingerTransform.rotation = Quaternion.Euler(0, 0, 180); // Pointing up
            }
            else if (screenPosition.y <= Screen.height / 2)
            {
                // Button is at the bottom of the screen
                tapDirection = Vector2.down;
                fingerTransform.rotation = Quaternion.Euler(0, 0, 0); // Pointing down
            }

            if (screenPosition.x < Screen.width / 2)
            {
                // Button is on the left side
                fingerTransform.rotation *= Quaternion.Euler(0, 0, -90); // Adjust for pointing left
            }
            else if (screenPosition.x > Screen.width / 2)
            {
                // Button is on the right side
                fingerTransform.rotation *= Quaternion.Euler(0, 0, 90); // Adjust for pointing right
            }
        }
    }
}