using System.Collections;
using Shared.Core.View.Halo;
using UnityEngine;
using UnityEngine.UI;

namespace Shared.View.Halo
{
    public class UIHalo : MonoBehaviour, IUIHalo
    {
        [SerializeField] private Image dimBackground; // Assign the dim background Image here
        private Camera _camera;
        private Material _material;
        
        private void Awake()
        {
            _camera = Camera.main;
            _material = dimBackground.material;
            if (_material == null)
            {
                Debug.LogError("Dim Background must have a material with the TransparentHole shader!");
            }
        }
        
        public void Show(GameObject target)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator Hide()
        {
            throw new System.NotImplementedException();
        }
    }
}