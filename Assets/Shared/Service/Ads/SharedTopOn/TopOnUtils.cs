#if TOPON
using System.Collections.Generic;
using System.Diagnostics;
using AnyThinkAds.Api;
using Newtonsoft.Json;
using Shared.Utils;

namespace Shared.Ads.SharedTopOn
{
    public static class TopOnUtils
    {
        // {"placementId":"b1f7vpdm1ut2co","callbackInfo":{"network_firm_id":34,"adsource_id":"4348472","adsource_index":0,"adsource_price":0.0568,"adsource_isheaderbidding":1,"id":"d809baf49e1da3fdac0eb11a8ac071f2_4348472_1713955938550","publisher_revenue":5.68E-05,"currency":"USD","country":"VN","adunit_id":"b1f7vpdm1ut2co","adunit_format":"Banner","precision":"exact","network_type":"Network","network_placement_id":"R-M-5623164-3","ecpm_level":0,"segment_id":750508,"scenario_id":"","user_load_extra_data":{"banner_ad_size":"1080x168","adaptive_width":"1080","adaptive_type":0,"uses_pixel":true,"inline_adaptive_orientation":0,"key_width":1080,"adaptive_orientation":0,"banner_ad_size_struct":{},"inline_adaptive_width":1080,"key_height":168},"scenario_reward_name":"","scenario_reward_number":0,"abtest_id":494946,"sub_channel":"","channel":"","custom_rule":{"user_id":"868de3de2ad3cee07c385811ff32598e"},"ext_info":null,"reward_custom_data":""},"isTimeout":false,"isDeeplinkSucceed":false}
        [Conditional("LOG_INFO")]
        public static void DebugATAdEventArgs(string fromFunction, ATAdEventArgs erg)
        {
            var logDict = new Dictionary<string, object>()
            {
                {"placementId", erg.placementId},
                {"callbackInfo", erg.callbackInfo.toDictionary()},
                {"isTimeout", erg.isTimeout},
                {"isDeeplinkSucceed", erg.isDeeplinkSucceed}
            };
            if (erg is ATAdErrorEventArgs err)
            {
                logDict.Add("errorMessage", err.errorMessage);
                logDict.Add("errorCode", err.errorCode);
            }

            SharedLogger.Log($"{fromFunction}: {JsonConvert.SerializeObject(logDict)}");
        }
    }
}
#endif