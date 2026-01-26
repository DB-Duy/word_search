using System.Collections.Generic;
using Shared.Utils;

namespace Shared.Core.IoC.UnityLifeCycle
{
    public class SequenceUnityOnApplicationPause : IUnityOnApplicationPause
    {
        private const string Tag = "SequenceUnityOnApplicationPause";
        private readonly HashSet<IUnityOnApplicationPause> _onApplicationPauses = new();

        public void Add(IUnityOnApplicationPause u)
        {
            _onApplicationPauses.Add(u);
            SharedLogger.LogJson(SharedLogTag.Ioc, $"{Tag}->Add", nameof(u), u.GetType().FullName);
        }

        public void Remove(IUnityOnApplicationPause u)
        {
            _onApplicationPauses.Remove(u);
            SharedLogger.LogJson(SharedLogTag.Ioc, $"{Tag}->Remove", nameof(u), u.GetType().FullName);
        }

        public void OnApplicationPause(bool pauseStatus)
        {
            foreach (var p in _onApplicationPauses) p.OnApplicationPause(pauseStatus);
        }
    }
}