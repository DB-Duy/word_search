using UnityEngine;

namespace Shared.View.SharedFlyObject
{
    public interface ISharedFlyObject
    {
        Transform StartTransform { get; set; }
        Transform EndTransform { get; set; }
        float DurationInSeconds { get; set; }
    }
}