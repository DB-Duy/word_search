#if IAP
using Shared.Core.IoC;
using Shared.Repository.Iap;
using Shared.Service.Iap;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Shared.View.Iap
{
    [RequireComponent(typeof(Button))]
    public class ManageSubscriptionButton : IoCMonoBehavior
    {
        [Inject] private IapItemRepository _itemRepository;
        [Inject] private IIapService _iapService;
        
        /// <summary>
        /// It can be id or productId
        /// </summary>
        [SerializeField] private string productId;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                var i = _itemRepository.GetById(productId);
                _iapService.RedirectingToSubscriptionManagementScreen(i == null ? productId : i.ProductId);    
            });
        }
    }
}

#endif