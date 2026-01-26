using System.Collections;
using UnityEngine;

namespace Shared.Core.Presenter
{
    public interface IPresenter
    {
        
    }

    public interface IPresenter<in T> : IPresenter 
        where T : MonoBehaviour
    {
        IEnumerator Present(T o);
    }
    
    public interface IPresenter<in FT, in TT> : IPresenter
        where FT : MonoBehaviour 
        where TT : MonoBehaviour
    {
        IEnumerator Present(FT from, TT to);
    } 
}