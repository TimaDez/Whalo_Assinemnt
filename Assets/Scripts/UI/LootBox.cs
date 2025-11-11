using Cysharp.Threading.Tasks;
using DG.Tweening;
using Infrastructure;
using UnityEngine;

namespace UI
{
    public class LootBox : MonoBehaviour, IInteractable
    {
        #region Editor

        [SerializeField] private GameObject _capGraphic;

        #endregion

        #region Private Members

        private bool _isOpen = false;

        #endregion
        
        #region Properties
        
        public bool IsOpen => _isOpen;
        
        #endregion

        #region Methods

        public async UniTask Interact()
        {
            if(_isOpen)
                return;
            
            _isOpen = true;
            Debug.Log($"[LootBox] Interact()");
            
            _capGraphic.SetActive(false);
            var t = transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.25f, 10, 0.9f);
            await t.AsyncWaitForCompletion();
            
            //TODO: animation
            //TODO: animation
        }

        #endregion
    }
}