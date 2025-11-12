using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Models;
using Navigation;
using UnityEngine;
using UnityEngine.UI;
using Whalo.Services;

namespace Whalo.Controllers
{
    public class EventsSceneController : MonoBehaviour
    {
        #region Editor

        [SerializeField] private Transform _mainPanel;
        [SerializeField] private Transform _backButton;
        [SerializeField] private GameObject _uiBlocker;
        [SerializeField] private GameObject _loadingPanel;
        [SerializeField] private Image _loadingImage;
        [SerializeField] private EventsPopupsModel _eventsPopupsModel;

        #endregion
        
        #region Private Fields

        private Tween _spin;
        private readonly Stack<EventsPopupData> _popupsStack =  new();

        #endregion

        #region MyRegion

        private void Awake()
        {
            _uiBlocker.SetActive(false);
            _loadingPanel.SetActive(false);
            _backButton.localScale = Vector3.zero;
            CreatePopupsStack();
        }

        private async void Start()
        {
            await ShowPopupsSequence();
            _backButton.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
        }

        private void CreatePopupsStack()
        {
            var popups = _eventsPopupsModel.GetOrderedPopupsData();
            foreach (var popup in popups)
            {
                _popupsStack.Push(popup);
            }
        }
        
        private async UniTask ShowPopupsSequence()
        {
            while (_popupsStack.TryPop(out var popup))
            {
                if (popup == null || popup.EventPopupPrefab == null)
                    continue;

                var instance = Instantiate(popup.EventPopupPrefab, _mainPanel);
                await instance.ShowPopupSequence(popup.Url);

                // Destroy(instance.gameObject);
            }
        }

        public void OnBackButtonClicked()
        {
            SceneManagementSystem.LoadSceneAsync(ScenesNavigation.MENU_SCENE_NAME);
        }
        #endregion
    }
}