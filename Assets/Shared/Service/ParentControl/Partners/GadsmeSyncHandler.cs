#if GADSME
using Gadsme;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.ParentControl;
using Shared.Utils;
using Gender = Shared.Entity.ParentControl.Gender;

namespace Shared.Service.ParentControl.Partners
{
    [Component]
    public class GadsmeSyncHandler : IHandler<ParentControlEntity>, ISharedUtility, IParentControlSyncHandler
    {
        public void Handle(ParentControlEntity entity)
        {
            var gender = entity.Gender switch
            {
                Gender.Male => global::Gadsme.Gender.MALE,
                Gender.Female => global::Gadsme.Gender.FEMALE,
                _ => global::Gadsme.Gender.UNDEFINED
            };

            this.LogInfo(SharedLogTag.ParentControlNGadsme, nameof(gender), gender.ToString(), nameof(entity.Age), entity.Age);
            GadsmeSDK.SetUserAge(entity.Age);
            GadsmeSDK.SetUserGender(gender);
        }
    }
}
#endif