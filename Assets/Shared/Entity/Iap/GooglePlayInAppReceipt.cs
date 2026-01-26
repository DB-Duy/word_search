using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shared.Entity.Iap
{
    /// <summary>
    /// "{\"Payload\":\"{\\\"json\\\":\\\"{\\\\\\\"orderId\\\\\\\":\\\\\\\"GPA.3346-7240-5557-76013\\\\\\\",\\\\\\\"packageName\\\\\\\":\\\\\\\"com.indiez.penguin.dash\\\\\\\",\\\\\\\"productId\\\\\\\":\\\\\\\"com.indiez.penguin.dash.p1\\\\\\\",\\\\\\\"purchaseTime\\\\\\\":1720429489203,\\\\\\\"purchaseState\\\\\\\":0,\\\\\\\"purchaseToken\\\\\\\":\\\\\\\"bjpiapfbomaijncaibpfmjle.AO-J1OwKSko8ZfNABWpk-aUHirctM-gPonqOIVm_LwJe8X2XyiUQFAQ3CnF1wQXNSjNTTRez861KuD1nbW1d6DVBZoXrUsdIb-dBeAodhTVYbNsytuxq8gs\\\\\\\",\\\\\\\"quantity\\\\\\\":1,\\\\\\\"acknowledged\\\\\\\":false}\\\",\\\"signature\\\":\\\"f3GNrQYPuBRpahkFQK98y/jcukoVRowyDQSu6Mzc0+j8D4uJP4AS9IuW8TA3I0zAntwoLNCqhCH1A6YE6YeoJXwFnLrbancK1q9dug7cBzQo+b0HHbj3+QZQgHTtWknY8Svwzrl6zMiHaG1c9G9zFaAbh/Y6I/lTNorrzZR6Ej1pwssUsk5RrXk6wiRn+GLtYtfL5/SZUHfvtEumdpln4xCRBm8/fPvxEOLcXZ0xrBKeZcuEVZTxUDDLZaivIXAsKBkU4s4EzEAwaBrfupA04QzDfa1BCq4z2RS6jYQQDXySQhXk65nZdFPBWZJ4eFFsZf0jlhXWCivEUs3yKkzvMw==\\\",\\\"skuDetails\\\":[\\\"{\\\\\\\"productId\\\\\\\":\\\\\\\"com.indiez.penguin.dash.p1\\\\\\\",\\\\\\\"type\\\\\\\":\\\\\\\"inapp\\\\\\\",\\\\\\\"title\\\\\\\":\\\\\\\"Coin Pack 1 (Penguin Dash: Run Race 3D)\\\\\\\",\\\\\\\"name\\\\\\\":\\\\\\\"Coin Pack 1\\\\\\\",\\\\\\\"iconUrl\\\\\\\":\\\\\\\"https:\\\\\\\\/\\\\\\\\/lh3.googleusercontent.com\\\\\\\\/87elkd53B9c59jHvpcghibzdw6Up4cFrG96GqUnYm3O5TNBeWHhBS1nzCG4wq_OzXyjA\\\\\\\",\\\\\\\"description\\\\\\\":\\\\\\\"Coin Pack 1\\\\\\\",\\\\\\\"price\\\\\\\":\\\\\\\"\\\\u20ac1.99\\\\\\\",\\\\\\\"price_amount_micros\\\\\\\":1990000,\\\\\\\"price_currency_code\\\\\\\":\\\\\\\"EUR\\\\\\\",\\\\\\\"skuDetailsToken\\\\\\\":\\\\\\\"AEuhp4LYzrJ6oJ7jDTVCnbJ5540CMYyLmvjqVwvDclG8BJFRM1EwC-FekpFdTesbM_5Uk6voi-tlPzfPZC8gwBXxWWGuW69d2mGJB9kFlTk15_AFn1b6\\\\\\\"}\\\"]}\",\"Store\":\"GooglePlay\",\"TransactionID\":\"bjpiapfbomaijncaibpfmjle.AO-J1OwKSko8ZfNABWpk-aUHirctM-gPonqOIVm_LwJe8X2XyiUQFAQ3CnF1wQXNSjNTTRez861KuD1nbW1d6DVBZoXrUsdIb-dBeAodhTVYbNsytuxq8gs\"}"
    /// "{"Payload":"{\"json\":\"{\\\"orderId\\\":\\\"GPA.3328-6737-3827-56788\\\",\\\"packageName\\\":\\\"com.realbizgames.f2p.games.dice.woody.puzzle\\\",\\\"productId\\\":\\\"com.realbizgames.f2p.games.dice.woody.puzzle.remove.all.ads\\\",\\\"purchaseTime\\\":1718724854317,\\\"purchaseState\\\":0,\\\"purchaseToken\\\":\\\"agdoicajnjelfnbeieahjodn.AO-J1OwyPSisOfqcyl8oYEWAvxLMIHd3U1hDxKQGfbI0WUo-WaqxAz-9fTLSOioVoVHQaoJ-F0yYQzdA2ngi9NZXZAsh0dJgN32vGiQbvBWt2r7W36XuizxKs4ZeCGQnTB7BvTh4cbUP\\\",\\\"quantity\\\":1,\\\"acknowledged\\\":true}\",\"signature\":\"XCYLV+NgeTVOksKejyF/RepbADwOHfk3pc21di+CJUi3SdUUXN/6KKsx10Phfu6g7U4XXTtnBf2egVRQTXfkaUnZYrDVOv5Vf4BPCNcaChchBmjXSwkM6Vknk0Mj52TpKqZ2mW+MI573MYY/kf14J8uZwc6XTzn67yNd0L0pMWOpMMPubQUQ7jLSwJCSBTASSF2O0i/hhsfUZDXPJZ6xxsfBY/yEJKEzpa8O1yqkPU44g9EKKP6b3aO7PpikY1QAOlK9QtXHpR6EO5c/7igDwG9M26V/htUHlCajielW9g1wmZwfM1ce/RnHoxli/TKYIB+uN7l1PJn3hDVEMMu5zg==\",\"skuDetails\":[\"{\\\"productId\\\":\\\"com.realbizgames.f2p.games.dice.woody.puzzle.remove.all.ads\\\",\\\"type\\\":\\\"inapp\\\",\\\"title\\\":\\\"Remove All Banner and Video Ads (Merge Dice - Brain Master)\\\",\\\"name\\\":\\\"Remove All Banner and Video Ads\\\",\\\"description\\\":\\\"This game is yours. Keep it your game clean, professional now!\\\",\\\"price\\\":\\\"\\\\u20ab105,000\\\",\\\"price_amount_micros\\\":\\\"105000000000\\\",\\\"price_currency_code\\\":\\\"VND\\\"}\"]}","Store":"GooglePlay","TransactionID":"agdoicajnjelfnbeieahjodn.AO-J1OwyPSisOfqcyl8oYEWAvxLMIHd3U1hDxKQGfbI0WUo-WaqxAz-9fTLSOioVoVHQaoJ-F0yYQzdA2ngi9NZXZAsh0dJgN32vGiQbvBWt2r7W36XuizxKs4ZeCGQnTB7BvTh4cbUP"}"
    /// {
    ///     "Payload": "{\"json\":\"{\\\"orderId\\\":\\\"GPA.3346-7240-5557-76013\\\",\\\"packageName\\\":\\\"com.indiez.penguin.dash\\\",\\\"productId\\\":\\\"com.indiez.penguin.dash.p1\\\",\\\"purchaseTime\\\":1720429489203,\\\"purchaseState\\\":0,\\\"purchaseToken\\\":\\\"bjpiapfbomaijncaibpfmjle.AO-J1OwKSko8ZfNABWpk-aUHirctM-gPonqOIVm_LwJe8X2XyiUQFAQ3CnF1wQXNSjNTTRez861KuD1nbW1d6DVBZoXrUsdIb-dBeAodhTVYbNsytuxq8gs\\\",\\\"quantity\\\":1,\\\"acknowledged\\\":false}\",\"signature\":\"f3GNrQYPuBRpahkFQK98y/jcukoVRowyDQSu6Mzc0+j8D4uJP4AS9IuW8TA3I0zAntwoLNCqhCH1A6YE6YeoJXwFnLrbancK1q9dug7cBzQo+b0HHbj3+QZQgHTtWknY8Svwzrl6zMiHaG1c9G9zFaAbh/Y6I/lTNorrzZR6Ej1pwssUsk5RrXk6wiRn+GLtYtfL5/SZUHfvtEumdpln4xCRBm8/fPvxEOLcXZ0xrBKeZcuEVZTxUDDLZaivIXAsKBkU4s4EzEAwaBrfupA04QzDfa1BCq4z2RS6jYQQDXySQhXk65nZdFPBWZJ4eFFsZf0jlhXWCivEUs3yKkzvMw==\",\"skuDetails\":[\"{\\\"productId\\\":\\\"com.indiez.penguin.dash.p1\\\",\\\"type\\\":\\\"inapp\\\",\\\"title\\\":\\\"Coin Pack 1 (Penguin Dash: Run Race 3D)\\\",\\\"name\\\":\\\"Coin Pack 1\\\",\\\"iconUrl\\\":\\\"https:\\\\/\\\\/lh3.googleusercontent.com\\\\/87elkd53B9c59jHvpcghibzdw6Up4cFrG96GqUnYm3O5TNBeWHhBS1nzCG4wq_OzXyjA\\\",\\\"description\\\":\\\"Coin Pack 1\\\",\\\"price\\\":\\\"\\u20ac1.99\\\",\\\"price_amount_micros\\\":1990000,\\\"price_currency_code\\\":\\\"EUR\\\",\\\"skuDetailsToken\\\":\\\"AEuhp4LYzrJ6oJ7jDTVCnbJ5540CMYyLmvjqVwvDclG8BJFRM1EwC-FekpFdTesbM_5Uk6voi-tlPzfPZC8gwBXxWWGuW69d2mGJB9kFlTk15_AFn1b6\\\"}\"]}",
    ///     "Store": "GooglePlay",
    ///     "TransactionID": "bjpiapfbomaijncaibpfmjle.AO-J1OwKSko8ZfNABWpk-aUHirctM-gPonqOIVm_LwJe8X2XyiUQFAQ3CnF1wQXNSjNTTRez861KuD1nbW1d6DVBZoXrUsdIb-dBeAodhTVYbNsytuxq8gs"
    /// }
    ///
    /// {
    ///     "json": "{\"orderId\":\"GPA.3346-7240-5557-76013\",\"packageName\":\"com.indiez.penguin.dash\",\"productId\":\"com.indiez.penguin.dash.p1\",\"purchaseTime\":1720429489203,\"purchaseState\":0,\"purchaseToken\":\"bjpiapfbomaijncaibpfmjle.AO-J1OwKSko8ZfNABWpk-aUHirctM-gPonqOIVm_LwJe8X2XyiUQFAQ3CnF1wQXNSjNTTRez861KuD1nbW1d6DVBZoXrUsdIb-dBeAodhTVYbNsytuxq8gs\",\"quantity\":1,\"acknowledged\":false}",
    ///     "signature": "f3GNrQYPuBRpahkFQK98y/jcukoVRowyDQSu6Mzc0+j8D4uJP4AS9IuW8TA3I0zAntwoLNCqhCH1A6YE6YeoJXwFnLrbancK1q9dug7cBzQo+b0HHbj3+QZQgHTtWknY8Svwzrl6zMiHaG1c9G9zFaAbh/Y6I/lTNorrzZR6Ej1pwssUsk5RrXk6wiRn+GLtYtfL5/SZUHfvtEumdpln4xCRBm8/fPvxEOLcXZ0xrBKeZcuEVZTxUDDLZaivIXAsKBkU4s4EzEAwaBrfupA04QzDfa1BCq4z2RS6jYQQDXySQhXk65nZdFPBWZJ4eFFsZf0jlhXWCivEUs3yKkzvMw==",
    ///     "skuDetails": [
    ///         "{\"productId\":\"com.indiez.penguin.dash.p1\",\"type\":\"inapp\",\"title\":\"Coin Pack 1 (Penguin Dash: Run Race 3D)\",\"name\":\"Coin Pack 1\",\"iconUrl\":\"https:\\/\\/lh3.googleusercontent.com\\/87elkd53B9c59jHvpcghibzdw6Up4cFrG96GqUnYm3O5TNBeWHhBS1nzCG4wq_OzXyjA\",\"description\":\"Coin Pack 1\",\"price\":\"€1.99\",\"price_amount_micros\":1990000,\"price_currency_code\":\"EUR\",\"skuDetailsToken\":\"AEuhp4LYzrJ6oJ7jDTVCnbJ5540CMYyLmvjqVwvDclG8BJFRM1EwC-FekpFdTesbM_5Uk6voi-tlPzfPZC8gwBXxWWGuW69d2mGJB9kFlTk15_AFn1b6\"}"
    ///     ]
    /// }
    ///
    /// {
    ///     "orderId": "GPA.3346-7240-5557-76013",
    ///     "packageName": "com.indiez.penguin.dash",
    ///     "productId": "com.indiez.penguin.dash.p1",
    ///     "purchaseTime": 1720429489203,
    ///     "purchaseState": 0,
    ///     "purchaseToken": "bjpiapfbomaijncaibpfmjle.AO-J1OwKSko8ZfNABWpk-aUHirctM-gPonqOIVm_LwJe8X2XyiUQFAQ3CnF1wQXNSjNTTRez861KuD1nbW1d6DVBZoXrUsdIb-dBeAodhTVYbNsytuxq8gs",
    ///     "quantity": 1,
    ///     "acknowledged": false
    /// }
    ///
    /// {
    ///     "productId": "com.indiez.penguin.dash.p1",
    ///     "type": "inapp",
    ///     "title": "Coin Pack 1 (Penguin Dash: Run Race 3D)",
    ///     "name": "Coin Pack 1",
    ///     "iconUrl": "https://lh3.googleusercontent.com/87elkd53B9c59jHvpcghibzdw6Up4cFrG96GqUnYm3O5TNBeWHhBS1nzCG4wq_OzXyjA",
    ///     "description": "Coin Pack 1",
    ///     "price": "€1.99",
    ///     "price_amount_micros": 1990000,
    ///     "price_currency_code": "EUR",
    ///     "skuDetailsToken": "AEuhp4LYzrJ6oJ7jDTVCnbJ5540CMYyLmvjqVwvDclG8BJFRM1EwC-FekpFdTesbM_5Uk6voi-tlPzfPZC8gwBXxWWGuW69d2mGJB9kFlTk15_AFn1b6"
    /// }
    /// </summary>
    [System.Serializable]
    public class GooglePlayInAppReceipt
    {
        public static GooglePlayInAppReceipt NewInstance(string receipt) => JsonConvert.DeserializeObject<GooglePlayInAppReceipt>(receipt);

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

                [JsonProperty("acknowledged")] public bool Acknowledged { get; private set; }
            }

            public class SkuDetail
            {
                [JsonProperty("productId")] public string ProductId { get; private set; }

                [JsonProperty("type")] public string Type { get; private set; }

                [JsonProperty("title")] public string Title { get; private set; }

                [JsonProperty("name")] public string Name { get; private set; }
                
                [JsonProperty("iconUrl")] public string IconUrl { get; private set; }

                [JsonProperty("description")] public string Description { get; private set; }

                [JsonProperty("price")] public string Price { get; private set; }

                [JsonProperty("price_amount_micros")] public string PriceAmountMicros { get; private set; }

                [JsonProperty("price_currency_code")] public string PriceCurrencyCode { get; private set; }
                
                [JsonProperty("skuDetailsToken")] public string SkuDetailsToken { get; private set; }
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