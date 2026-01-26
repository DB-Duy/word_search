using System;
using System.Collections;
using System.Collections.Generic;
using Shared.Utils;
using UnityEngine;

namespace Shared.Core.Handler.Corou.Initializable
{
    public class InitializableHandlerChain : ICoroutineHandler, ISharedUtility
    {
        private readonly ICoroutineHandler[] _handlers;
        private readonly Action<int> _stepCompleteAction;
        private readonly Action<float, Dictionary<string, float>> _completeAction;

        public InitializableHandlerChain(Action<float, Dictionary<string, float>> completeAction, Action<int> stepCompleteAction, params InitializableHandler[] handlers)
        {
            _completeAction = completeAction;
            _stepCompleteAction = stepCompleteAction;
            _handlers = handlers;
        }

        public IEnumerator Handle()
        {
            Dictionary<string, float> timeDict = new();
            var temp = Time.realtimeSinceStartup;
            var index = 0;
            foreach (var handler in _handlers)
            {
                index++;
                var startTime = Time.realtimeSinceStartup;
                var name = ((IInitializableHandler)handler).Config.Name;
                this.LogInfo(SharedLogTag.Loading, "service", name);
                yield return handler.Handle();   
                timeDict.Upsert(name, Time.realtimeSinceStartup - startTime);
                _stepCompleteAction?.Invoke(index);
            }
            _completeAction.Invoke(Time.realtimeSinceStartup - temp, timeDict);
        }
    }
}