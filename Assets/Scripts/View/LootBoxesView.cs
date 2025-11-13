using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DataTypes;
using Infrastructure;
using Models;
using Navigation;
using UnityEngine;
using UnityEngine.Serialization;
using Whalo.Models;
using Whalo.View;

namespace Whalo.UI
{
    public class ResourceData
    {
        public Sprite Sprite;
        public Transform EndPivot;
        public CounterView Container;
    }
    
    public class LootBoxesView : MonoBehaviour
    {
        #region Events

        public event Action<int> OnButtonClickedEvent;

        #endregion
        
        #region Editor
        [SerializeField] private GameObject _uiBlocker;
        [SerializeField] private Transform _poolObjectsParent;
        [SerializeField] private FlyingResource _flyingResourcePrefab;
        
        [FormerlySerializedAs("_keyCounter")]
        [Header("Counters")]
        [SerializeField] private CounterView _key;
        [SerializeField] private CounterView _energy;
        [SerializeField] private CounterView _coin;

        [Header("Prize Pivots")]
        [SerializeField] private Transform _keyPivot;
        [SerializeField] private Transform _energyPivot;
        [SerializeField] private Transform _coinPivot;
        
        [SerializeField] private LootBoxData[] _boxesData;
        
        #endregion

        #region Private Fields

        private LevelModel _levelModel;
        private Sprite _keySprite;
        private Sprite _coinsSprite;
        private Sprite _energySprite;
        private ObjectsPool<FlyingResource> _flyingResourcePool;
        private Dictionary<PrizeType, ResourceData> _resourcesData;
        
        private PlayerModelSingleton _playerModel;
        private List<PrizeModel> _shuffledPrizes;
        
        #endregion
        
        #region Methods

        private void Awake()
        {
            _uiBlocker.SetActive(false);
            //For debug
            var instance = Models.PlayerModelSingleton.EnsureInstance();
            _playerModel = instance;
            
            _flyingResourcePool = new ObjectsPool<FlyingResource>(_flyingResourcePrefab, _poolObjectsParent);
            SubscribeBoxButtons();
            SubscribeModelEvens();
        }

        public void Initialize(LevelModel levelModel, List<PrizeModel> shuffledPrizes)
        {
            _levelModel = levelModel;
            _shuffledPrizes = shuffledPrizes;
            CreateCountersView();
        }

        private async UniTaskVoid OnButtonClicked(int index)
        {
            if (_boxesData[index].IsOpen)
                return;
            
            OnButtonClickedEvent?.Invoke(index);
        }
        
        public async UniTask OnBoxClickAnimFlow(int index, CancellationToken token)
        {
            _uiBlocker.SetActive(true);
            await _boxesData[index].AnimateBoxOnOpen();
            
            var prize = _shuffledPrizes[index];
            await FlyResource(prize.Type, _boxesData[index].PrizeContainer, prize.Amount);
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, token);
            
            _uiBlocker.SetActive(false);
        }
        
        public async UniTask OnLevelEnd()
        {
            var tasks = new List<UniTask>();

            foreach (var box in _boxesData)
            {
                if (box.IsOpen)
                    continue;

                tasks.Add(box.AnimateBoxOnOpen());
            }

            await UniTask.WhenAll(tasks);
        }
        
        public async UniTask InitView()
        {
            var token = this.GetCancellationTokenOnDestroy();

            (_keySprite, _coinsSprite, _energySprite) = await UniTask.WhenAll(
                SpriteLoader.GetSpriteAsync(NetworkNavigation.KEY_IMAGE_LINK, token),
                SpriteLoader.GetSpriteAsync(NetworkNavigation.COINS_IMAGE_LINK, token),
                SpriteLoader.GetSpriteAsync(NetworkNavigation.ENERGY_IMAGE_LINK, token)
            );
            
            _key.Init(_keySprite, _levelModel.KeyStarterAmount);
            _coin.Init(_coinsSprite);
            _energy.Init(_energySprite);
            
            _resourcesData = new Dictionary<PrizeType, ResourceData>
            {
                { PrizeType.Key,    new ResourceData { Sprite = _keySprite, EndPivot = _keyPivot, Container = _key} },
                { PrizeType.Coins,  new ResourceData { Sprite = _coinsSprite, EndPivot = _coinPivot, Container = _coin} },
                { PrizeType.Gems, new ResourceData { Sprite = _energySprite, EndPivot = _energyPivot, Container = _energy } },
            };
        }
        
        private void CreateCountersView()
        {
            for (int i = 0; i < _shuffledPrizes.Count; i++)
            {
                var prize = Instantiate(_shuffledPrizes[i].PrizeViewPrefab, _boxesData[i].PrizeContainer);
                prize.SetData(_shuffledPrizes[i]);
                _boxesData[i].SetContainerView(prize);
            }
        }
        
        private void OnKeysBalanceChange(int oldBalance, int newGemsBalance)
        {
            _key.SetAmount(newGemsBalance);
        }

        private void OnGemsBalanceChange(int oldBalance, int newGemsBalance)
        {
            _energy.SetAmount(newGemsBalance);
        }

        private void OnCoinsBalanceChange(int oldBalance, int newGemsBalance)
        {
            _coin.SetAmount(newGemsBalance);
        }

        public async UniTask FlyResource(PrizeType prizeType, Transform startPivot, int amountToAdd)
        {
            var numOfFliers = amountToAdd;
            var amountPerFlier = 1;
            var remainder = 0;
            if (numOfFliers > 10)
            {
                numOfFliers = 10;
                amountPerFlier = amountToAdd / 10;
                remainder = amountToAdd % 10;
            }
        
            var balance = _playerModel.GetBalance(prizeType);
            var data = _resourcesData[prizeType];
            
            var tasks = new List<UniTask>();
            for (var i = 0; i < numOfFliers; i++)
            {
                var nextAmount = i == numOfFliers - 1 ? amountPerFlier + remainder : amountPerFlier;

                tasks.Add(FlyTo(startPivot, data.Sprite, data.EndPivot));
                await UniTask.Delay(TimeSpan.FromSeconds(0.15f), cancellationToken: this.GetCancellationTokenOnDestroy());
                balance += nextAmount;
                data.Container.SetAmount(balance);
            }
        
            await UniTask.WhenAll(tasks);
        }

        private async UniTask FlyTo(Transform startPivot, Sprite sprite, Transform endPivot)
        {
             var resource = _flyingResourcePool.GetItem();
             resource.transform.position = startPivot.position;
             resource.SetSprite(sprite);
             
             await resource.FlyTo(endPivot);
        }
        
        private void SubscribeBoxButtons()
        {
            for (var i = 0; i < _boxesData.Length; i++)
            {
                var index = i;
                _boxesData[i].Button.onClick.AddListener(() => OnButtonClicked(index).Forget());
            }
        }
        
        private void UnsubscribeBoxButtons()
        {
            for (var i = 0; i < _boxesData.Length; i++)
            {
                var index = i;
                _boxesData[i].Button.onClick.RemoveListener(() => OnButtonClicked(index).Forget());
            }
        }
        
        private void SubscribeModelEvens()
        {
            _playerModel.CoinsBalanceChange += OnCoinsBalanceChange;
            _playerModel.GemsBalanceChange += OnGemsBalanceChange;
            _playerModel.KeysBalanceChange += OnKeysBalanceChange;
        }

        private void UnsubscribeModelEvens()
        {
            _playerModel.CoinsBalanceChange -= OnCoinsBalanceChange;
            _playerModel.GemsBalanceChange -= OnGemsBalanceChange;
            _playerModel.KeysBalanceChange -= OnKeysBalanceChange;
        }
        
        private void OnDestroy()
        {
            UnsubscribeModelEvens();
            UnsubscribeBoxButtons();
            _playerModel = null;
            _flyingResourcePool.ClearAll();
        }

        #endregion
    }
}