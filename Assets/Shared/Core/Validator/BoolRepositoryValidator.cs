using Shared.Core.Repository.BoolType;

namespace Shared.Core.Validator
{
    public class BoolRepositoryValidator : IValidator
    {
        // private const string Tag = "BoolRepositoryValidator";
        private readonly IBoolRepository _repository;
        private readonly bool _expectedValue;

        public BoolRepositoryValidator(IBoolRepository repository, bool expectedValue)
        {
            _repository = repository;
            _expectedValue = expectedValue;
        }

        public bool Validate()
        {
            var currentValue = _repository.Get();
            return currentValue == _expectedValue;
        }
    }
    
    public class BoolRepositoryValidator<T> : IValidator<T>
    {
        // private const string Tag = "BoolRepositoryValidator";
        private readonly IBoolRepository _repository;
        private readonly bool _expectedValue;

        public BoolRepositoryValidator(IBoolRepository repository, bool expectedValue)
        {
            _repository = repository;
            _expectedValue = expectedValue;
        }

        public bool Validate(T t)
        {
            var currentValue = _repository.Get();
            return currentValue == _expectedValue;
        }
    }
}