namespace Shared.Tracking.Models.IAP
{
    public interface IGooglePlayPurchasingParams
    {
        string Receipt { get; }
        float DefaultUsdPriceValue { get; }
    }
}