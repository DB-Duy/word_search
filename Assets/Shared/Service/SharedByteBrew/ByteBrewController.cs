#if BYTE_BREW
using ByteBrewSDK;
using Shared.Common;

namespace Shared.SharedByteBrew
{
    /// <summary>
    /// https://docs.bytebrew.io/sdk/unity
    /// </summary>
    public class ByteBrewController : IByteBrewController
    {
        public bool IsInitialized => _initOperation != null && _initOperation.IsComplete;
        private IAsyncOperation _initOperation;
        
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            ByteBrew.InitializeByteBrew();
            _initOperation = new SharedAsyncOperation().Success();
            return _initOperation;
        }
    }
}
#endif