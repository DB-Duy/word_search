using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shared.Core.View.Dialog
{
    public class DialogTemplate
    {
        public CanvasGroup DimBackground { get; private set; }
        public Transform Content { get; private set; }

        public static DialogTemplate Create(MonoBehaviour o)
        {
            var transform = o.transform;
            var children = new List<Transform>();

            foreach (Transform child in transform)
            {
                children.Add(child);
            }
            
            if (children.Count < 2) return null;
            if (children[0].name != "DimBackground") return null;
            if (children[1].name != "Content") return null;
            var dimBackgroundObject = children[0].gameObject;
            var dimBackground = dimBackgroundObject.GetOrAddComponent<CanvasGroup>();
            if (dimBackground == null) throw new ArgumentException($"dimBackground == null for {dimBackgroundObject.name}");
            var content = children[1];
            return new DialogTemplate()
            {
                DimBackground = dimBackground,
                Content = content
            };
        }
    }
}