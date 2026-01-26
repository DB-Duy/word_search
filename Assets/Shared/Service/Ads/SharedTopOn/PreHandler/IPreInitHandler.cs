#if HUAWEI
using System.Collections;

namespace Shared.Ads.SharedTopOn.PreHandler
{
    public interface IPreInitHandler
    {
        IEnumerator Handle();
    }
}
#endif