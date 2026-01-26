
namespace Shared.Core.Repository.IntType
{
    public class IntRamRepository : IntBaseRepository
    {
        private const string Tag = "IntRamRepository";
        
        private int _value;

        public IntRamRepository(string name = null, int defaultValue = 0) : base(name, defaultValue)
        {
            _value = defaultValue;
        }

        public override int Get() => _value;

        public override void Set(int newValue)
        {
            var oldValue = Get();
            if (newValue == oldValue) return;
            _value = newValue;
            onIntValueUpdated?.Invoke(oldValue, newValue);
            RepositoryEvents.OnIntValueChangedEvent.Invoke(this, oldValue, newValue);
        }

        public override void Delete()
        {
            _value = DefaultValue;
        }
    }
}