using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared.Utils
{
    [System.Serializable]
    public class FrameCounter
    {
        [SerializeField]
        private int _counter;
        [SerializeField]
        private int _space;
        bool _allow = false;

        public FrameCounter(int space) => _space = space;

        public void IncreaseByOne() => _counter++;

        public bool Allow(bool resetIfAllow = true)
        {
            _allow = _counter >= _space;
            if (_allow && resetIfAllow) this.Reset();
            return _allow;
        }

        public void Reset() => _counter = 0;

        public static FrameCounter NewInstance(int space)
        {
            return new FrameCounter(space: space);
        }
    }
}