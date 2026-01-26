using System;
using Shared.Utils;

namespace Shared.Service.Fps.Handlers
{
    public class LowFpsCountHandler : ISharedFpsHandler
    {
        private const string Tag = "LowFpsCountHandler";
        private int UnderFps { get; }
        private int TriggerCount { get; }
        private int _lowCount;
        private bool _isTrigger;
        private readonly Action _action;
        

        public LowFpsCountHandler(int underFps, int triggerCount, Action action)
        {
            UnderFps = underFps;
            TriggerCount = triggerCount;
            _action = action;
        }

        public void Handle(int framePerSecond)
        {
            if(_isTrigger) return;
            if (framePerSecond > UnderFps) return;
            _lowCount++;
            SharedLogger.Log($"{Tag}-> _lowCount={_lowCount}, framePerSecond={framePerSecond}");
            if (_lowCount < TriggerCount) return;
            _action?.Invoke();
            _isTrigger = true;
        }
    }
}