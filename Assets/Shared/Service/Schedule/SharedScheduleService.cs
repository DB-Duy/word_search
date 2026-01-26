using System;
using System.Collections.Generic;
using Shared.Core.IoC;
using Shared.Core.IoC.UnityLifeCycle;
using Shared.Service.Schedule.Internal;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.Schedule
{
    [Service]
    public class SharedScheduleService : IUnityUpdate, ISharedScheduleService
    {
        private const string Tag = "SharedScheduleService";
        private readonly Dictionary<string, ISharedRunner> _scheduleTasks = new();
        private readonly List<ISharedRunner> _finishedTasks = new();

        public ISharedRunner ScheduleOnce(string id, Action action, float delayInSeconds, bool replaceMode = false) => Schedule(id, action, delayInSeconds, replaceMode, forever: false);

        public ISharedRunner ScheduleForever(string id, Action action, float delayInSeconds, bool replaceMode = false) => Schedule(id, action, delayInSeconds, replaceMode, forever: true);

        public ISharedRunner Schedule(string id, Action action, float delayInSeconds, bool replaceMode = false, bool forever = false)
        {
            lock (_scheduleTasks)
            {
                var task = new SharedRunner(id, delayInSeconds, action, forever);
                if (!replaceMode) return _scheduleTasks.AddIfNotExisted(id, task) ? task : null;
                _scheduleTasks.Upsert(id, task);
                task.StartSchedule();
                return task;
            }
        }

        public void Schedule(ISharedRunner taskDefine)
        {
            lock (_scheduleTasks)
            {
                if (_scheduleTasks.ContainsKey(taskDefine.TaskName)) throw new Exception($"{Tag}->Schedule: _scheduleTasks.ContainsKey({taskDefine.TaskName})");
                _scheduleTasks.Add(taskDefine.TaskName, taskDefine);
                taskDefine.StartSchedule();
            }
        }

        public bool RemoveSchedule(ISharedRunner taskDefine)
        {
            lock (_scheduleTasks)
            {
                return RemoveSchedule(taskDefine.TaskName);    
            }
        }

        public bool RemoveSchedule(string id)
        {
            lock (_scheduleTasks)
            {
                return _scheduleTasks.Remove(id);    
            }
        }

        public void Update()
        {
            if (Time.frameCount % 5 == 0) _ExecuteTasks();
        }

        private void _ExecuteTasks()
        {
            lock (_scheduleTasks)
            {
                if (_scheduleTasks.Count <= 0) return;
                foreach (var t in _scheduleTasks)
                {
                    if (t.Value.IsStarted && t.Value.Validate())
                    {
                        t.Value.ExecuteTask();
                        if (!t.Value.Forever) _finishedTasks.Add(t.Value);
                    }
                }

                if (_finishedTasks.Count > 0)
                {
                    foreach (var task in _finishedTasks) _scheduleTasks.Remove(task.TaskName);
                    _finishedTasks.Clear();
                }
            }
        }
    }
}