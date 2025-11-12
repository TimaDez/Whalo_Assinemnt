using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Whalo.UI
{
    public abstract class EventPopupBase : MonoBehaviour
    {
        #region Editor

        [SerializeField] protected Image _bgImage;
        [SerializeField] protected Button _xButton;

        #endregion
        
        #region Methods

        public abstract UniTask StartShowPopupSequence(string url);

        #endregion
    }
}