using UnityEngine;

namespace Shared.View.SharedFlyObject.Group
{
    public interface IFlyObjectGroup
    {
        Transform StartTransform { get; set; }
        Transform EndTransform { get; set; }
        float MinDurationInSeconds { get; set; }
        float MaxDurationInSeconds { get; set; }
    }
}