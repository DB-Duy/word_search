using System.Collections.Generic;

namespace Shared.SharedDevToDev.Internal
{
    public interface IDevToDevConfig
    {
        string AppId { get; }
        int Level { get; }
        Dictionary<string, object> DefaultRemoteConfigDictionary { get; }
        bool StrictMode { get; }
    }
}