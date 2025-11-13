using Cysharp.Threading.Tasks;
using DataTypes;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Whalo.Infrastructure;

namespace Whalo.UI
{
    public class FlyingResource : MonoBehaviour
    {
        #region Editor
        
        [SerializeField] private Image _image;
        [SerializeField] private float _flyDuration = 0.5f;
        
        #endregion

        public void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }

        public async UniTask FlyTo(Transform targetView)
        {
            transform.DOMoveX(targetView.position.x, _flyDuration).ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
            await transform.DOMoveY(targetView.position.y, _flyDuration).SetEase(Ease.InCubic).ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
            
            SoundManager.Instance.PlaySFX(SfxType.Coin);
            gameObject.SetActive(false);
        }
    }
}