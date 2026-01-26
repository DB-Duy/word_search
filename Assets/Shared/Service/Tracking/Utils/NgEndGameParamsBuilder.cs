using System.Collections.Generic;
using Shared.Utils;

namespace Shared.Tracking.Utils
{
    public class NgEndGameParamsBuilder : INgEndGameParamsBuilder
    {
        private readonly Dictionary<string, object> _data = new();
        public Dictionary<string, object> Build() => _data;

        public INgEndGameParamsBuilder SetResult(string result)
        {
            _data.Upsert("result", result);
            return this;
        }

        public INgEndGameParamsBuilder SetLiveRemain(int liveRemain)
        {
            _data.Upsert("live_remain", liveRemain);
            return this;
        }
        
        public INgEndGameParamsBuilder SetCountX(int countX)
        {
            _data.Upsert("count_x", countX);
            return this;
        }

        public INgEndGameParamsBuilder SetContinueCount(int continueCount)
        {
            _data.Upsert("continue", continueCount);
            return this;
        }
    }
}