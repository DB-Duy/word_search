using System.Collections;
using UnityEngine.TestTools;

namespace Shared.IAP.Editor.Tests
{
    public class IapTestCases
    {
        [UnityTest]
        public IEnumerator TestJsonParse()
        {
            // var jsonString = "{\"premium\":{\"id\":\"premium\",\"appstore\":\"com.indiez.nonogram.premiumpack\",\"googleplay\":\"com.indiez.nonogram.premiumpack\",\"type\":\"subscription\",\"default_usd_price\":\"1.99\",\"function_tags\":\"remove_ads\",\"en\":\"Premium Package\"}}";
            // var container = IapProductDefineContainer.NewFromJson(IapProductDefineContainer.KGooglePlay, jsonString);
            // Assert.AreEqual(container.SubscriptionProducts[0].DefaultUsdPriceValue, 1.99f);
            yield return null;
        }
    }
}