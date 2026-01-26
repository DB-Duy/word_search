using Shared.SharedReport.Version;

namespace Shared.SharedReport
{
    public interface ISharedVersionReport
    {
        void Add(IVersionGetter getter);
        void Report();
    }
}