namespace Shared.Core.Handler.Corou.Initializable
{
    public interface IInitializableHandler : ICoroutineHandler
    {
        Config Config { get; }
    }
}