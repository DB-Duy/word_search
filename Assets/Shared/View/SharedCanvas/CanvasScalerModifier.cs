using Shared.Utilities.SharedBehaviour;
using Shared.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Shared.View.SharedCanvas
{
    [RequireComponent(typeof(CanvasScaler))]
    [DisallowMultipleComponent]
    public class CanvasScalerModifier : SharedMonoBehaviour, ISharedUtility
    {
        private const string Tag = "CanvasScalerModifier";
        private CanvasScaler _scaler;
        
        private void Start()
        {
            SharedLogger.Log($"{Tag}->Start");
            if (!this.IsTablet()) return;
            if (_scaler == null)
                _scaler = GetComponent<CanvasScaler>();
            _scaler.matchWidthOrHeight = 1;
        }
    }
}