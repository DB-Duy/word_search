using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Shared.Referrer.Editor.Tests
{
    public class GooglePlayReferrerTests
    {
        [UnityTest]
        public IEnumerator TestPaidReferrer()
        {
            var data = "utm_source=admob&amp;utm_campaign=first_campaign&amp;anid=admob";
            var installReferrer = InstallReferrer.NewInstance(data);
            Assert.AreEqual(installReferrer.UtmSource, "admob");
            Assert.AreEqual(installReferrer.UtmCampaign, "first_campaign");
            Assert.AreEqual(installReferrer.AnId, "admob");
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator TestOrganicReferrer()
        {
            var data = "utm_source=google-play&amp;utm_medium=organic";
            var installReferrer = InstallReferrer.NewInstance(data);
            Assert.AreEqual(installReferrer.UtmSource, "google-play");
            Assert.AreEqual(installReferrer.UtmMedium, "organic");
            yield return null;
        }
    }
}