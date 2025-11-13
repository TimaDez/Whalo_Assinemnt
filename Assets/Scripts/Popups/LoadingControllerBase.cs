using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Infrastructure;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace Popups
{
    public abstract class LoadingControllerBase : MonoBehaviour, ILoadingScreen
    {
        #region Editor

        [SerializeField] private GameObject _loadingPanel;
        [SerializeField] private Image _loadingImage;

        [SerializeField] private CanvasGroup _group;
        [SerializeField] private float _duration = 1.3f;
        
        #endregion
        
        #region Private Fields

        private Tween _spin;
        protected List<string> _eventsPopups;

        #endregion

        private void Awake()
        {
            _loadingPanel.SetActive(false);
        }

        public async UniTask Load()
        {
            CreateUrlsList();
            await FadeInAsync();
            StartLoadingAnimation();
            await SpritesLoaderService.LoadSprites(_eventsPopups);
            await FadeOutAsync();
        }

        protected abstract void CreateUrlsList();
        
        private async UniTask FadeInAsync()
        {
            _group.gameObject.SetActive(true);
            _group.alpha = 0f;
        
            await _group
                .DOFade(1f, _duration)
                .SetEase(Ease.OutCubic)
                .AsyncWaitForCompletion();
        }
        
        private async UniTask FadeOutAsync()
        {
            await _group
                .DOFade(0f, _duration)
                .SetEase(Ease.InCubic)
                .AsyncWaitForCompletion();
        
            _group.gameObject.SetActive(false);
        }
        
        private void StartLoadingAnimation()
        {
            _loadingPanel.SetActive(true);
            _spin = _loadingImage.transform
                .DORotate(new Vector3(0, 0, -360), 3f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }

        private void OnDisable()
        {
            _spin?.Kill();
        }
    }
}