using System;
using Shared.Core.Repository.BoolType;
using Shared.Core.Repository.IntType;
using Shared.Core.Repository.LongType;
using Shared.Core.Repository.ObjectType;
using Shared.Core.Repository.StringType;

namespace Shared.Core.Repository
{
    public static class RepositoryEvents
    {
        public static Action<IBoolRepository, bool> OnBoolValueChangedEvent = delegate {  };
        public static Action<IIntRepository, int, int> OnIntValueChangedEvent = delegate {  };
        public static Action<ILongRepository, long, long> OnLongValueChangedEvent = delegate {  };
        public static Action<IStringRepository, string, string> OnStringValueChangedEvent = delegate {  };
    }
}