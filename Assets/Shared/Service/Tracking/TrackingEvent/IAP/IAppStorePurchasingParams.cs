namespace Shared.Tracking.Models.IAP
{
    public interface IAppStorePurchasingParams
    {
        string Receipt { get; }
        
        float DefaultUsdPriceValue { get; }
    }
}