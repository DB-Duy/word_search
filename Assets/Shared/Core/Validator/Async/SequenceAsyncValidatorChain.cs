using System;
using System.Collections;
using Shared.Core.Async;
using Shared.Service.SharedCoroutine;

namespace Shared.Core.Validator.Async
{
    public class SequenceAsyncValidatorChain : IAsyncValidator, ISharedUtility
    {
        private readonly IAsyncValidator[] _validators;

        public SequenceAsyncValidatorChain(params IAsyncValidator[] validators)
        {
            _validators = validators;
        }

        public IAsyncOperation Validate()
        {
            var operation = new SharedAsyncOperation();
            this.StartSharedCoroutine(_Validate((errorMessage) =>
            {
                if (string.IsNullOrEmpty(errorMessage))
                    operation.Success();
                else
                    operation.Fail(errorMessage);
            }));
            return operation;
        }

        private IEnumerator _Validate(Action<string> onComplete)
        {
            foreach (var validator in _validators)
            {
                var o = validator.Validate();
                if (!o.IsComplete) yield return null;
                if (o.IsSuccess) continue;
                onComplete.Invoke($"Validator {validator.GetType().FullName} was failed.");
                yield break;
            }
            onComplete.Invoke(null);
        }
    }
    
    public class SequenceAsyncValidatorChain<T> : IAsyncValidator<T>, ISharedUtility
    {
        private readonly IAsyncValidator<T>[] _validators;

        public SequenceAsyncValidatorChain(params IAsyncValidator<T>[] validators)
        {
            _validators = validators;
        }

        public IAsyncOperation Validate(T t)
        {
            var operation = new SharedAsyncOperation();
            this.StartSharedCoroutine(_Validate(t, (errorMessage) =>
            {
                if (string.IsNullOrEmpty(errorMessage))
                    operation.Success();
                else
                    operation.Fail(errorMessage);
            }));
            return operation;
        }

        private IEnumerator _Validate(T t, Action<string> onComplete)
        {
            foreach (var validator in _validators)
            {
                var o = validator.Validate(t);
                if (!o.IsComplete) yield return null;
                if (o.IsSuccess) continue;
                onComplete.Invoke($"Validator {validator.GetType().FullName} was failed.");
                yield break;
            }
            onComplete.Invoke(null);
        }
    }
}