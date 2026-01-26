using System;

namespace Shared.Core.Repository.RemoteConfig
{
    public interface IFirebaseRemoteConfigRepository
    {
        string Name { get; }
        Action OnValueChangedEvent { get; set; }
    }
}