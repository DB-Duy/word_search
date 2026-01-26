using Shared.Core.IoC;
using Shared.Service.Audio;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Shared.View.Audio
{
    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    public class AudioForButton : IoCMonoBehavior
    {
        [Inject] private IAudioService _audioService;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => _audioService.PlayButton());
        }
    }
}