using Shared.Core.IoC;
using Shared.Core.Repository.IntType;

namespace Shared.Repository.Session
{
    // new IntPlayerPrefsRepository(Constants.PlayerPrefsRepository.SessionCount, defaultValue: 0);
    [Repository]
    public class SessionCountRepository : IntStoreRepository
    {
    }
}