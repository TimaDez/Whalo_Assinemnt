using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DataTypes;
using DG.Tweening;
using Extension;
using Infrastructure;
using Infrastructure.Networking;
using Models;
using Navigation;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField] private PlayerModel _playerModel;
        [SerializeField] private LevelModel _levelModel;

        // [Header("UI View")]
        // [SerializeField] private ViewCounter _keyCounter;
        // [SerializeField] private ViewCounter _energyCounter;
        // [SerializeField] private ViewCounter _coinCounter;
        #endregion

        #region Private Variables

        private bool[] _clicked;
        private int _clickedCount = 0;
        private List<PrizeModel> _shuffledPrizes;

        #endregion

        #region Methods

        private void Awake()
        {
            _lootBoxesView.Initialize(_levelModel, _playerModel);
        }

        private async void Start()
        {
            InitButtons();
            await _lootBoxesView.InitView();
            InitLevel();
        }

        private void InitButtons()
        {
            //RunFlowAsync().Forget();
            
            _clicked = new bool[_boxButtons.Length];
            AddListenerToButton(true);
        }

        private void AddListenerToButton(bool add)
        {
            var count = _boxButtons.Length;
            for (var i = 0; i < count; i++)
            {
                var index = i;
                if(add)
                    _boxButtons[i].onClick.AddListener(() => OnButtonClicked(index).Forget());
                else
                    _boxButtons[i].onClick.RemoveListener(() => OnButtonClicked(index).Forget());
                    
            }
        }
        
        private async UniTask InitLevel()
        {
            //await InitView();
            _shuffledPrizes = _levelModel.Prizes.Shuffled();

            for (int i = 0; i < _shuffledPrizes.Count; i++)
            {
                var prize = Instantiate(_shuffledPrizes[i].PrizeViewPrefab, _viewsContainers[i]);
                prize.SetData(_shuffledPrizes[i]);
            }
        }

        // private async UniTask InitView()
        // {
        //     var token = this.GetCancellationTokenOnDestroy();
        //
        //     var (keySprite, coinsSprite, energySprite) = await UniTask.WhenAll(
        //         SpriteLoader.GetSpriteAsync(NetworkNavigation.KEY_IMAGE_LINK, token),
        //         SpriteLoader.GetSpriteAsync(NetworkNavigation.COINS_IMAGE_LINK, token),
        //         SpriteLoader.GetSpriteAsync(NetworkNavigation.ENERGY_IMAGE_LINK, token)
        //     );
        //
        //     _playerModel.Initialize(0, 0 ,_levelModel.KeyStarterAmount);
        //     _playerModel.CoinsBalanceChange += OnCoinsBalanceChange;
        //     _playerModel.GemsBalanceChange += OnGemsBalanceChange;
        //     _playerModel.KeysBalanceChange += OnKeysBalanceChange;
        //     
        //     _keyCounter.Init(keySprite, _levelModel.KeyStarterAmount);
        //     _coinCounter.Init(coinsSprite);
        //     _energyCounter.Init(energySprite);
        // }

        // private void OnKeysBalanceChange(int oldBalance, int newGemsBalance)
        // {
        //     _keyCounter.AddAmount(newGemsBalance);
        // }
        //
        // private void OnGemsBalanceChange(int oldBalance, int newGemsBalance)
        // {
        //     _energyCounter.AddAmount(newGemsBalance);
        // }
        //
        // private void OnCoinsBalanceChange(int oldBalance, int newGemsBalance)
        // {
        //     _coinCounter.AddAmount(newGemsBalance);
        // }

        private async UniTaskVoid OnButtonClicked(int index)
        {
            if (_clicked[index])
                return;

            _clicked[index] = true;
            _clickedCount++;

            Debug.Log($"Button clicked: {index}");

            _boxButtons[index].interactable = false;
            _playerModel.WithdrawKeys(1);

            await UpdateView(index);
            
            await OnBoxClickAnim(index, this.GetCancellationTokenOnDestroy());

            if (_clickedCount == _boxButtons.Length || _playerModel.KeysBalance <= 0)
            {
                Debug.Log("ALL BUTTONS WERE CLICKED!");
                await OnLevelEnd();
                Debug.Log($"[BoxingLootController] OnButtonClicked() All boxes opened");
            }
        }

        private async UniTask OnLevelEnd()
        {
            //TODO: open all boxes if needed
            var tasks = new List<UniTask>();

            for (var i = 0; i < _boxes.Length; i++)
            {
                if (_boxes[i].IsOpen)
                    continue;

                tasks.Add(OpenBoxFlow(i));  // add full flow into WhenAll
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask OpenBoxFlow(int i)
        {
            var tr = _viewsContainers[i].GetChild(0);
            tr.gameObject.SetActive(true);
            tr.localScale = Vector3.zero;

            await _boxes[i].Interact();
            await tr.DOScale(Vector3.one, 0.2f).AsyncWaitForCompletion();
        }
        
        private async UniTask UpdateView(int buttonIndex)
        {
            var type = _shuffledPrizes[buttonIndex].Type;
            
            Debug.Log($"[BoxingLootController] OnButtonClicked() type: {type}");
            switch (type)
            {
                case PrizeType.Key:
                    _playerModel.AddKeys(_shuffledPrizes[buttonIndex].Amount);
                    break;
                case PrizeType.Gems:
                    _playerModel.AddGems(_shuffledPrizes[buttonIndex].Amount);
                    break;
                case PrizeType.Coins:
                    _playerModel.AddCoins(_shuffledPrizes[buttonIndex].Amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private async UniTask OnBoxClickAnim(int index, CancellationToken token)
        {
            //TODO: Animate
            //TODO: Set UI
            //TODO: Set Sound
            //TODO: Set Partical system
            //TODO: check if no more keys left
            
            if (_boxes[index] != null)
            {
                await _boxes[index].Interact();
                _viewsContainers[index].GetChild(0).gameObject.SetActive(true);
            }
            
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, token);
            
        }
        
        // Your per-click async work (animation, UI updates, SFX, logic)
        private async UniTask OnButtonChosenAsync(int index, Button btn, CancellationToken token)
        {
            Debug.Log($"Chosen index: {index}, button: {btn?.name}");

            // Example: DOTween punch + small settle
            
            if (_boxes[index] != null)
            {
                await _boxes[index].Interact();
                _viewsContainers[index].GetChild(0).gameObject.SetActive(true);
            }

            // TODO: your UI updates here (text, sprites, sounds)
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, token);
        }

        
        private void OnDisable()
        {
            AddListenerToButton(false);
        }

        #endregion
    }
}