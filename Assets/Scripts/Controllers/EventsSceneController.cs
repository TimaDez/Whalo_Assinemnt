using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Models;
using Navigation;
using UnityEngine;
using Whalo.Services;

namespace Whalo.Controllers
{
    public class EventsSceneController : MonoBehaviour
    {
        #region Editor

        [SerializeField] private Transform _mainPanel;
        [SerializeField] private EventsPopupsModel _eventsPopupsModel;

        #endregion
        
        #region Private Fields

        private readonly Stack<EventsPopupData> _popupsStack =  new();

        #endregion

        #region MyRegion

        private void Awake()
        {
            CreatePopupsStackByOrder();
        }

        private async void Start()
        {
            await ShowPopupsSequence();
            await UniTask.WaitForSeconds(0.2f, cancellationToken: this.GetCancellationTokenOnDestroy());
            await SceneManagementSystem.LoadSceneAsync(ScenesNavigation.MENU_SCENE_NAME);
        }

        private void CreatePopupsStackByOrder()
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

        #endregion
    }
}