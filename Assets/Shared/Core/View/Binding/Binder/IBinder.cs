namespace Shared.Core.View.Binding.Binder
{
    public interface IBinder
    {
        IBinder Bind();
        void Unbind();
    }
}