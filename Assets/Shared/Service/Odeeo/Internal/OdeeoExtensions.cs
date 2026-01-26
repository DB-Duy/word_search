#if ODEEO_AUDIO
using System.Collections.Generic;
using Odeeo.Data;

namespace Shared.Service.Odeeo.Internal
{
    public static class OdeeoExtensions
    {
        private const string Tag = "OdeeoExtensions";
        public static Dictionary<string, object> ToDict(this OdeeoImpressionData impressionData)
        {
            var logObject = new Dictionary<string, object>
            {
                { "SessionID", impressionData.GetSessionID() },
                { "PlacementType", impressionData.GetPlacementType().ToString() },
                { "PlacementID", impressionData.GetPlacementID() },
                { "Country", impressionData.GetCountry() },
                { "PayableAmount", impressionData.GetPayableAmount() },
                { "TransactionID", impressionData.GetTransactionID() },
                { "CustomTag", impressionData.GetCustomTag() }
            };
            return logObject;
        }
        
        public static Dictionary<string, object> ToDict(this OdeeoAdData adData)
        {
            var logObject = new Dictionary<string, object>
            {
                { "SessionID", adData.GetSessionID() },
                { "PlacementType", adData.GetPlacementType().ToString() },
                { "PlacementID", adData.GetPlacementID() },
                { "Country", adData.GetCountry() },
                { "PayableAmount", adData.GetEcpm() },
                { "TransactionID", adData.GetTransactionID() },
                { "CustomTag", adData.GetCustomTag() }
            };
            return logObject;
        }
    }
}
#endif