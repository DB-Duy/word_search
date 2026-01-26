using UnityEngine.Events;

namespace Shared.Service.Vibration
{
    public interface IVibrationService
    {
        bool Enable { get; set; }
        UnityEvent<bool> OnSettingChanged { get; set; }   
        void PlaySelection();
        void PlayWarning();
        void PlayFailure();
#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
        public void PlayPreset(Lofelt.NiceVibrations.HapticPatterns.PresetType type) => Lofelt.NiceVibrations.HapticPatterns.PlayPreset(type);
#endif 
    }
}