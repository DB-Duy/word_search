// #if LEVEL_PLAY
// using System.Collections.Generic;
// using Shared.Core.Handler;
// using Shared.Core.IoC;
// using Shared.Entity.ParentControl;
// using Shared.Utils;
// using UnityEngine;
//
// namespace Shared.Service.ParentControl.Partners
// {
//     /// <summary>
//     /// https://developers.is.com/ironsource-mobile/unity/additional-sdk-settings/#step-2
//     /// KHÔNG dùng gì segment k thể set lại lần 2.
//     /// </summary>
//     [Component]
//     public class LevelPlaySyncHandler : IHandler<ParentControlEntity>, IParentControlSyncHandler, ISharedUtility
//     {
//         private static readonly Dictionary<Gender, string> GenderMap = new()
//         {
//             { Gender.Male , "male" },
//             { Gender.Female , "female"}
//         };
//         
//         public void Handle(ParentControlEntity entity)
//         {
//             if(Application.isEditor) return;
//             var segment = new IronSourceSegment
//             {
//                 age = entity.Age
//             };
//             if (GenderMap.TryGetValue(entity.Gender, out var value)) segment.gender = value;
//             
//             IronSource.Agent.setSegment (segment);
//             this.LogInfo(SharedLogTag.ParentControlNLevelPlay, nameof(segment.gender), segment.gender, nameof(segment.age), segment.age);
//         }
//     }
// }
// #endif