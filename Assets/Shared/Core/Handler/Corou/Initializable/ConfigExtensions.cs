using System.Collections;
using Shared.Core.Async;
using Shared.Utils;
using UnityEngine;

namespace Shared.Core.Handler.Corou.Initializable
{
    public static class ConfigExtensions
    {
        public static IEnumerator Handle(this Config c, IInitializable initializable)
        {
            if (initializable == null)
            {
#if LOG_INFO
                SharedLogger.LogError("ConfigExtensions.Handle: IInitializable is null");
#endif
                yield break;
            }

            var operation = initializable.Initialize();
            if (c.IsFreeTask)
                yield break;

            if (c.TimeOut <= 0)
            {
                while (!operation.IsComplete)
                    yield return null;
#if LOG_INFO
                if (!operation.IsSuccess)
                {
                    SharedLogger.Log($"{initializable.GetType()}->Initialize->Failed: {operation.FailReason}");
                }
#endif
                yield break;
            }

            var startTime = Time.realtimeSinceStartup;
            while (!operation.IsComplete && (Time.realtimeSinceStartup - startTime) < c.TimeOut)
                yield return null;

#if LOG_INFO
            if (!operation.IsSuccess)
            {
                SharedLogger.Log($"{initializable.GetType()}->Initialize->Failed: {operation.FailReason}");
            }
#endif
        }
        
        public static IEnumerator Handle(this Config c, params IInitializable[] initializables)
        {
            var multiOperations = new MultiAsyncOperation();
            foreach (var initializable in initializables)
            {
                if (initializable == null)
                    continue;

                var operation = initializable.Initialize();
                multiOperations.Add(operation);
            }
            
            if (c.IsFreeTask)
                yield break;

            if (c.TimeOut <= 0)
            {
                while (!multiOperations.IsAllOperationComplete())
                    yield return null;

                yield break;
            }

            var startTime = Time.realtimeSinceStartup;
            while (!multiOperations.IsAllOperationComplete() && (Time.realtimeSinceStartup - startTime) < c.TimeOut)
                yield return null;
        }
    }
}