using System;
using Shared.Service.Schedule.Internal;

namespace Shared.Service.Schedule
{
    public interface ISharedScheduleService
    {
        ISharedRunner ScheduleOnce(string id, Action action, float delayInSeconds, bool replaceMode = false);
        ISharedRunner ScheduleForever(string id, Action action, float delayInSeconds, bool replaceMode = false);
        ISharedRunner Schedule(string id, Action action, float delayInSeconds, bool replaceMode = false, bool forever = false);
        void Schedule(ISharedRunner taskDefine);
        
        bool RemoveSchedule(ISharedRunner taskDefine);
        bool RemoveSchedule(string id);
    }
}