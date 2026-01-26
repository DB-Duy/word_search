#if CHEAT_TIME_DETECTOR
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Shared.Core.IoC;
using Shared.Core.IoC.UnityLifeCycle;
using Shared.Repository.Cheat;
using Shared.Utils;
using UnityEngine;
using Zenject;

namespace Shared.Service.Cheat
{
    [Service]
    public class CheatTimeDetector : IUnityOnApplicationPause, ISharedUtility
    {
        
        [Inject] private CheatTimeConfigRepository _repository;
        
        public void OnApplicationPause(bool pauseStatus)
        {
            _ = _DetectCheatTimeAsync();
        }
        
        private async Task _DetectCheatTimeAsync()
        {
            var config = _repository.Get();
            if (config == null || !config.Unlocked) return;
            
            using var client = new HttpClient();
            try
            {
                // Chỉ cần HEAD request để lấy header
                // "https://www.google.com"
                var request = new HttpRequestMessage(HttpMethod.Head, config.Url);
                var response = await client.SendAsync(request);

                if (response.Headers.Date.HasValue)
                {
                    var serverTimeUtc = response.Headers.Date.Value.UtcDateTime;
                    var allowedOffset = TimeSpan.FromMinutes(config.AllowedOffset);
                    var actualOffset = (DateTime.UtcNow - serverTimeUtc).Duration();
                    CheatTimeFlag.IsCheating = actualOffset > allowedOffset;
                    if (CheatTimeFlag.IsCheating)
                    {
                        Debug.LogError("CheatTimeFlag.IsCheating");
                    }
                }
                
            }
            catch (Exception ex)
            {
                this.LogError(SharedLogTag.CheatNTime, nameof(ex), ex.Message);
            }
        }
    }
}
#endif