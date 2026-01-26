using System;
using DG.Tweening;
using Shared.Utilities.SharedBehaviour;
using UnityEngine;

namespace Shared.View.SharedFlyObject
{
    [DisallowMultipleComponent]
    public class ActionFlyObject : SharedMonoBehaviour, IActionFlyObject
    {
        public Transform StartTransform { get; set; }
        public Transform EndTransform { get; set; }
        public float DurationInSeconds { get; set; }
        public int CurveHeight { get; set; } = 2; // Adjust this for a more pronounced arc
        
        public void Fly(Action<ISharedFlyObject> onComplete = null)
        {
            // Define waypoints for the curved path
            var midPoint = Vector3.Lerp(StartTransform.position, EndTransform.position, 0.5f) + Vector3.up * CurveHeight;

            var path = new[]
            {
                StartTransform.position,
                midPoint,
                EndTransform.position
            };

            var mTransform = this.transform;
            // Make the object move along the path with an ease for smooth motion
            mTransform.position = StartTransform.position;
            mTransform.DOPath(path, DurationInSeconds, PathType.CatmullRom)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => onComplete?.Invoke(this));
        }
    }
}