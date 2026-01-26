using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Core.IoC;
using UnityEditor;
using UnityEngine;

namespace Shared.Service.Editor
{
    public class ServiceScanner
    {
        private const string Tag = "ServiceScanner";
        public static Type[] GetRepositoryTypes()
        {
            // Scan all assemblies for types with the [Repository] attribute
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetCustomAttributes(typeof(ServiceAttribute), false).Length > 0)
                .ToArray();
        }
        
        [MenuItem("Shared/Template/Validate Services")]
        public static void ScanAllServices()
        {
            var types = GetRepositoryTypes();
            foreach (var t in types)
            {
                if (typeof(MonoBehaviour).IsAssignableFrom(t))
                {
                    Debug.LogError($"{Tag}->typeof(MonoBehaviour).IsAssignableFrom({t.FullName})");
                }
            }
        }
    }
}