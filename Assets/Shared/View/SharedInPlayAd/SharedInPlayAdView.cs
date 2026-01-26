using Shared.Utilities.SharedBehaviour;
using UnityEngine;

namespace Shared.View.SharedInPlayAd
{
    [DisallowMultipleComponent]
    public class SharedInPlayAdView : SharedMonoBehaviour, ISharedInPlayAdView
    {
        public bool IsReady { get; protected set; }
    }
}