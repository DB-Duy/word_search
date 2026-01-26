// #if UNITY_EDITOR
// using Shared.Core.Async;
// using Shared.Core.IoC;
// using Shared.Service.Ads.Common;
// using Shared.Service.AudioAds;
// using Shared.Service.AudioAds.Validator;
// using Shared.Utils;
// using Shared.View.AudioAds;
//
// namespace Shared.Service.Ads.Fake
// {
//     [Service]
//     public class FakeAudioAdsService : IAudioAdsService
//     {
//         private const string Tag = "AudioAdsService";
//
//         public IAudioAdsService AddValidators(params IAudioAdValidator[] validators)
//         {
//             return this;
//         }
//
//         public bool Validate()
//         {
//             return true;
//         }
//
//         public void Register(AudioAdController controller)
//         {
//             // throw new System.NotImplementedException();
//         }
//
//         public void Register(IAudioAdPlacement audioAd)
//         {
//             // throw new System.NotImplementedException();
//         }
//
//         public void Unregister(IAudioAdPlacement audioAd)
//         {
//             // throw new System.NotImplementedException();
//         }
//
//         public IAsyncOperation Play(IAudioAdPlacement placement)
//         {
//             SharedLogger.Log($"{Tag}->AudioAds Play->Placement: {placement}");
//             var playOperation = new SharedAsyncOperation();
//             playOperation.Success();
//             return playOperation;
//         }
//     }
// }
// #endif