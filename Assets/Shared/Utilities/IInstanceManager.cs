using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared.Utils
{
    public interface IInstanceManager<T>
    {
        T Get(string key);
    }
}