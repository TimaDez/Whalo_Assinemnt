using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Navigation;
using UnityEngine;
using UnityEngine.UI;
using Whalo.UI;

namespace Whalo.Controllers
{
    public class EventsSceneController : MonoBehaviour
    {
        #region Editor

        [SerializeField] private GameObject _uiBlocker;
        [SerializeField] private GameObject _loadingPanel;
        [SerializeField] private Image _loadingImage;
        [SerializeField] private EventPopupBase _popupPrefab;

        #endregion
        
        #region Private Fields

        private Tween _spin;
        private Stack<string> _popupsStack =  new Stack<string>();

        #endregion

        #region MyRegion

        private void Awake()
        {
            _uiBlocker.SetActive(false);
            _loadingPanel.SetActive(false);
            CreatePopupsStack();
        }

        private void CreatePopupsStack()
        {
            _popupsStack.Push(NetworkNavigation.COINS_EVENT_POPUP_LINK);
            _popupsStack.Push(NetworkNavigation.THREE_KEYS_POPUP_LINK);
        }

        private async UniTask ShowPopupsSequence()
        {
            var popupsCount = _popupsStack.Count - 1;
            for (int i = 0; i < popupsCount; i++)
            {
                if(_popupsStack.Count == 0)
                    return;

                if (_popupsStack.TryPop(out var popup))
                {
                    var p = Instantiate(_popupPrefab);
                    await p.StartShowPopupSequence();
                }
            }
        }
        #endregion
    }
}