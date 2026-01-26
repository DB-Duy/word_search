using System.Collections.Generic;
using Shared.Utils;

namespace Shared.Core.IoC.UnityLifeCycle
{
    public class SequenceUnityUpdate : IUnityUpdate
    {
        private const string Tag = "SequenceUnityUpdate";
        private readonly HashSet<IUnityUpdate> _updates = new();
        private readonly List<IUnityUpdate> _addBuffer = new();
        private readonly List<IUnityUpdate> _removeBuffer = new();
        private bool _isLocked = false;

        public void Add(IUnityUpdate u)
        {
            if (u == null)
                return;

            if (_isLocked)
            {
                _addBuffer.Add(u);
            }
            else
            {
                _updates.Add(u); 
            }

            SharedLogger.LogJson(SharedLogTag.Ioc, $"{Tag}->Add", nameof(u), u.GetType().FullName);
        }

        public void Remove(IUnityUpdate u)
        {
            if (u == null)
                return;

            if (_isLocked)
            {
                _removeBuffer.Add(u);
            }
            else
            {
                _updates.Remove(u); 
            }

            SharedLogger.LogJson(SharedLogTag.Ioc, $"{Tag}->Remove", nameof(u), u.GetType().FullName);
        }

        public void Update()
        {
            _isLocked = true;
            foreach (var u in _updates)
            {
                if(u == null)
                    continue;

                u.Update();
            }
            _isLocked = false;

            if (_removeBuffer.Count > 0)
            {
                foreach (var u in _removeBuffer)
                {
                    _updates.Remove(u);
                }
                _removeBuffer.Clear();
            }

            if (_addBuffer.Count > 0)
            {
                foreach (var u in _addBuffer)
                {
                    _updates.Add(u);
                }
                _addBuffer.Clear();
            }
        }
    }
}