using Cysharp.Threading.Tasks;
using DataTypes;
using DG.Tweening;
using Infrastructure;
using UnityEngine;
using Whalo.Infrastructure;

namespace Whalo.UI
{
    public class CoinsEventPopup : EventPopupBase
    {
        #region Methods
        
        public override async UniTask ShowPopupSequence(string url)
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            _bgImage.sprite = await SpriteLoader.GetSpriteAsync(url);
            await ShowPopup();
            await _xButton.OnClickAsync();
            SoundManager.Instance.PlaySFX(SfxType.ButtonClick);
            await HidePopup();
        }

        private async UniTask ShowPopup()
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);

            await transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack).ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        private async UniTask HidePopup()
        {
            await transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
            gameObject.SetActive(false);
        }

        #endregion
    }
}