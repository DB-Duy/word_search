using UnityEngine;

namespace Shared.Core.IoC.UnityLifeCycle
{
    public class UnityLifeCycle : MonoBehaviour
    {
        public SequenceUnityUpdate SequenceUnityUpdate { get; } = new();
        public SequenceUnityOnApplicationPause SequenceUnityOnApplicationPause { get; } = new();

        private void Update() => SequenceUnityUpdate.Update();
        private void OnApplicationPause(bool pauseStatus) => SequenceUnityOnApplicationPause.OnApplicationPause(pauseStatus);
    }
}