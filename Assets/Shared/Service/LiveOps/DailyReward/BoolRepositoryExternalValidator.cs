using System;
using JetBrains.Annotations;
using Shared.Core.Repository.BoolType;

namespace Shared.LiveOps.DailyReward
{
    public class BoolRepositoryExternalValidator : IExternalValidator
    {
        private readonly IBoolRepository _repository;
        private readonly bool _expectedValue;

        public BoolRepositoryExternalValidator([NotNull] IBoolRepository repository, bool expectedValue)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _expectedValue = expectedValue;
        }

        public bool Validate() => _repository.Get() == _expectedValue;
    }
}