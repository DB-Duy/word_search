using UnityEngine;

namespace Shared.Controller.SharedInPlayAd
{
    public interface ISharedInPlayAdController
    {
        T InitInPlayAdView<T>() where T : MonoBehaviour;
        T Request<T>() where T : MonoBehaviour;
        GameObject RequestByClassName(params string[] classFullNames);
        void Release<T>(T o) where T : MonoBehaviour;
    }
}