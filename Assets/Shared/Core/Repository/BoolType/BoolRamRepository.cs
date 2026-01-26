namespace Shared.Core.Repository.BoolType
{
    /// <summary>
    /// Store in RAM or Memory
    /// </summary>
    public class BoolRamRepository : BoolBaseRepository
    {
        private bool _value;

        public BoolRamRepository(string name = null, bool defaultValue = false) : base(name, defaultValue)
        {
            _value = DefaultValue;
        }
     
        public override bool Get() => _value;

        public override void Set(bool newValue)
        {
            if (_value == newValue) return;
            _value = newValue;
            onValueChanged.Invoke(newValue);
            foreach (var handler in Handlers) handler.OnValueChanged(newValue);
            RepositoryEvents.OnBoolValueChangedEvent.Invoke(this, newValue);
        }
    }
}
