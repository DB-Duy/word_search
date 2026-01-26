#if ADJUST || APPS_FLYER
using System;
using System.Collections.Generic;
using CoreCLR.NCalc;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Core.IoC.UnityLifeCycle;
using Shared.Entity.Mmp;
using Shared.Repository.Mmp;
using Shared.Tracking.Templates;
using Shared.Utils;
using Shared.Utilities;
using UnityEngine;
using Zenject;
using IInitializable = Shared.Core.Handler.Corou.Initializable.IInitializable;

namespace Shared.Service.Mmp
{
    [Service]
    public class MmpService : IInitializable, ISharedUtility, IUnityUpdate
    {
        [Inject] private MmpConfigRepository _configRepository;

        private List<MmpConfig.EventConfig> _eventConfigs;
        private float _timeRestriction = 0;
        
        public bool IsInitialized { get; private set; }
        private IAsyncOperation _initOperation;
        
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation().Success();
            IsInitialized = true;
            _eventConfigs = _configRepository.Get();
            _timeRestriction = _ResolveTimeRestriction();
            return _initOperation;
        }
        
        public void Track(ITrackingEvent e)
        {
            if (_eventConfigs == null)
            {
                this.LogInfo(SharedLogTag.TrackingNMmp, "action", "ignore", nameof(_eventConfigs), "null");
                return;
            }

            if (_eventConfigs.Count == 0)
            {
                this.LogInfo(SharedLogTag.TrackingNMmp, "action", "ignore", nameof(_eventConfigs), "empty");
                return;
            }

            // if (!_eventConfigs.ContainsKey(e.EventName))
            // {
            //     this.LogInfo(SharedLogTag.TrackingNMmp, "action", "ignore", nameof(e.EventName), e.EventName, nameof(_eventConfigs), _eventConfigs);
            //     return;
            // }

            if (e is IConvertableEvent convertableEvent)
            {
                var p = convertableEvent.ToConvertableEvent();
                foreach (var config in _eventConfigs)
                {
                    if (config.EventName != e.EventName) continue;
                    var val = Validate(p, config.ParamCondition);
                    if (!val) continue;
                    this.LogInfo(SharedLogTag.TrackingNMmp, "action", "track", nameof(e), e, nameof(config), config);

#if ADJUST
                    if (!string.IsNullOrEmpty(config.adjustEventToken))
                    {
                        this.LogInfo(SharedLogTag.TrackingNMmpNAdjust, "action", "adjust", nameof(config.adjustEventToken), config.adjustEventToken);
                        AdjustSdk.Adjust.TrackEvent(new AdjustSdk.AdjustEvent(config.adjustEventToken));
                    }
                    else
                    {
                        this.LogError(SharedLogTag.TrackingNMmpNAdjust, "action", "adjust", nameof(config.adjustEventToken), "null or empty");
                    }
#endif
                    
#if APPS_FLYER
                    if (!string.IsNullOrEmpty(config.appsflyerEventName))
                    {
                        this.LogInfo(SharedLogTag.TrackingNMmpNAppsFlyer, "action", "appsflyer", nameof(config.appsflyerEventName), config.appsflyerEventName);
                        var eventValues = new Dictionary<string, string>();
                        AppsFlyerSDK.AppsFlyer.sendEvent(config.appsflyerEventName, eventValues);
                    }
                    else
                    {
                        this.LogError(SharedLogTag.TrackingNMmpNAppsFlyer, "action", "appsflyer", nameof(config.appsflyerEventName), "null or empty");
                    }
#endif
                }
            }
            else
            {
                this.LogInfo(SharedLogTag.TrackingNMmp, "action", "ignore", nameof(e), "e is not IConvertableEvent");
                return;
            }
        }

        public bool Validate(Dictionary<string, object> parameters, string condition)
        {
            try
            {
                if (string.IsNullOrEmpty(condition))
                {
                    this.LogError(SharedLogTag.TrackingNMmp, "error", "string.IsNullOrEmpty(condition)");
                    return false;
                }

                // NCalc uses single quotes for string literals in the expression
                // Let's adjust the input condition if it uses double quotes for strings
                // This is a simple replacement, more robust parsing might be needed for complex cases
                var ncalcCondition = condition;// condition.Replace("='", "=='").Replace("' ", " '"); // Ensure == for comparison, handle spaces
                ncalcCondition = ncalcCondition.Replace("\"", "'"); // Replace double quotes with single quotes for NCalc strings
                
                var expression = new Expression(ncalcCondition);

                // Set parameters for the expression from the dictionary
                foreach (var kvp in parameters)
                {
                    // Important: NCalc uses the key name directly as the parameter name
                    expression.Parameters[kvp.Key] = kvp.Value;
                }

                // Evaluate the expression
                var result = expression.Evaluate();
                
                this.LogInfo(SharedLogTag.TrackingNMmp, "action", "validate", nameof(result), result, nameof(condition), condition, "for", parameters);

                // Ensure the result is a boolean
                if (result is bool boolResult)
                {
                    return boolResult;
                }
                this.LogError(SharedLogTag.TrackingNMmp, "reason", $"Warning: Expression did not evaluate to a boolean. Result: {result}");
                return false; // Or throw an exception, depending on desired behavior
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                this.LogError(SharedLogTag.TrackingNMmp, $"An unexpected error occurred: {ex.Message}");
                // Handle other potential errors during setup or evaluation
                return false; // Or re-throw
            }
        }
        
        public void Update()
        {
            if (!IsInitialized) return;
            _timeRestriction -= Time.unscaledDeltaTime;
            if (_timeRestriction <= 0)
            {
                _eventConfigs = _configRepository.Get();
                _timeRestriction = _ResolveTimeRestriction();
            }
        }

        private float _ResolveTimeRestriction()
        {
            return _eventConfigs == null || _eventConfigs.IsEmpty() ? 3f : 10f;
        }
    }
}
#endif