namespace Shared.Entity.ParentControl
{
    public enum Step
    {
        NotStarted = -1,
        AskUserInfo = 1,
        Policy = 4,
        ParentPermission = 6,
        Granted = 7
    }
}