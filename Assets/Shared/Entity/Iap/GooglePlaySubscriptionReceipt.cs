using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shared.Entity.Iap
{
    /// <summary>
    /// "{\"Payload\":\"{\\\"json\\\":\\\"{\\\\\\\"orderId\\\\\\\":\\\\\\\"GPA.3346-7240-5557-76013\\\\\\\",\\\\\\\"packageName\\\\\\\":\\\\\\\"com.indiez.penguin.dash\\\\\\\",\\\\\\\"productId\\\\\\\":\\\\\\\"com.indiez.penguin.dash.p1\\\\\\\",\\\\\\\"purchaseTime\\\\\\\":1720429489203,\\\\\\\"purchaseState\\\\\\\":0,\\\\\\\"purchaseToken\\\\\\\":\\\\\\\"bjpiapfbomaijncaibpfmjle.AO-J1OwKSko8ZfNABWpk-aUHirctM-gPonqOIVm_LwJe8X2XyiUQFAQ3CnF1wQXNSjNTTRez861KuD1nbW1d6DVBZoXrUsdIb-dBeAodhTVYbNsytuxq8gs\\\\\\\",\\\\\\\"quantity\\\\\\\":1,\\\\\\\"acknowledged\\\\\\\":false}\\\",\\\"signature\\\":\\\"f3GNrQYPuBRpahkFQK98y/jcukoVRowyDQSu6Mzc0+j8D4uJP4AS9IuW8TA3I0zAntwoLNCqhCH1A6YE6YeoJXwFnLrbancK1q9dug7cBzQo+b0HHbj3+QZQgHTtWknY8Svwzrl6zMiHaG1c9G9zFaAbh/Y6I/lTNorrzZR6Ej1pwssUsk5RrXk6wiRn+GLtYtfL5/SZUHfvtEumdpln4xCRBm8/fPvxEOLcXZ0xrBKeZcuEVZTxUDDLZaivIXAsKBkU4s4EzEAwaBrfupA04QzDfa1BCq4z2RS6jYQQDXySQhXk65nZdFPBWZJ4eFFsZf0jlhXWCivEUs3yKkzvMw==\\\",\\\"skuDetails\\\":[\\\"{\\\\\\\"productId\\\\\\\":\\\\\\\"com.indiez.penguin.dash.p1\\\\\\\",\\\\\\\"type\\\\\\\":\\\\\\\"inapp\\\\\\\",\\\\\\\"title\\\\\\\":\\\\\\\"Coin Pack 1 (Penguin Dash: Run Race 3D)\\\\\\\",\\\\\\\"name\\\\\\\":\\\\\\\"Coin Pack 1\\\\\\\",\\\\\\\"iconUrl\\\\\\\":\\\\\\\"https:\\\\\\\\/\\\\\\\\/lh3.googleusercontent.com\\\\\\\\/87elkd53B9c59jHvpcghibzdw6Up4cFrG96GqUnYm3O5TNBeWHhBS1nzCG4wq_OzXyjA\\\\\\\",\\\\\\\"description\\\\\\\":\\\\\\\"Coin Pack 1\\\\\\\",\\\\\\\"price\\\\\\\":\\\\\\\"\\\\u20ac1.99\\\\\\\",\\\\\\\"price_amount_micros\\\\\\\":1990000,\\\\\\\"price_currency_code\\\\\\\":\\\\\\\"EUR\\\\\\\",\\\\\\\"skuDetailsToken\\\\\\\":\\\\\\\"AEuhp4LYzrJ6oJ7jDTVCnbJ5540CMYyLmvjqVwvDclG8BJFRM1EwC-FekpFdTesbM_5Uk6voi-tlPzfPZC8gwBXxWWGuW69d2mGJB9kFlTk15_AFn1b6\\\\\\\"}\\\"]}\",\"Store\":\"GooglePlay\",\"TransactionID\":\"bjpiapfbomaijncaibpfmjle.AO-J1OwKSko8ZfNABWpk-aUHirctM-gPonqOIVm_LwJe8X2XyiUQFAQ3CnF1wQXNSjNTTRez861KuD1nbW1d6DVBZoXrUsdIb-dBeAodhTVYbNsytuxq8gs\"}"
    /// 
    /// {"Payload":"{\"json\":\"{\\\"orderId\\\":\\\"GPA.3329-0999-2703-63867\\\",\\\"packageName\\\":\\\"com.indiez.nonogram\\\",\\\"productId\\\":\\\"com.indiez.nonogram.premiumpack\\\",\\\"purchaseTime\\\":1720068533954,\\\"purchaseState\\\":0,\\\"purchaseToken\\\":\\\"keooennkpadp\\\",\\\"quantity\\\":1,\\\"autoRenewing\\\":true,\\\"acknowledged\\\":false}\",\"signature\":\"ioYx7QUHMqehwvUWRABcg==\",\"skuDetails\":[\"{\\\"productId\\\":\\\"com.indiez.nonogram.premiumpack\\\",\\\"type\\\":\\\"subs\\\",\\\"title\\\":\\\"Premium Pack (Pixel Art: Logic Nonogram)\\\",\\\"name\\\":\\\"Premium Pack\\\",\\\"description\\\":\\\"\\\",\\\"price\\\":\\\"\\\\u20ab49,000\\\",\\\"price_amount_micros\\\":\\\"49000000000\\\",\\\"price_currency_code\\\":\\\"VND\\\",\\\"subscriptionPeriod\\\":\\\"P1W\\\",\\\"freeTrialPeriod\\\":\\\"P3D\\\",\\\"introductoryPrice\\\":\\\"\\\\u20ab441,000\\\",\\\"introductoryPricePeriod\\\":\\\"P10W\\\",\\\"introductoryPriceCycles\\\":1,\\\"introductoryPriceAmountMicros\\\":441000000000}\"]}","Store":"GooglePlay","TransactionID":"keooennkpadpekpmgjlmaobb.AO-J1OyTZBT-CHrp6fqjBJhotFnHUNHCfKNaD3Gq3ho4JoBFy4lEgO9EXzBcyOUOvXAT3hx4U6DBakaXEi1y__o_42GD24Uwww"}
    ///
    /// Formatted receipt json
    /// {
    ///     "Payload": "{\"json\":\"{\\\"orderId\\\":\\\"GPA.3329-0999-2703-63867\\\",\\\"packageName\\\":\\\"com.indiez.nonogram\\\",\\\"productId\\\":\\\"com.indiez.nonogram.premiumpack\\\",\\\"purchaseTime\\\":1720068533954,\\\"purchaseState\\\":0,\\\"purchaseToken\\\":\\\"keooennkpadp\\\",\\\"quantity\\\":1,\\\"autoRenewing\\\":true,\\\"acknowledged\\\":false}\",\"signature\":\"ioYx7QUHMqehwvUWRABcg==\",\"skuDetails\":[\"{\\\"productId\\\":\\\"com.indiez.nonogram.premiumpack\\\",\\\"type\\\":\\\"subs\\\",\\\"title\\\":\\\"Premium Pack (Pixel Art: Logic Nonogram)\\\",\\\"name\\\":\\\"Premium Pack\\\",\\\"description\\\":\\\"\\\",\\\"price\\\":\\\"\\\\u20ab49,000\\\",\\\"price_amount_micros\\\":\\\"49000000000\\\",\\\"price_currency_code\\\":\\\"VND\\\",\\\"subscriptionPeriod\\\":\\\"P1W\\\",\\\"freeTrialPeriod\\\":\\\"P3D\\\",\\\"introductoryPrice\\\":\\\"\\\\u20ab441,000\\\",\\\"introductoryPricePeriod\\\":\\\"P10W\\\",\\\"introductoryPriceCycles\\\":1,\\\"introductoryPriceAmountMicros\\\":441000000000}\"]}",
    ///     "Store": "GooglePlay",
    ///     "TransactionID": "keooennkpadpekpmgjlmaobb.AO-J1OyTZBT-CHrp6fqjBJhotFnHUNHCfKNaD3Gq3ho4JoBFy4lEgO9EXzBcyOUOvXAT3hx4U6DBakaXEi1y__o_42GD24Uwww"
    /// }
    ///
    /// Formatted Payload json
    /// {
    ///     "json": "{\"orderId\":\"GPA.3329-0999-2703-63867\",\"packageName\":\"com.indiez.nonogram\",\"productId\":\"com.indiez.nonogram.premiumpack\",\"purchaseTime\":1720068533954,\"purchaseState\":0,\"purchaseToken\":\"keooennkpadp\",\"quantity\":1,\"autoRenewing\":true,\"acknowledged\":false}",
    ///     "signature": "ioYx7QUHMqehwvUWRABcg==",
    ///     "skuDetails": [
    ///         "{\"productId\":\"com.indiez.nonogram.premiumpack\",\"type\":\"subs\",\"title\":\"Premium Pack (Pixel Art: Logic Nonogram)\",\"name\":\"Premium Pack\",\"description\":\"\",\"price\":\"\\u20ab49,000\",\"price_amount_micros\":\"49000000000\",\"price_currency_code\":\"VND\",\"subscriptionPeriod\":\"P1W\",\"freeTrialPeriod\":\"P3D\",\"introductoryPrice\":\"\\u20ab441,000\",\"introductoryPricePeriod\":\"P10W\",\"introductoryPriceCycles\":1,\"introductoryPriceAmountMicros\":441000000000}"
    ///     ]
    /// }
    ///
    /// Formatted json inside Payload json
    /// {
    ///     "orderId": "GPA.3329-0999-2703-63867",
    ///     "packageName": "com.indiez.nonogram",
    ///     "productId": "com.indiez.nonogram.premiumpack",
    ///     "purchaseTime": 1720068533954,
    ///     "purchaseState": 0,
    ///     "purchaseToken": "keooennkpadp",
    ///     "quantity": 1,
    ///     "autoRenewing": true,
    ///     "acknowledged": false
    /// }
    ///
    /// Formatted sku details
    /// {
    ///     "productId": "com.indiez.nonogram.premiumpack",
    ///     "type": "subs",
    ///     "title": "Premium Pack (Pixel Art: Logic Nonogram)",
    ///     "name": "Premium Pack",
    ///     "description": "",
    ///     "price": "₫49,000",
    ///     "price_amount_micros": "49000000000",
    ///     "price_currency_code": "VND",
    ///     "subscriptionPeriod": "P1W",
    ///     "freeTrialPeriod": "P3D",
    ///     "introductoryPrice": "₫441,000",
    ///     "introductoryPricePeriod": "P10W",
    ///     "introductoryPriceCycles": 1,
    ///     "introductoryPriceAmountMicros": 441000000000
    /// }
    ///  
    /// </summary>
    [System.Serializable]
    public class GooglePlaySubscriptionReceipt
    {
        public static GooglePlaySubscriptionReceipt NewInstance(string receipt) => JsonConvert.DeserializeObject<GooglePlaySubscriptionReceipt>(receipt);

        [JsonProperty("Payload")] public string PayloadString { get; private set; }

        [JsonIgnore] private PayloadObject _payload;
        [JsonIgnore] public PayloadObject Payload => _payload ??= JsonConvert.DeserializeObject<PayloadObject>(PayloadString);

        [JsonProperty("Store")] public string Store { get; private set; }

        [JsonProperty("TransactionID")] public string TransactionID { get; private set; }

        [System.Serializable]
        public class PayloadObject
        {
            [System.Serializable]
            public class JsonObject
            {
                [JsonProperty("orderId")] public string OrderId { get; private set; }

                [JsonProperty("packageName")] public string PackageName { get; private set; }

                [JsonProperty("productId")] public string ProductId { get; private set; }

                [JsonProperty("purchaseTime")] public long PurchaseTime { get; private set; }

                [JsonProperty("purchaseState")] public int PurchaseState { get; private set; }

                [JsonProperty("purchaseToken")] public string PurchaseToken { get; private set; }

                [JsonProperty("quantity")] public int Quantity { get; private set; }

                [JsonProperty("autoRenewing")] public bool AutoRenewing { get; private set; }

                [JsonProperty("acknowledged")] public bool Acknowledged { get; private set; }
            }

            public class SkuDetail
            {
                [JsonProperty("productId")] public string ProductId { get; private set; }

                [JsonProperty("type")] public string Type { get; private set; }

                [JsonProperty("title")] public string Title { get; private set; }

                [JsonProperty("name")] public string Name { get; private set; }

                [JsonProperty("description")] public string Description { get; private set; }

                [JsonProperty("price")] public string Price { get; private set; }

                [JsonProperty("price_amount_micros")] public string PriceAmountMicros { get; private set; }

                [JsonProperty("price_currency_code")] public string PriceCurrencyCode { get; private set; }

                [JsonProperty("subscriptionPeriod")] public string SubscriptionPeriod { get; private set; }

                [JsonProperty("freeTrialPeriod")] public string FreeTrialPeriod { get; private set; }

                [JsonProperty("introductoryPrice")] public string IntroductoryPrice { get; private set; }

                [JsonProperty("introductoryPricePeriod")]
                public string IntroductoryPricePeriod { get; private set; }

                [JsonProperty("introductoryPriceCycles")]
                public int IntroductoryPriceCycles { get; private set; }

                [JsonProperty("introductoryPriceAmountMicros")]
                public string IntroductoryPriceAmountMicros { get; private set; }
            }

            [JsonProperty("json")] public string JsonString { get; private set; }

            private JsonObject _json;
            public JsonObject Json => _json ??= JsonConvert.DeserializeObject<JsonObject>(JsonString);

            [JsonProperty("signature")] public string Signature { get; private set; }

            [JsonProperty("skuDetails")] public List<string> StringSkuDetails { get; private set; }

            private List<SkuDetail> _skuDetails = null;

            public List<SkuDetail> SkuDetails
            {
                get
                {
                    if (_skuDetails != null) return _skuDetails;
                    _skuDetails = new List<SkuDetail>();
                    foreach (var str in StringSkuDetails) _skuDetails.Add(JsonConvert.DeserializeObject<SkuDetail>(str));
                    return _skuDetails;
                }
            }
        }
    }
}