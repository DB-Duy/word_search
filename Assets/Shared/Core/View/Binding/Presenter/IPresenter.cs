using Shared.Core.View.Binding.Binder;

namespace Shared.Core.View.Binding.Presenter
{
    public interface IPresenter<in T, V> where T : IBinder
    {
        void Present(T binder, V newValue);
    }
}