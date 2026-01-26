using System;
using UnityEngine;
#if ADVERTY_5
using Adverty5;
#endif

namespace Shared.Service.Adverty.Internal
{
    [DisallowMultipleComponent]
    public class AdvertyMainCameraSetter : MonoBehaviour
    {
        [SerializeField] private Camera camera;

#if ADVERTY_4
        private void OnEnable()
        {
            global::Adverty.AdvertySettings.SetMainCamera(camera);
        }
#endif
#if ADVERTY_5
        private void OnEnable()
        {
            if (camera == null)
            {
                Debug.LogError(
                    "Adverty was not able to detect a MainCamera. AdvertyLooper wasn't attached. Please add it manually to your Game Camera.");
                return;
            }

            GameObject currentCameraObject = camera.gameObject;
            if (!currentCameraObject.GetComponent<AdvertyLooper>())
            {
                currentCameraObject.AddComponent<AdvertyLooper>();
            }
        }
#endif
    }
}