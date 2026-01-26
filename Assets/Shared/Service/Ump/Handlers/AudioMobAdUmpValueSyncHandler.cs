#if AUDIO_MOB
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.Ump;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.Ump.Handlers
{
    [Component]
    public class AudioMobAdUmpValueSyncHandler : IHandler<UmpEntity>, ISharedUtility, IUmpValueSyncHandler
    {
        public void Handle(UmpEntity e)
        {
            _SetInt("AM_GDPR_Consent", e.GdprApplies);
            _SetString("AM_GDPR_Consent_String", e.TcString);
            _SetString("AM_US_Privacy", e.UsPrivacyString);
            PlayerPrefs.Save();
        }

        private void _SetInt(string key, int value) 
        {
            this.LogInfo(SharedLogTag.UmpNAudioMob, nameof(key), key, nameof(value), value);
            PlayerPrefs.SetInt(key, value);
        }
        
        private void _SetString(string key, string value) 
        {
            this.LogInfo(SharedLogTag.UmpNAudioMob, nameof(key), key, nameof(value), value);
            PlayerPrefs.SetString(key, value);
        }
    }
}
#endif