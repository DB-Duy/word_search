using UnityEngine;

namespace Shared.View.InPlayAds
{
    public interface IInPlayAdAdapter
    {
        bool ContainsScale(string placementName, string classname);
        Vector3 GetScale(string placementName, string classname);
        string GetPrefabPath(string placementName, string classname);
    }
}