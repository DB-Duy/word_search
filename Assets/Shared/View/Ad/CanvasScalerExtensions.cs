using UnityEngine;
using UnityEngine.UI;

namespace Shared.View.Ad
{
    public static class CanvasScalerExtensions
    {
        private static float ConvertPixelsToCanvasUnits(this CanvasScaler canvasScaler, float pixelAmount)
        {
            if (canvasScaler ==null) return pixelAmount;
            // Get the current resolution and reference resolution
            var referenceResolution = canvasScaler.referenceResolution;
            var currentResolution = new Vector2(Screen.width, Screen.height);
    
            // Calculate the scaling factor
            var scaleFactor = currentResolution.y / referenceResolution.y;

            // Convert pixels to canvas units
            return pixelAmount / scaleFactor;
        }
        
        public static float ConvertBannerTypeToCanvasUnits(this CanvasScaler canvasScaler, BannerType bannerType)
        {
            var height = bannerType.ToPixel();
            return canvasScaler.ConvertPixelsToCanvasUnits(height);
        }
    }
}