#if GADSME
using Gadsme;
using Shared.Utils;
using UnityEngine;

namespace Shared.Ads.InPlay.Internal.SharedGadsme
{
    public class GadsmeDebugger : MonoBehaviour
    {
        private const string TAG = "GadsmeDebugger";
        [SerializeField] private GameObject[] placements;

        private void _PrintPlacementInfos()
        {
            var log = "";
            foreach (var p in placements)
            {
                log += $"{p.name} {p.GetComponent<GadsmePlacement>().placementId} \n";
            }
            
            SharedLogger.Log($"{TAG}->_PrintPlacementInfos: \n{log}");
        }

#if UNITY_EDITOR
        // public void OnInspectorGUI()
        // {
        //     GUILayout.Label("Editor Quick Actions");
        //     //base.GUIEditor();
        //
        //     if (GUILayout.Button("Print Placement Infos"))
        //     {
        //         _PrintPlacementInfos();
        //     }
        //
        // }
#endif
    }
}
#endif