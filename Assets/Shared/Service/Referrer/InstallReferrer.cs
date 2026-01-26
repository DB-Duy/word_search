using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using Shared.Utils;

namespace Shared.Referrer
{
    /// <summary>
    /// "utm_source=admob&amp;amp;utm_campaign=first_campaign&amp;amp;anid=admob"
    ///
    /// "utm_source=google-play&amp;amp;utm_medium=organic"
    ///
    /// https://play.google.com/store/apps/details?id=com.truongps&amp;referrer=utm_source%3DcSource%26utm_medium%3DcMedium%26utm_term%3DcTerm%26utm_content%3DcContent%26utm_campaign%3DcName%26anid%3Daarki%26aclid%3D{click_id}%26cp1%3D{app_id}
    /// 
    /// </summary>
    [System.Serializable]
    public class InstallReferrer
    {
        [JsonProperty("utm_source")] public string UtmSource { get; private set; }
        [JsonProperty("utm_medium")] public string UtmMedium { get; private set; }
        [JsonProperty("utm_term")] public string UtmTerm { get; private set; }
        [JsonProperty("utm_content")] public string UtmContent { get; private set; }
        [JsonProperty("utm_campaign")] public string UtmCampaign { get; private set; }
        [JsonProperty("anid")] public string AnId { get; private set; }

        public override string ToString() => JsonConvert.SerializeObject(this);

        public static InstallReferrer NewInstance(string installReferrer)
        {
            var decodedString = WebUtility.HtmlDecode(installReferrer);
            
            var queryCollection = HttpUtility.ParseQueryString(decodedString);
            var queryDictionary = queryCollection.AllKeys.ToDictionary(key => key, key => queryCollection[key]);
            
            return new InstallReferrer()
            {
                UtmSource = queryDictionary.Get("utm_source", string.Empty), 
                UtmMedium = queryDictionary.Get("utm_medium", string.Empty),
                UtmTerm = queryDictionary.Get("utm_term", string.Empty),
                UtmContent = queryDictionary.Get("utm_content", string.Empty),
                UtmCampaign = queryDictionary.Get("utm_campaign", string.Empty),
                AnId = queryDictionary.Get("anid", string.Empty),
            };
        }
    }
}