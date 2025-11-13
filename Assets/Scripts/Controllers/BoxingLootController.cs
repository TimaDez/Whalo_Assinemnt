using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Extension;
using Models;
using Navigation;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Whalo.Services;
using Whalo.UI;

namespace Whalo.Controllers
{
    public class BoxingLootController : MonoBehaviour
    {
        #region Editor

        [SerializeField] private LootBoxesView _lootBoxesView;
        
        [SerializeField] private LootBox[] _boxes;
        [SerializeField] private Transform[] _viewsContainers;
        [SerializeField] private Button[] _boxButtons;
        
        [Header("Models")]
        [SerializeField] private LevelModel _levelModel;
        
        #endregion

        #region Private Variables

        private int _clickedCount = 0;
        private List<PrizeModel> _shuffledPrizes;
        private PlayerModelSingleton _playerModel;
        private Stack<bool> _boxesNotOpened = new Stack<bool>();
        #endregion

        #region Methods

        private void Awake()
        {
            _playerModel =  PlayerModelSingleton.Instance;
            
            _shuffledPrizes = _levelModel.Prizes.Shuffled();
            foreach (var _ in _levelModel.Prizes)
                _boxesNotOpened.Push(true);
            
            _lootBoxesView.Initialize(_levelModel, _shuffledPrizes);
            _lootBoxesView.OnButtonClickedEvent += OnButtonClicked;
        }

        private async void Start()
        {
            _playerModel.Initialize(0, 0, _levelModel.KeyStarterAmount);
            await _lootBoxesView.InitView();
        }
        
        private void OnButtonClicked(int buttonIndex)
        {
            OnBoxClickAnimFlow(buttonIndex).Forget();
        }

        private async UniTask OnBoxClickAnimFlow(int buttonIndex)
        {
            _playerModel.WithdrawKeys(1);
            await _lootBoxesView.OnBoxClickAnimFlow(buttonIndex, this.GetCancellationTokenOnDestroy());
            
            if (_boxesNotOpened.TryPop(out var result))
            {
                var prize = _shuffledPrizes[buttonIndex];
                _playerModel.AddPrize(prize.Type, prize.Amount);
                
                if(_playerModel.KeysBalance <= 0)
                    OnLevelEnd().Forget();
            }
        }
        
        private async UniTask OnLevelEnd()
        {
            await _lootBoxesView.OnLevelEnd();
            await SceneManagementSystem.LoadSceneAsync(ScenesNavigation.SUMMERY_SCREEN_NAME);
        }
        
        private void OnDisable()
        {
            _lootBoxesView.OnButtonClickedEvent -= OnButtonClicked;
            _playerModel = null;
        }

        #endregion
    }
}