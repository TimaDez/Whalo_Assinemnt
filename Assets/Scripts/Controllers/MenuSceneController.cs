using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DataTypes;
using DG.Tweening;
using Infrastructure;
using Models;
using Navigation;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Whalo.Infrastructure;
using Whalo.Services;

namespace Whalo.Controllers
{
    public class MenuSceneController : MonoBehaviour
    {
        #region Editor

        [SerializeField] private GameObject _uiBlocker;
        [SerializeField] private Image _loadingImage;

        #endregion
        
        #region Methods

        private void OnEnable()
        {
            _uiBlocker.SetActive(false);
        }

        private void Start()
        {
            Models.PlayerModelSingleton.EnsureInstance();
        }
        
        public void OnStartGameButtonClicked()
        {
            StartLoadingSequence(ScenesNavigation.GAME_PLAY_NAME, ScenesNavigation.LOADING_IMAGES_SCREEN_NAME).Forget();
        }

        public void OnEventButtonClicked()
        {
            StartLoadingSequence(ScenesNavigation.EVENTS_SCENE_NAME, ScenesNavigation.LOADING_POPUPS_SCREEN_NAME).Forget();
        }

        private async UniTaskVoid StartLoadingSequence(string sceneName, string loadingSceneName)
        {
            SoundManager.Instance.PlaySFX(SfxType.ButtonClick);
            _uiBlocker.SetActive(true);
            var service = await SceneManagementSystem.Get(loadingSceneName);
            await service.Load();
            await SceneManagementSystem.LoadSceneAsync(sceneName);
        }
        
        public void OnQuitButtonClicked()
        {
            SoundManager.Instance.PlaySFX(SfxType.ButtonClick);
            Application.Quit();
        }

        #endregion
    }
}