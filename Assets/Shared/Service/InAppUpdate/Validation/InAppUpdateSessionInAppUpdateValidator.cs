#if IN_APP_UPDATE
using Shared.Core.IoC;
using Shared.Repository.InAppUpdate;
using Shared.Repository.Session;
using Shared.Utils;
using Zenject;

namespace Shared.Service.InAppUpdate.Validation
{
    [Component]
    public class InAppUpdateSessionInAppUpdateValidator : IInAppUpdateValidator, ISharedUtility
    {
        [Inject] private SessionRepository _sessionRepository;
        [Inject] private InAppUpdateRepository _repository;

        public bool Validate()
        {
            var e = _repository.Get();
            var se = _sessionRepository.Get();
            if (e.SessionId == se.SessionId)
            {
                this.LogInfo(SharedLogTag.InAppUpdate, nameof(e.SessionId), e.SessionId, nameof(se.SessionId), se.SessionId);
                return false;
            }

            e.SessionId = se.SessionId;
            _repository.Save(e);
            return true;
        }
    }
}
#endif