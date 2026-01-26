#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
using Lofelt.NiceVibrations;
#endif
using Shared.Core.IoC;
using Shared.Repository.Vibration;
using UnityEngine.Events;
using Zenject;

namespace Shared.Service.Vibration
{
    [Service]
    public class VibrationService : IVibrationService, IInitializable
    {
        [Inject] private VibrationEnableRepository _enableRepository;

        public bool Enable
        {
            get => _enableRepository.Get();
            set
            {
                _enableRepository.Set(value);
#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
                HapticController.hapticsEnabled = value;
#endif
            }
        }

        public UnityEvent<bool> OnSettingChanged
        {
            get => _enableRepository.onValueChanged;
            set { }
        }

        public void Initialize()
        {
#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
            HapticController.hapticsEnabled = _enableRepository.Get();
#endif
        }

#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
        public void PlayPreset(HapticPatterns.PresetType type) => HapticPatterns.PlayPreset(type);
        
        public void PlaySelection() => HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);

        public void PlayWarning() => HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);

        public void PlayFailure() => HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);
#else
        public void PlaySelection(){}

        public void PlayWarning(){}

        public void PlayFailure(){}
#endif
    }
}