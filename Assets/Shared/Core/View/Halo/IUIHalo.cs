using System.Collections;
using UnityEngine;

namespace Shared.Core.View.Halo
{
    public interface IUIHalo
    {
        void Show(GameObject target);
        IEnumerator Hide();
    }
}