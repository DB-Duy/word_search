#if INNERACTIVE_AD && UNITY_ANDROID
using Shared.Core.IoC;
using Shared.Entity.ParentControl;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.ParentControl.Partners
{
    /// <summary>
    /// https://developer.digitalturbine.com/hc/en-us/articles/360010822437-Integrating-the-Android-SDK
    /// https://developer.digitalturbine.com/hc/en-us/articles/360019954377-iOS-Ad-Formats
    /// </summary>
    [Component]
    public class FyberInneractiveAdSyncHandler : IParentControlSyncHandler, ISharedUtility
    {
        private static readonly AndroidJavaClass InneractiveAdParentControl = new("com.unity3d.player.FyberInneractiveAdPlugin");
        
        public void Handle(ParentControlEntity entity)
        {
            if(Application.isEditor) return;
            this.LogInfo(SharedLogTag.ParentControlNInneractiveAdOrFyber, nameof(entity), entity);
            InneractiveAdParentControl.CallStatic("setAgeAndGender", entity.Age, (int)entity.Gender);
        }
    }
}
#endif

#if INNERACTIVE_AD && UNITY_IOS
using System.Runtime.InteropServices;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.ParentControl;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.ParentControl.Partners
{
    /// <summary>
    /// Fyber
    /// https://developer.digitalturbine.com/hc/en-us/articles/360010822437-Integrating-the-Android-SDK
    /// https://developer.digitalturbine.com/hc/en-us/articles/360019954377-iOS-Ad-Formats
    /// </summary>
    [Component]
    public class FyberInneractiveAdSyncHandler : IParentControlSyncHandler, ISharedUtility
    {
        [DllImport("__Internal")]
        private static extern void _SetFyberAgeAndGender(int age, int gender);
        
        public void Handle(ParentControlEntity entity)
        {
            this.LogInfo(SharedLogTag.ParentControlNInneractiveAdOrFyber, nameof(entity), entity);
            if(Application.isEditor) return;
            _SetFyberAgeAndGender(entity.Age, (int)entity.Gender);
        }
    }
}
#endif