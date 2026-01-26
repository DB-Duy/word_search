using System;
using System.Collections.Generic;

namespace Shared.Core
{
    public class SharedException : Exception
    {
        public SharedException(List<string> tags, string classTag, string message, params object[] jsonParams) 
            : base($"{tags.ToJsonString()} {classTag}->{message} {StringUtils.ToJsonString(jsonParams)}")
        {
        }
        
        public SharedException(string tag, string classTag, string message, params object[] jsonParams) 
            : base($"[{tag}] {classTag}->{message} {StringUtils.ToJsonString(jsonParams)}")
        {
        }
    }
}