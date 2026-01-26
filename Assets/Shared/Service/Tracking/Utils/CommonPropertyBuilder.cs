using System.Collections.Generic;
using System.Linq;
using Shared.Service.Tracking.Property;
using Shared.Tracking.Property;
using Shared.Utils;

namespace Shared.Tracking.Utils
{
    public class CommonPropertyBuilder : IPropertyBuilder
    {
        private readonly Dictionary<string, ITrackingProperty> _data = new();

        public ITrackingProperty[] Build() => _data.Values.ToArray();
        
        public IPropertyBuilder SetSessionId(long sessionId)
        {
            _data.Upsert(PropertyConst.SESSION_ID, new TrackingProperty(PropertyConst.SESSION_ID, sessionId));
            return this;
        }

        public IPropertyBuilder SetGameMode(string gameMode)
        {
            _data.Upsert(PropertyConst.GAME_MODE, new TrackingProperty(PropertyConst.GAME_MODE, gameMode));
            return this;
        }

        public IPropertyBuilder SetTheme(string theme)
        {
            _data.Upsert(PropertyConst.THEME, new TrackingProperty(PropertyConst.THEME, theme));
            return this;
        }

        public IPropertyBuilder SetLevel(long level)
        {
            _data.Upsert(PropertyConst.LEVEL, new TrackingProperty(PropertyConst.LEVEL, level));
            return this;
        }

        public IPropertyBuilder SetSubLevel(long subLevel)
        {
            _data.Upsert(PropertyConst.SUB_LEVEL, new TrackingProperty(PropertyConst.SUB_LEVEL, subLevel));
            return this;
        }

        public IPropertyBuilder SetStage(string stage)
        {
            _data.Upsert(PropertyConst.STAGE, new TrackingProperty(PropertyConst.STAGE, stage));
            return this;
        }

        public IPropertyBuilder SetSegment(string segment)
        {
            _data.Upsert(PropertyConst.SEGMENT, new TrackingProperty(PropertyConst.SEGMENT, segment));
            return this;
        }

        public IPropertyBuilder SetStartCount(long startCount)
        {
            _data.Upsert(PropertyConst.START_COUNT, new TrackingProperty(PropertyConst.START_COUNT, startCount));
            return this;
        }

    }
}