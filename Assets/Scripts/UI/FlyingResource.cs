using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Whalo.UI
{
    public class FlyingResource : MonoBehaviour
    {
        #region Editor
        
        [SerializeField] private Image _image;
        [SerializeField] private float _flyDuration = 0.5f;
        
        #endregion

        #region Propeties

        public int Amount { get; private set; }
        
        #endregion

        public void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }

        public async UniTask FlyTo(Transform targetView)
        {
            //var position = targetView.Target.position;
            transform.DOMoveX(targetView.position.x, _flyDuration).ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
            await transform.DOMoveY(targetView.position.y, _flyDuration).SetEase(Ease.InCubic).ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
            
            //targetView.OnResourceFlyComplete(this);
            //await _animator.SetTrigger("Out");
            gameObject.SetActive(false);
        }
    }
}