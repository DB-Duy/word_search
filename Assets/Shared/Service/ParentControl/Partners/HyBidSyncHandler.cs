#if HY_BID && UNITY_ANDROID
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.ParentControl;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.ParentControl.Partners
{
    /// <summary>
    /// https://developers.verve.com/reference/hybid-android-configuration#targeting-parameters
    /// </summary>
    [Component]
    public class HyBidSyncHandler : IHandler<ParentControlEntity>, IParentControlSyncHandler, ISharedUtility
    {
        private static readonly AndroidJavaClass JavaClass = new("com.unity3d.player.HyBidPlugin");
        
        public void Handle(ParentControlEntity entity)
        {
            if(Application.isEditor) return;
            this.LogInfo(SharedLogTag.ParentControlNHyBid, nameof(entity), entity);
            JavaClass.CallStatic("setAgeAndGender", entity.Age, (int)entity.Gender);
        }
    }
}
#endif

#if HY_BID && UNITY_IOS
using System.Runtime.InteropServices;
using Shared.Core.IoC;
using Shared.Entity.ParentControl;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.ParentControl.Partners
{
    /// <summary>
    /// https://developers.verve.com/reference/hybid-android-configuration#targeting-parameters
    /// </summary>
    [Component]
    public class HyBidSyncHandler : IParentControlSyncHandler, ISharedUtility
    {
        [DllImport("__Internal")]
        private static extern void _SetHyBidAgeAndGender(int age, int gender);
        
        public void Handle(ParentControlEntity entity)
        {
            if(Application.isEditor) return;
            this.LogInfo(SharedLogTag.ParentControlNHyBid, nameof(entity), entity);
            _SetHyBidAgeAndGender(entity.Age, (int)entity.Gender);
        }
    }
}
#endif