using Shared.Utils;

namespace Shared.Core.Repository.ObjectType
{
    public class ObjectRamRepository<T> : ObjectBaseRepository<T>
    {
        private const string Tag = "ObjectRamRepository";

        private T _value;

        public ObjectRamRepository(string name = null, T defaultValue = default(T)) : base(name, defaultValue)
        {
            _value = defaultValue;
        }

        public override T Get() => _value;

        public override void Save(T ob)
        {
            var oldValue = Get();
            if (ReferenceEquals(oldValue, ob)) return;
            _value = ob;
            onValueUpdated?.Invoke(oldValue, ob);
        }

        public override bool IsExisted()
        {
            return _value != null;
        }

        public override void Save(object ob)
        {
            if (ob is T)
            {
                Save((T)ob);
            }
            else
            {
                SharedLogger.LogError($"{Tag}->Save, ob is not correct type");
            }
        }

        public override void Delete()
        {
            _value = DefaultValue;
        }
    }
}