#if FIREBASE
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Analytics;
using Shared.Tracking.Templates;
using UnityEngine;


namespace Shared.Service.Firebase
{
    public static class FirebaseExtensions
    {
        private const string Tag = "FirebaseExtensions";
        
        public static Parameter[] ToFirebaseParams(this IConvertableEvent e)
        {
            var dict = e.ToConvertableEvent();
            var parameters = new List<Parameter>();
            foreach (var kv in dict)
            {
                if (kv.Value == null) continue;
                switch (kv.Value)
                {
                    case int intValue:
                        parameters.Add(new Parameter(kv.Key, intValue));
                        break;
                    case long longValue:
                        parameters.Add(new Parameter(kv.Key, longValue));
                        break;
                    case string stringValue:
                        parameters.Add(new Parameter(kv.Key, stringValue));
                        break;
                    case float floatValue:
                        parameters.Add(new Parameter(kv.Key, floatValue));
                        break;
                    case double doubleValue:
                        parameters.Add(new Parameter(kv.Key, doubleValue));
                        break;
                    case bool boolValue:
                        parameters.Add(new Parameter(kv.Key, boolValue.ToString()));
                        break;
                    default:
#if LOG_INFO            
                        Debug.LogError($"{Tag}->ToFirebaseParams: ERROR: {((ITrackingEvent)e).EventName} {kv.Key} {kv.Value.GetType().FullName}");
                        throw new Exception($"{Tag}->ToFirebaseParams: ERROR: {((ITrackingEvent)e).EventName} {kv.Key} {kv.Value.GetType().FullName}");
#else
                        Debug.LogError($"{Tag}->ToFirebaseParams: ERROR: {((ITrackingEvent)e).EventName} {kv.Key} {kv.Value.GetType().FullName}");
                        break;
#endif                    
                }
            }
            return parameters.ToArray();
        }

        public static Dictionary<string, object> ToDict(this Task<string> task)
        {
            return  new Dictionary<string, object>()
            {
                {"idTask.IsFaulted", task.IsFaulted},
                {"idTask.Result", task.Result}
            };
        }
    }

}
#endif