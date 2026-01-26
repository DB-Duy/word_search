namespace Shared.Core.Repository.BoolType
{
    public class BoolStoreRepository : BoolPlayerPrefsRepository
    {
        public BoolStoreRepository(string name = null, bool defaultValue = false) : base(name, defaultValue)
        {
        }
    }
}