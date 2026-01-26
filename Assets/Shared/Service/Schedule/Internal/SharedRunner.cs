using System;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.Schedule.Internal
{
    public class SharedRunner : ISharedRunner
    {
        private const string Tag = "SharedRunner";

        public string TaskName { get; }
        private float DelayInSeconds { get; }
        public bool Forever { get; }
        public bool IsStarted { get; private set; }
        private readonly Action _action;

        private float _startTime;

        public SharedRunner(string taskName, float delayInSeconds, Action action, bool forever = false)
        {
            TaskName = taskName;
            DelayInSeconds = delayInSeconds;
            Forever = forever;
            _action = action;
            _startTime = Time.realtimeSinceStartup;
        }

        public void StartSchedule()
        {
            _startTime = Time.realtimeSinceStartup;
            IsStarted = true;
        }

        public bool Validate() => (Time.realtimeSinceStartup - _startTime) >= DelayInSeconds;

        public bool ExecuteTask()
        {
            var errorMessage = string.Empty;
            if (!Validate()) errorMessage = "!Validate()";

            if (!string.IsNullOrEmpty(errorMessage))
            {
                return false;
            }
            SharedLogger.Log($"{Tag}->ExecuteTask: {TaskName}");
            _action();
            return true;
        }
    }
}