using Shared.Core.Async;
using Shared.Core.Handler.Corou.Initializable;
using Shared.Entity.Ump;

namespace Shared.Service.Ump
{
    public interface IUmpService : IInitializable
    {
        void InitUsPrivacyIfNotExisted();
        
        /// <summary>
        /// Sync Ump values to other parties.
        /// </summary>
        void Sync();

        bool CanEdit();
        IAsyncOperation Edit();

        UmpEntity Get();

        bool IsEnable();

        bool UpdateUsPrivacyValue(bool isTurnOn);

        bool IsTurnOn();
    }
}