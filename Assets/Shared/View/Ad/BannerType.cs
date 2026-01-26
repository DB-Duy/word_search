using System;
using System.Collections.Generic;
using Shared.Utils;
using UnityEngine;

namespace Shared.View.Ad
{
    public enum BannerType
    {
        None, B50, B90, Adaptive
    }

    public static class BannerTypeExtensions
    {
        /// <summary>
        /// The getMaximalAdaptiveHeight method provides the maximum height for a given width in adaptive banner supported networks. If there are no networks that support adaptive banners, the returned value will be -1
        /// Tại sao chỉ xảy ra ở RU, vì Adaptive Banner chỉ hoạt động với Google AdMob mà Google AdMob không được bật ở RU do chiến tranh, nên giá trị của biến này luôn trả về bằng -1. Dựa vào biến này để setup SafeArea sẽ bị sai, cần phải bổ sung thêm case:
        /// Nếu trả về value = -1 thì sẽ set height mặc định 50 cho phone và 90 cho tablet nhen ae
        /// </summary>
        private static readonly Dictionary<BannerType, Func<float>> _adHeightResolver = new()
        {
            { BannerType.None, () => 0 },
            { BannerType.B50, () => 50 },
            { BannerType.B90, () => 90 },
            {
                BannerType.Adaptive, () =>
                {
                    if (Application.isEditor) return 90;
#if LEVEL_PLAY
                    var width = IronSource.Agent.getDeviceScreenWidth();
                    var height = IronSource.Agent.getMaximalAdaptiveHeight(width);
                    SharedLogger.LogInfoCustom(SharedLogTag.AdNBanner, "BannerTypeExtensions", "_adHeightResolver", nameof(width), width, nameof(height), height);
                    if (height <= 0)
                    {
                        SharedLogger.LogInfoCustom(SharedLogTag.AdNBanner, "BannerTypeExtensions", "_adHeightResolver", nameof(width), width, nameof(height), height, "defaultHeight", SharedUtilities.IsTabletDevice ? 90 : 50);
                        return SharedUtilities.IsTabletDevice ? 90 : 50;
                    }

                    return height;
#else
                    return SharedUtilities.IsTabletDevice ? 90 : 50;
#endif
                }
            }
        };
        
        public static float ToPixel(this BannerType bannerType) => _adHeightResolver[bannerType].Invoke();
    }
}