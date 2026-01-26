using System;
using System.Collections.Generic;
using Shared.Core.IoC;

namespace Shared.Service.NotificationCenter
{
    [Service]
    public class SharedNotificationCenter
    {
        private static readonly Dictionary<string, List<Action<string, Dictionary<string, object>>>> Listeners = new();

        public static void Register(string cmd, Action<string, Dictionary<string, object>> callback)
        {
            if (!Listeners.TryGetValue(cmd, out var list))
            {
                list = new List<Action<string, Dictionary<string, object>>>();
                Listeners[cmd] = list;
            }

            if (!list.Contains(callback))
            {
                list.Add(callback);
            }
        }

        public static void Unregister(string cmd, Action<string, Dictionary<string, object>> callback)
        {
            if (Listeners.TryGetValue(cmd, out var list))
            {
                list.Remove(callback);
                if (list.Count == 0)
                {
                    Listeners.Remove(cmd);
                }
            }
        }

        public static void Notify(string cmd, Dictionary<string, object> parameters = null)
        {
            if (Listeners.TryGetValue(cmd, out var list))
            {
                // Gọi theo thứ tự đăng ký
                for (var i = 0; i < list.Count; i++)
                {
                    list[i].Invoke(cmd, parameters);
                }
            }
        }
    }
}