using System.Collections;
using UnityEngine;

namespace Shared.Service.SharedCoroutine
{
    public static class SharedCoroutineServiceExtensions
    {
        public static MonoBehaviour CoroutineMonoBehaviour { get; set; }

        public static Coroutine StartSharedCoroutine(this ISharedUtility o, IEnumerator task)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return CoroutineMonoBehaviour == null ? null : CoroutineMonoBehaviour.StartCoroutine(task);
            Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(task);
            return null;
#else
            return CoroutineMonoBehaviour == null ? null : CoroutineMonoBehaviour.StartCoroutine(task);
#endif
        }
        
        public static void StopSharedCoroutine(this ISharedUtility o, Coroutine task)
        {
            CoroutineMonoBehaviour?.StopCoroutine(task);
        }
    }
}