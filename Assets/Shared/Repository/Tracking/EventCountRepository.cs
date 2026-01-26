using System.Collections.Generic;
using Shared.Core.IoC;
using Shared.Core.Repository.IntType;
using Shared.Utils;

namespace Shared.Repository.Tracking
{
    [Repository]
    public class EventCountRepository : IEventCountRepository, ISharedUtility
    {
        private readonly Dictionary<string, IIntRepository> _counterDict = new();
        
        public int IncreaseAndGet(string countKey)
        {
            this.LogInfo(nameof(countKey), countKey);
            if (!_counterDict.ContainsKey(countKey))
            {
                var counter = new IntPlayerPrefsRepository(countKey, 0);
                var v = counter.AddMore(1);
                _counterDict.Add(countKey, counter);
                return v;
            }

            return _counterDict[countKey].AddMore(1);
        }
    }
}