namespace Shared.Core.View.Binding.Formatter
{
    public interface ITextFormatter<in T>
    {
        string Format(T v);
    }
}