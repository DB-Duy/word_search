#if UNITY_ANDROID && BID_MACHINE
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.ParentControl;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.ParentControl.Partners
{
    /// <summary>
    /// https://docs.bidmachine.io/docs/in-house-mediation-android#targeting-parameters
    /// https://docs.bidmachine.io/docs/in-house-mediation-ios#targeting-info
    /// </summary>
    [Component]
    public class BidMachineSyncHandler : IHandler<ParentControlEntity>, IParentControlSyncHandler, ISharedUtility
    {
        private static readonly AndroidJavaClass BidMachineParentControl = new("com.unity3d.player.BidMachinePlugin");
        
        public void Handle(ParentControlEntity entity)
        {
            if(Application.isEditor) return;
            
            this.LogInfo(SharedLogTag.ParentControlNBidMachine, nameof(entity), entity);
            BidMachineParentControl.CallStatic("setYearOfBirthAndGender", entity.YearOfBirth, (int)entity.Gender);
        }
    }
}
#endif

#if UNITY_IOS && BID_MACHINE
using System.Runtime.InteropServices;
using Shared.Core.IoC;
using Shared.Entity.ParentControl;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.ParentControl.Partners
{
    /// <summary>
    /// https://docs.bidmachine.io/docs/in-house-mediation-android#targeting-parameters
    /// https://docs.bidmachine.io/docs/in-house-mediation-ios#targeting-info
    /// </summary>
    [Component]
    public class BidMachineSyncHandler : IParentControlSyncHandler, ISharedUtility
    {
        [DllImport("__Internal")]
        private static extern void _SetBidMachineYearOfBirthAndGender(int yearOfBirth, int gender);
        
        public void Handle(ParentControlEntity entity)
        {
            if(Application.isEditor) return;
            this.LogInfo(SharedLogTag.ParentControlNBidMachine, nameof(entity), entity);
            _SetBidMachineYearOfBirthAndGender(entity.YearOfBirth, (int)entity.Gender);
        }
    }
}
#endif