using Shared.Tracking.Property;

namespace Shared.Tracking.Utils
{
    public interface IPropertyBuilder
    {
        ITrackingProperty[] Build();

        IPropertyBuilder SetSessionId(long sessionId);
        IPropertyBuilder SetGameMode(string gameMode);
        IPropertyBuilder SetTheme(string theme);
        IPropertyBuilder SetLevel(long level);
        IPropertyBuilder SetSubLevel(long subLevel);
        IPropertyBuilder SetStage(string stage);
        IPropertyBuilder SetSegment(string segment);
        IPropertyBuilder SetStartCount(long startCount);
    }
}