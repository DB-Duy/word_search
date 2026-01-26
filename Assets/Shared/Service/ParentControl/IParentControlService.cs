using System.Collections.Generic;
using Shared.Core.Handler.Corou.Initializable;
using Shared.Entity.ParentControl;

namespace Shared.Service.ParentControl
{
    public interface IParentControlService : IInitializable
    {
        ParentControlEntity Get();
        
        List<int> GetYears();
        void SaveYear(int year);
        int GetYear();
        int GetAge();
        
        void SetStep(Step policy);
        Step GetStep();
        Step EvaluateNextStep();
        
        void SaveGender(Gender gender);
        Gender GetGender();

        void Sync();
    }
}