using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Whalo.UI
{
    public abstract class EventPopupBase : MonoBehaviour
    {
        #region Editor

        [SerializeField] private Image _bgImage;
        [SerializeField] private Button _xButton;

        #endregion
        
        #region Methods

        public abstract UniTask StartShowPopupSequence();

        #endregion
    }
}