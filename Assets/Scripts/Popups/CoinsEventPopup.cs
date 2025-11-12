using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Infrastructure;
using Navigation;
using UnityEngine;
using UnityEngine.UI;

namespace Whalo.UI
{
    public class CoinsEventPopup : EventPopupBase
    {
        #region Editor

        [SerializeField] private Image _bgImage;
        [SerializeField] private Button _xButton;

        #endregion

        #region Methods
        
        public override async UniTask StartShowPopupSequence()
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            _bgImage.sprite = await SpriteLoader.GetSpriteAsync(NetworkNavigation.COINS_EVENT_POPUP_LINK);
            await ShowPopup();
            await _xButton.OnClickAsync();
            await HidePopup();
        }

        private async UniTask ShowPopup()
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);

            await transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack).ToUniTask();
        }

        private async UniTask HidePopup()
        {
            await transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).ToUniTask();
            gameObject.SetActive(false);
        }

        #endregion
    }
}