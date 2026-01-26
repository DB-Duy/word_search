using System.Collections;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Shared.Tracking.Models.IAP.Editor
{
    public class TestIapTracking
    {
        private void TryCalculate(string priceAmountMicrosString, float expectedValue)
        {
            var priceAmountMicros = long.Parse(priceAmountMicrosString);
            var amount = priceAmountMicros / 1000000d;
            Assert.AreEqual(expectedValue, amount);
        }

        [UnityTest]
        public IEnumerator Test49000000000()
        {
            TryCalculate("49000000000", 49000);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator Test129000000000()
        {
            TryCalculate("129000000000", 129000);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator Test299000000000()
        {
            TryCalculate("299000000000", 299000);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator Test499000000000()
        {
            TryCalculate("499000000000", 499000);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator Test1299000000000()
        {
            TryCalculate("1299000000000", 1299000);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator Test2499000000000()
        {
            TryCalculate("2499000000000", 2499000);
            yield return null;
        }
    }
}