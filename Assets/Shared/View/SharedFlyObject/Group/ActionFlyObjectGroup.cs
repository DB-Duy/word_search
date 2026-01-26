using System;
using UnityEngine;

namespace Shared.View.SharedFlyObject.Group
{
    public class ActionFlyObjectGroup : IActionFlyObjectGroup
    {
        public Transform StartTransform { get; set; }
        public Transform EndTransform { get; set; }
        public float MinDurationInSeconds { get; set; }
        public float MaxDurationInSeconds { get; set; }
        
        public void Fly(Action<IActionFlyObjectGroup> onComplete = null)
        {
            throw new NotImplementedException();
        }
    }
}