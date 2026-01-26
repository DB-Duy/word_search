using System.Linq;
using Shared.Core.Handler;
using Shared.Core.IoC;

namespace Shared.Core.Validator
{
    public class ValidatorChain : IValidator
    {
        private IValidator[] _validators;


        public ValidatorChain(params IValidator[] validators)
        {
            _validators = validators;
        }

        public bool Validate() => _validators.All(v => v.Validate());
        
        public static ValidatorChain CreateChainFromType<C>()
        {
            var items = IoCExtensions.ResolveAll<C>();
            return new ValidatorChain
            {
                _validators = items.OfType<IValidator>().ToArray()
            };
        }
    }
    
    public class ValidatorChain<T> : IValidator<T>
    {
        private IValidator<T>[] _validators;


        public ValidatorChain(params IValidator<T>[] validators)
        {
            _validators = validators;
        }

        public bool Validate(T t) => _validators.All(v => v.Validate(t));
        
        public static ValidatorChain<T> CreateChainFromType<C>()
        {
            var items = IoCExtensions.ResolveAll<C>();
            return new ValidatorChain<T>
            {
                _validators = items.OfType<IValidator<T>>().ToArray()
            };
        }
    }
    
    public class ValidatorChain<T, U> : IValidator<T, U>
    {
        private readonly IValidator<T, U>[] _validators;


        public ValidatorChain(IValidator<T, U>[] validators)
        {
            _validators = validators;
        }

        public bool Validate(T t, U u) => _validators.All(v => v.Validate(t, u));
    }
}