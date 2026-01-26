using Shared.Core.IoC;
using Shared.Core.Repository.JsonType;
using Shared.Entity.Firebase;

namespace Shared.Repository.Firebase
{
    [Repository]
    public class FirebaseRepository : JsonStoreRepository<FirebaseEntity>
    {
    }
}