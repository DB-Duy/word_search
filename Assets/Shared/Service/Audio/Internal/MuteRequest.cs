using Service.Audio.Internal;

namespace Shared.Service.Audio.Internal
{
    [System.Serializable]
    public class MuteRequest : IMuteRequest
    {
        public string Name { get; }

        public MuteRequest(string name)
        {
            Name = name;
        }
    }
}