using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Models;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Whalo.UI;

namespace Whalo.View
{
    [Serializable]
    public class LootBoxData
    {
        #region Editor

        [SerializeField] private Transform _lootBox;
        [SerializeField] private GameObject _lootBoxDoor;
        [SerializeField] private Transform  _prizeContainer;
        [SerializeField] private Button _button;

        #endregion

        #region Private Members

        private PrizeView  _prizeView;
        private bool _isOpen;

        #endregion
        
        #region Properties
        
        public GameObject LootBoxDoor => _lootBoxDoor;
        public Transform PrizeContainer => _prizeContainer;
        public Button Button => _button;
        public bool IsOpen => _isOpen;

        #endregion
        
        #region Methods

        public void SetContainerView(PrizeView prizeView)
        {
            _prizeView = prizeView;
        }

        public async UniTask AnimateBoxOnOpen()
        {
            await OpenBox();
            await ShowCounter();
        }
        
        public async UniTask OpenBox()
        {
            if(_isOpen)
                return;
            
            _isOpen = true;
            
            var t = _lootBox.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.25f, 10, 0.9f);
            await t.AsyncWaitForCompletion();
            _lootBoxDoor.gameObject.SetActive(false);
        }
        
        private async UniTask ShowCounter()
        {
            _prizeView.transform.localScale = Vector3.zero;
            _prizeView.gameObject.SetActive(true);

            await _prizeView.transform.DOScale(Vector3.one, 0.2f).AsyncWaitForCompletion();
        }
        
        #endregion

    }
}