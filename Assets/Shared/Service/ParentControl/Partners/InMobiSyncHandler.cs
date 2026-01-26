#if UNITY_ANDROID && INMOBI
using Shared.Core.IoC;
using Shared.Entity.ParentControl;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.ParentControl.Partners
{
    /// <summary>
    /// https://support.inmobi.com/monetize/sdk-documentation/ios-guidelines/overview-ios-guidelines#b-%C2%A0pass-age-and-gender-information
    /// </summary>
    [Component]
    public class InMobiSyncHandler : IParentControlSyncHandler, ISharedUtility
    {
        private static readonly AndroidJavaClass PluginClass = new("com.unity3d.player.InMobiPlugin");
        
        public void Handle(ParentControlEntity entity)
        {
            if(Application.isEditor) return;
            
            this.LogInfo(SharedLogTag.ParentControlNInMobi, nameof(entity), entity);
            PluginClass.CallStatic("setAgeAndGender", entity.Age, (int)entity.Gender);
        }
    }
}
#endif

#if INMOBI && UNITY_IOS
using System.Runtime.InteropServices;
using Shared.Core.IoC;
using Shared.Entity.ParentControl;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.ParentControl.Partners
{
    /// <summary>
    /// https://support.inmobi.com/monetize/sdk-documentation/ios-guidelines/overview-ios-guidelines#b-%C2%A0pass-age-and-gender-information
    /// </summary>
    [Component]
    public class InMobiSyncHandler : IParentControlSyncHandler, ISharedUtility
    {
        [DllImport("__Internal")]
        private static extern void _SetInMobiAgeAndGender(int age, int gender);
        
        public void Handle(ParentControlEntity entity)
        {
            if(Application.isEditor) return;
            this.LogInfo(SharedLogTag.ParentControlNInMobi, nameof(entity), entity);
            _SetInMobiAgeAndGender(entity.Age, (int)entity.Gender);
        }
    }
}
#endif