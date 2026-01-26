#if ODEEO_AUDIO
using System;
using Odeeo.Data;

namespace Shared.Service.Odeeo
{
    public static class OdeeoEvents
    {
        
        public static Action OnInitializationFinishedEvent = delegate {  };
        /// <summary>
        /// int errorParam, string error
        /// </summary>
        public static Action<int, string> OnInitializationFailedEvent = delegate {  };
        // -------------------------------------------------------------------------------------------------------------
        // Callbacks from placement
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// (string placement, float value);
        /// </summary>
        public static Action<string, float> OnPlacementRewardEvent = delegate {  };
        /// <summary>
        /// (string placement, bool flag);
        /// </summary>
        /// <returns></returns>
        public static Action<string, bool> OnPlacementAvailabilityChangedEvent = delegate {  };
        /// <summary>
        /// (string placement);
        /// </summary>
        public static Action<string> OnPlacementClickEvent = delegate {  };
        /// <summary>
        /// (string placement);
        /// </summary>
        public static Action<string>  OnPlacementCloseEvent = delegate {  };
        /// <summary>
        /// (string placement);
        /// </summary>
        public static Action<string>  OnPlacementShowEvent = delegate {  };
        /// <summary>
        /// (string placement);
        /// </summary>
        public static Action<string>  OnPlacementPauseEvent = delegate {  };
        /// <summary>
        /// (string placement);
        /// </summary>
        public static Action<string>  OnPlacementResumeEvent = delegate {  };
        /// <summary>
        /// (string placement);
        /// </summary>
        public static Action<string>  OnPlacementMuteEvent = delegate {  };
        /// <summary>
        /// (string placement, OdeeoImpressionData entity);
        /// </summary>
        public static Action<string, OdeeoImpressionData> OnPlacementImpressionEvent = delegate {  };
        /// <summary>
        /// (string placement, string errorCode, string des);
        /// </summary>
        public static Action<string, string, string> OnPlacementShowFailedEvent = delegate {  };
    }
}
#endif