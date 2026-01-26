using System;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;

namespace Shared.Service.Iap.Editor.Tests
{
    public class TestServerTimeFetcher
    {
        
        [UnityTest]
        public IEnumerator TestGetServerTime()
        {
            _ = GetGoogleServerTimeIso8601Async();
            yield return null;
        }
        
        public static async Task<string> GetGoogleServerTimeIso8601Async()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    // Chỉ cần HEAD request để lấy header
                    var request = new HttpRequestMessage(HttpMethod.Head, "https://www.google.com");
                    var response = await client.SendAsync(request);

                    if (response.Headers.Date.HasValue)
                    {
                        DateTime utcDateTime = response.Headers.Date.Value.UtcDateTime;
                        // DateTime.UtcNow
                        Debug.Log(utcDateTime.ToString() + " vs " + DateTime.UtcNow.ToString());

                        // Trả về định dạng ISO 8601
                        return utcDateTime.ToString("o"); // ISO 8601: 2025-06-05T04:07:02.0000000Z
                    }
                    else
                    {
                        return "Date header not found.";
                    }
                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}";
                }
            }
        }
    }
}