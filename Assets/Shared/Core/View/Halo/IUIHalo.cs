using System;
using System.Collections;
using UnityEngine;

namespace Shared.Core.View.Halo
{
    public interface IUIHalo
    {
        void Show(GameObject target);
        void Show(GameObject target, Action onClickHaloCallback);
        IEnumerator Hide();
    }
}