#if PARENT_CONTROL
using System;
using System.Collections.Generic;
using Shared.Core.Async;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Core.View.Scene;
using Shared.Entity.ParentControl;
using Shared.Repository.ParentControl;
using Shared.Service.ParentControl.Partners;
using Shared.Utilities;
using Shared.Utils;
using Shared.View.ParentControl;
using UnityEngine;
using Zenject;

namespace Shared.Service.ParentControl
{
    [Service]
    public class ParentControlService : IParentControlService, ISharedUtility, ISharedLogTag
    {
        [Inject] private ParentControlRepository _repository;
        [Inject] private ParentControlStepRepository _stepRepository;
        [Inject] private ParentControlConfigRepository _configRepository;
        
        private IHandler<ParentControlEntity> _syncHandler;
        private IHandler<ParentControlEntity> SyncHandler => _syncHandler ??= SequenceHandlerChain<ParentControlEntity>.CreateChainFromType<IParentControlSyncHandler>();

        private IAsyncOperation _initOperation;

        public bool IsInitialized { get; private set; }
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation();
            var step = GetStep();
            if (step == Step.Granted)
            {
                _initOperation.Success();
                IsInitialized = true;
            }
            else
            {
                if (IsUnlock())
                {
                    this.ShowDialog<ParentControlDialog>();
                }
                else
                {
                    _initOperation.Success();
                    IsInitialized = true;
                }
            }
            return _initOperation;
        }

        public ParentControlEntity Get() => _repository.Get();
        
        private List<int> _years;
        public List<int> GetYears()
        {
            if (_years != null) return _years;
            _years = new List<int>();
            var max = DateTime.Now.Year;
            for (var i = 1900; i < max; i++) {
                _years.Add(i);
            }
            return _years;
        }

        public void SaveYear(int year)
        {
            var e = _repository.Get();
            e.YearOfBirth = year;
            _repository.Save(e);
        }

        public int GetYear()
        {
            var e = _repository.Get();
            return e.YearOfBirth;
        }

        public int GetAge()
        {
            var e = _repository.Get();
            return e.Age;
        }

        public Step GetStep()
        {
            var e = _stepRepository.Get();
            return e.Step;
        }
        
        public void SetStep(Step step)
        {
            var e = _stepRepository.Get();
            e.Step = step;
            _stepRepository.Save(e);
            if (step == Step.Granted)
            {
                _initOperation.Success();
                IsInitialized = true;
            }
        }

        public bool IsUnlock()
        {
            var e = _configRepository.Get();
            return e.Unlocked;
        }

        public Step EvaluateNextStep()
        {
            var age = GetAge();
            return age < 13 ? Step.ParentPermission : Step.Granted;
        }

        public void SaveGender(Gender gender)
        {
            var e = _repository.Get();
            e.Gender = gender;
            _repository.Save(e);
        }

        public Gender GetGender()
        {
            var e = _repository.Get();
            return e == null ? Gender.None : e.Gender;
        }

        public void Sync()
        {
            if (Application.isEditor) return;
            if (!IsUnlock())
            {
                this.LogInfo(SharedLogTag.ParentControl, "ignore", "!IsUnlock()");
                return;
            }

            var d = _repository.Get(); 
            if (d == null) throw new ArgumentNullException(nameof(d));
            SyncHandler?.Handle(d);
        }

        public string LogTag => SharedLogTag.ParentControl;
    }
}
#endif