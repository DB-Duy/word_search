using Shared.Core.Async;

namespace Shared.Service.InAppUpdate
{
    public interface IInAppUpdateService
    {
        /// <summary>
        /// Check co version moi hay k?
        /// Neu co thi download.
        /// Khi download thanh cong thi tuy dieu kien ma auto update or cho user confirm
        /// </summary>
        IAsyncOperation<UpdateProcess> HandleNewVersionIfExisted();
        /// <summary>
        /// Chi call ham nay khi type update la flexible.
        /// </summary>
        bool ConfirmUpdateByUser();
    }
}