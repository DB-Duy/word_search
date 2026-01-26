namespace Shared.Service.Schedule.Internal
{
    public interface ISharedRunner
    {
        string TaskName { get; }
        bool Forever { get; }
        bool IsStarted { get; }

        void StartSchedule();
        bool Validate();
        bool ExecuteTask();
    }
}