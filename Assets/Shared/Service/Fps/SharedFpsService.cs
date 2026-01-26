using System.Collections.Generic;
using Shared.Core.IoC;
using Shared.Core.IoC.UnityLifeCycle;
using Shared.Service.Fps.Handlers;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.Fps
{
    [Service]
    public class SharedFpsService : IUnityUpdate, ISharedFpsService
    {
        private float _deltaTime = 0.0f;
        private float _fps;
        private int _fixedFps;
        private readonly HashSet<ISharedFpsHandler> _handlers = new();
        
        public ISharedFpsService Add(params ISharedFpsHandler[] handlers)
        {
            _handlers.AddRange(handlers);
            return this;
        }
        
        public void Update()
        {
            if (Time.timeScale <= 0f) return;
            _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
            _fps = 1.0f / _deltaTime;
            _fixedFps = (int)_fps;
            foreach (var handler in _handlers) handler.Handle(_fixedFps);
        }
    }
}