using UnityEngine;

public class FlagLock : MonoBehaviour
{
    const string TAG = "FlagLock";

    public string Name { get; }

    public bool IsLock { get; private set; } = false;
    public bool IsNotLock => !IsLock;

    public FlagLock(string name, bool isLock = false)
    {
        Name = name;
        IsLock = isLock;
    }

    public void Lock()
    {
#if LOG_INFO
        Debug.LogFormat("{0}->Lock({1})", TAG, Name);
#endif
        IsLock = true;
    }

    public void Unlock()
    {
#if LOG_INFO
        Debug.LogFormat("{0}->Unlock({1})", TAG, Name);
#endif
        IsLock = false;
    }

    public override string ToString() => string.Format("[{0} name={1}, isLock={2}]", TAG, Name, IsLock);
}
