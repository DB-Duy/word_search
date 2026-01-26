#if SMAATO && UNITY_ANDROID
using Shared.Core.IoC;
using Shared.Entity.ParentControl;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.ParentControl.Partners
{
    /// <summary>
    /// https://developers.smaato.com/publishers/android-nextgen-sdk-integration/
    /// https://developers.smaato.com/publishers/nextgen-sdk-ios-integration/
    /// </summary>
    [Component]
    public class SmaatoSyncHandler : IParentControlSyncHandler, ISharedUtility
    {
        private static readonly AndroidJavaClass PluginClass = new("com.unity3d.player.SmaatoPlugin");
        
        public void Handle(ParentControlEntity entity)
        {
            if(Application.isEditor) return;
            this.LogInfo(SharedLogTag.ParentControlNSmaato, nameof(entity), entity);
            PluginClass.CallStatic("setAgeAndGender", entity.Age, (int)entity.Gender);
        }
    }
}
#endif

#if SMAATO && UNITY_IOS
using System.Runtime.InteropServices;
using Shared.Core.IoC;
using Shared.Entity.ParentControl;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.ParentControl.Partners
{
    /// <summary>
    /// https://developers.smaato.com/publishers/android-nextgen-sdk-integration/
    /// https://developers.smaato.com/publishers/nextgen-sdk-ios-integration/
    /// </summary>
    [Component]
    public class SmaatoSyncHandler : IParentControlSyncHandler, ISharedUtility
    {
        [DllImport("__Internal")]
        private static extern void _SetSmaatoAgeAndGender(int age, int gender);
        
        public void Handle(ParentControlEntity entity)
        {
            if(Application.isEditor) return;
            this.LogInfo(SharedLogTag.ParentControlNSmaato, nameof(entity), entity);
            _SetSmaatoAgeAndGender(entity.Age, (int)entity.Gender);
        }
    }
}
#endif