#if ADVERTY_4 && PARENT_CONTROL
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.ParentControl;
using Shared.Utils;

namespace Shared.Service.ParentControl.Partners
{
    [Component]
    public class AdvertySyncHandler : IHandler<ParentControlEntity>, IParentControlSyncHandler, ISharedUtility
    {
        public void Handle(ParentControlEntity entity)
        {
            var gender = entity.Gender switch
            {
                Gender.Female => global::Adverty.Gender.Female,
                Gender.Male => global::Adverty.Gender.Male,
                Gender.Unknown => global::Adverty.Gender.Unknown,
                Gender.Other => global::Adverty.Gender.Other,
                _ => global::Adverty.Gender.Undefined
            };
            
            this.LogInfo(SharedLogTag.ParentControlNAdverty, nameof(gender), gender, nameof(entity.YearOfBirth), entity.YearOfBirth);
            var ageData = new global::Adverty.AgeData(yearOfBirth: entity.YearOfBirth);
            var userData = new global::Adverty.UserData(global::Adverty.AgeSegment.Unknown, gender, ageData);
            global::Adverty.AdvertySDK.UpdateUserData(userData);
        }
    }
}

#endif