using Shared.Core.Repository.BoolType;
using Shared.Utils;
using UnityEngine;

namespace Shared.Core.View.Binding.Binder
{
    public class BoolGameObjectActiveBinder : IBinder
    {
        public GameObject GameObj { get; private set; }
        public IBoolRepository Repository { get; private set; }
        public bool IsReversed { get; set; }

        public BoolGameObjectActiveBinder(GameObject go, IBoolRepository repository, bool isReversed = false)
        {
            GameObj = go;
            Repository = repository;
            IsReversed = isReversed;
        }

        public IBinder Bind()
        {
            this.LogInfo(SharedLogTag.UIBind, nameof(Repository), Repository.Name, nameof(GameObj), GameObj.name, nameof(IsReversed), IsReversed);
            Repository.onValueChanged.AddListener(_OnValueChanged);
            _OnValueChanged(Repository.Get());
            return this;
        }

        public void Unbind()
        {
            this.LogInfo(SharedLogTag.UIBind, nameof(Repository), Repository.Name, nameof(GameObj), GameObj.name, nameof(IsReversed), IsReversed);
            Repository.onValueChanged.RemoveListener(_OnValueChanged);
        }

        private void _OnValueChanged(bool newValue)
        {
            GameObj.SetActive(IsReversed ? !newValue : newValue);
        }
    }
}