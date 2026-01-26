#if USING_UMP
using GoogleMobileAds.Ump.Api;
using Shared.Core.Async;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.Ump;
using Shared.Repository.Ump;
using Shared.Service.Ump.Internal;
using Shared.Service.Ump.Internal.Steps;
using Shared.Service.Ump.Validation;
using Shared.Utilities;
using Shared.Utils;
using Zenject;

namespace Shared.Service.Ump
{
    [Service]
    public class SharedUmpService : IUmpService, ISharedLogTag, ISharedUtility
    {
        [Inject] private SharedUmpConfigValidator _configValidator;
        [Inject] private GdprAppliesRepository _gdprAppliesRepository;
        [Inject] private PurposeConsentsRepository _purposeConsentsRepository;
        [Inject] private TcStringRepository _tcStringRepository;
        [Inject] private UsPrivacyStringRepository _usPrivacyStringRepository;
        
        private IHandler<UmpEntity> _syncHandler;
        public IHandler<UmpEntity> SyncHandler => _syncHandler ??= SequenceHandlerChain<UmpEntity>.CreateChainFromType<IUmpValueSyncHandler>();

        public bool IsInitialized => _initOperation is { IsComplete: true, IsSuccess: true };
        private IAsyncOperation _initOperation;
        private IAsyncOperation _editOperation;

        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            this.LogInfo();
            InitUsPrivacyIfNotExisted();
            var i = new UmpProcessInfo(this, timeOutInSeconds: 5);
            var e = new MultiStepExecutor(
                new CheckStatusExecutor(), //
                new LoadFormExecutor(), //
                new ShowFormExecutor(), //
                new SyncUmpExecutor()); //
            i.OnComplete = () => { UmpFlag.IsUmpReady = true; };
            _initOperation = e.Execute(i);
            return _initOperation;
        }

        public void InitUsPrivacyIfNotExisted()
        {
            var v = _usPrivacyStringRepository.Get();
            if (!string.IsNullOrEmpty(v)) return;
            _usPrivacyStringRepository.Set(UsPrivacyValue.Const1YnnOff);
            this.LogInfo("us_privacy_string", UsPrivacyValue.Const1YnnOff);
        }

        public void Sync()
        {
            this.LogInfo();
            var e = Get();
            SyncHandler?.Handle(e);
        }

        public bool CanEdit() => ConsentInformation.ConsentStatus is ConsentStatus.Obtained or ConsentStatus.Required;

        public IAsyncOperation Edit()
        {
            this.LogInfo();
            if (_editOperation != null) return _editOperation;
            var i = new UmpProcessInfo(this, timeOutInSeconds: 5);
            var e = new MultiStepExecutor(
                new LoadFormExecutor(isEdit: true), //
                new ShowFormExecutor(), //
                new SyncUmpExecutor()); //
            i.OnComplete = () => { _editOperation = null; };
            _editOperation = e.Execute(i);
            return _editOperation;
        }

        public UmpEntity Get()
        {
            return new UmpEntity
            {
                GdprApplies = _gdprAppliesRepository.Get(),
                PurposeConsents = _purposeConsentsRepository.Get(),
                TcString = _tcStringRepository.Get(),
                UsPrivacyString = _usPrivacyStringRepository.Get()
            };
        }

        public bool IsEnable() => _configValidator == null || _configValidator.Validate();
        
        public bool UpdateUsPrivacyValue(bool isTurnOn)
        {
            var newValue = isTurnOn ? UsPrivacyValue.Const1YynOn : UsPrivacyValue.Const1YnnOff;
            this.LogInfo(nameof(isTurnOn), isTurnOn, nameof(newValue), newValue);
            var isUpdated = _usPrivacyStringRepository.Set(newValue);
            if (isUpdated) Sync();
            return isUpdated;
        }

        public bool IsTurnOn()
        {
            var v = _usPrivacyStringRepository.Get();
            return v == UsPrivacyValue.Const1YynOn;
        }

        public string LogTag => SharedLogTag.Ump;
    }
}
#endif