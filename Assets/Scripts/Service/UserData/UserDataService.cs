
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
        public int DisplayLevel => _userDataRepository.Get().Level + 1;
        public bool IsInitialized;

        public void Initialize()
        {
            IsInitialized = true; 
        }
        
        public void IncrementLevel()
        {
            var data = _userDataRepository.Get();
            data.Level++;
            _userDataRepository.Save(data);
        }
    }
}