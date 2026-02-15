using Coffee.UIExtensions;
using PrimeTween;
using Shared;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace View.Puzzle
{
    public class CompletedEffectsView : MonoBehaviour, ISharedUtility
    {
        private static readonly string[] CompletedTexts = new string[]
        {
            "AMAZING",
            "GREAT",
            "PERFECT"
        };
        private const float CompletedEffectsChance = 0.6f;

        [SerializeField] private UIParticle _completedParticle;
        [SerializeField] private CanvasGroup _banner;
        [SerializeField] private TextMeshProUGUI _completedText;

        public static bool IsPlayingCompletedEffects { get; private set; }
        
        private Sequence _completedSequence;

        private void Start()
        {
            _banner.alpha = 0;
        }

        public void TryPlayCompletedEffects(Vector3 particlePos)
        {
            _completedParticle.transform.position = particlePos;
            _completedParticle.Play();

            if (!_completedSequence.isAlive && Random.value < CompletedEffectsChance)
            {
                IsPlayingCompletedEffects = true;
                _completedText.SetText(CompletedTexts[Random.Range(0, CompletedTexts.Length)]);
                _banner.alpha = 0;
                _banner.transform.localScale = new Vector3(0, 1, 1);
                _completedSequence = Sequence.Create()
                    .Group(Tween.Alpha(_banner, 1, 0.3f, Ease.OutQuad))
                    .Group(Tween.ScaleX(_banner.transform, 1, 0.3f, Ease.OutBack))
                    .Chain(Tween.Scale(_banner.transform, new Vector3(1.2f, 1.2f, 1.2f), 0.3f, Ease.OutBack))
                    .Chain(Tween.Alpha(_banner, 0, 0.3f, Ease.InQuad))
                    .OnComplete(this, (x) => x.OnEffectsCompleted());
            }
        }

        private void OnEffectsCompleted()
        {
            IsPlayingCompletedEffects = false;
        }
    }
}