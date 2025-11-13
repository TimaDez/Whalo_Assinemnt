using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Infrastructure;
using Models;
using Navigation;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Whalo.Services;

namespace Whalo.Controllers
{
    public class MenuSceneController : MonoBehaviour
    {
        #region Editor

        [SerializeField] private GameObject _uiBlocker;
        [SerializeField] private GameObject _loadingPanel;
        [SerializeField] private Image _loadingImage;

        #endregion

        #region Private Fields

        private Tween _spin;
        private List<string> _eventsPopups;

        #endregion
        
        #region Methods

        private void Awake()
        {
            _uiBlocker.SetActive(false);
            _loadingPanel.SetActive(false);
            CreateEventsList();
        }

        private void Start()
        {
            Models.PlayerModelSingleton.EnsureInstance();
        }

        private void CreateEventsList()
        {
            _eventsPopups = new List<string>
            {
                NetworkNavigation.COINS_EVENT_POPUP_LINK,
                NetworkNavigation.KEYS_EVENT_POPUP_LINK
            };
        }
        
        public void OnStartGameButtonClicked()
        {
            StartLoadingSequence(ScenesNavigation.GAME_PLAY_NAME).Forget();
        }

        public async void OnEventButtonClicked()
        {
            StartLoadingSequence(ScenesNavigation.EVENTS_SCENE_NAME).Forget();
        }

        private async UniTaskVoid StartLoadingSequence(string sceneName)
        {
            StartLoadingAnimation();
            await SpritesLoaderService.LoadSprites(_eventsPopups);
            await SceneManagementSystem.LoadSceneAsync(sceneName);
        }
        
        private void StartLoadingAnimation()
        {
            _uiBlocker.SetActive(true);
            _loadingPanel.SetActive(true);
            _spin = _loadingImage.transform
                .DORotate(new Vector3(0, 0, -360), 3f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }
        
        public void OnQuitButtonClicked()
        {
            Application.Quit();
        }

        private void OnDestroy()
        {
            _spin?.Kill();
        }

        #endregion
    }
}