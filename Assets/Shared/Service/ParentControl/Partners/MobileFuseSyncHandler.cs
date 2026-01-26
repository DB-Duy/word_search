#if UNITY_ANDROID && MOBILE_FUSE
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.ParentControl;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.ParentControl.Partners
{
    [Component]
    public class MobileFuseSyncHandler : IHandler<ParentControlEntity>, IParentControlSyncHandler, ISharedUtility
    {
        private static readonly AndroidJavaClass ParentControl = new("com.unity3d.player.MobileFusePlugin");

        public void Handle(ParentControlEntity data)
        {
            if (Application.isEditor) return;
            this.LogInfo(SharedLogTag.ParentControlNMobileFuse, nameof(data), data);
            ParentControl.CallStatic("setAgeAndGender", data.Age, (int)data.Gender);
        }
    }
}
#endif

#if UNITY_IOS && MOBILE_FUSE
using Shared.Core.IoC;
using Shared.Entity.ParentControl;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.ParentControl.Partners
{
    
    /// <summary>
    /// typedef NS_ENUM(NSUInteger, MFTargetingDataGender) {
    /// MOBILEFUSE_TARGETING_DATA_GENDER_UNKNOWN = 0,
    /// MOBILEFUSE_TARGETING_DATA_GENDER_FEMALE,
    /// MOBILEFUSE_TARGETING_DATA_GENDER_MALE,
    /// MOBILEFUSE_TARGETING_DATA_GENDER_OTHER
    /// };
    /// </summary>
    [Component]
    public class MobileFuseSyncHandler : IParentControlSyncHandler, ISharedUtility
    {
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void _SetMobileFuseAgeAndGender(int age, int gender);

        public void Handle(ParentControlEntity data)
        {
            if (Application.isEditor) return;
            this.LogInfo(SharedLogTag.ParentControlNMobileFuse, nameof(data), data);
            _SetMobileFuseAgeAndGender(data.Age, (int)data.Gender);
        }
    }
}
#endif