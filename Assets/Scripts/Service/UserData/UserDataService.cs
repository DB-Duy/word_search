
using Repository.UserData;
using Shared.Core.IoC;
using Zenject;

namespace Service.UserData
{
    [Service]
    public class UserDataService : IInitializable
    {
        private const string Tag = "UserDataService";
        
        [Inject] private UserDataRepository _userDataRepository;
        public bool IsInitialized;

        public void Initialize()
        {
            IsInitialized = true; 
        }
    }
}