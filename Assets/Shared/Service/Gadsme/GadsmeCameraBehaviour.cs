#if GADSME
using Gadsme;
using UnityEngine;

namespace Shared.Ads.InPlay.Internal.SharedGadsme
{
    [DisallowMultipleComponent]     
    public class GadsmeCameraBehaviour : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        private void OnEnable()
        {
            GadsmeSDK.SetMainCamera(_camera);
        }
    }
}
#endif