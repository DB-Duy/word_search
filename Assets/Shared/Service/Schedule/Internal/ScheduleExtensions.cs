using System;
using Shared.Core.IoC;

namespace Shared.Service.Schedule.Internal
{
    public static class ScheduleExtensions
    {
        private static ISharedScheduleService _service;

        private static ISharedScheduleService Service
        {
            get
            {
                if (_service == null) _service = IoCExtensions.Resolve<ISharedScheduleService>();
                return _service;
            }
        }

        public static ISharedRunner ScheduleOnce(this ISharedUtility o, string id, Action action, float delayInSeconds, bool replaceMode = false) 
            => Service.ScheduleOnce(id, action, delayInSeconds, replaceMode);
        
        public static ISharedRunner ScheduleForever(this ISharedUtility o, string id, Action action, float delayInSeconds, bool replaceMode = false) 
            => Service.ScheduleForever(id, action, delayInSeconds, replaceMode);

        public static ISharedRunner Schedule(this ISharedUtility o, string id, Action action, float delayInSeconds, bool replaceMode = false, bool forever = false) 
            => Service.Schedule(id, action, delayInSeconds, replaceMode, forever);

        public static void Schedule(this ISharedUtility o, ISharedRunner taskDefine) 
            => Service.Schedule(taskDefine);

        public static bool RemoveSchedule(this ISharedUtility o, ISharedRunner taskDefine) 
            => Service.RemoveSchedule(taskDefine);
        public static bool RemoveSchedule(this ISharedUtility o, string id) 
            => Service.RemoveSchedule(id);
    }
}