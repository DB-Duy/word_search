#if HUAWEI
using System;

namespace Shared.IAP.HuaweiIap
{
    public class HuaweiApplicationResumeAction
    {
        public float Time { get; }
        private Action Action { get; }
        public bool IsExecuted { get; private set; } = false;

        public HuaweiApplicationResumeAction(float time, Action action)
        {
            Time = time;
            Action = action;
        }

        public void Invoke()
        {
            if (IsExecuted) return;
            IsExecuted = true;
            Action.Invoke();
        }
    }
}
#endif