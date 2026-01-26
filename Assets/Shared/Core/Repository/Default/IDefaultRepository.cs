using System;

namespace Shared.Core.Repository.Default
{
    public interface IDefaultRepository
    {
        string GetName(Type type);
        bool GetBool(Type type);
        string GetString(Type type);
        int GetInt(Type type);
        long GetLong(Type type);
        T GetObject<T>(Type type);
    }
}