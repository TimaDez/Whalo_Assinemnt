using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataTypes;
using Infrastructure;
using Models;
using Navigation;
using UI;
using UnityEngine;
using UnityEngine.Pool;

namespace Whalo.UI
{
    public class ResourceData
    {
        public Sprite Sprite;
        public Transform EndPivot;
        public ViewCounter Container;
    }
    
    public class LootBoxesView : MonoBehaviour
    {
        #region Editor

        [SerializeField] private Transform _poolObjectsParent;
        [SerializeField] private FlyingResource _flyingResourcePrefab;
        
        [Header("Counters")]
        [SerializeField] private ViewCounter _keyCounter;
        [SerializeField] private ViewCounter _energyCounter;
        [SerializeField] private ViewCounter _coinCounter;

        [Header("Prize Pivots")]
        [SerializeField] private Transform _keyPivot;
        [SerializeField] private Transform _energyPivot;
        [SerializeField] private Transform _coinPivot;
        
        #endregion

        #region Private Fields

        private PlayerModel _playerModel;
        private LevelModel _levelModel;
        private Sprite _keySprite;
        private Sprite _coinsSprite;
        private Sprite _energySprite;
        private ObjectsPool<FlyingResource> _flyingResourcePool;
        private Dictionary<PrizeType, ResourceData> _resourceData;
        
        #endregion
        
        #region Methods

        private void Awake()
        {
            _flyingResourcePool = new ObjectsPool<FlyingResource>(_flyingResourcePrefab, _poolObjectsParent);
        }

        public void Initialize(LevelModel levelModel, PlayerModel playerModel)
        {
            _levelModel = levelModel;
            _playerModel = playerModel;
            
            _playerModel.CoinsBalanceChange += OnCoinsBalanceChange;
            _playerModel.GemsBalanceChange += OnGemsBalanceChange;
            _playerModel.KeysBalanceChange += OnKeysBalanceChange;
        }
        
        public async UniTask InitView()
        {
            var token = this.GetCancellationTokenOnDestroy();

            (_keySprite, _coinsSprite, _energySprite) = await UniTask.WhenAll(
                SpriteLoader.GetSpriteAsync(NetworkNavigation.KEY_IMAGE_LINK, token),
                SpriteLoader.GetSpriteAsync(NetworkNavigation.COINS_IMAGE_LINK, token),
                SpriteLoader.GetSpriteAsync(NetworkNavigation.ENERGY_IMAGE_LINK, token)
            );
            
            _keyCounter.Init(_keySprite, _levelModel.KeyStarterAmount);
            _coinCounter.Init(_coinsSprite);
            _energyCounter.Init(_energySprite);
            
            _resourceData = new Dictionary<PrizeType, ResourceData>
            {
                { PrizeType.Key,    new ResourceData { Sprite = _keySprite, EndPivot = _keyPivot, Container = _keyCounter} },
                { PrizeType.Coins,  new ResourceData { Sprite = _coinsSprite, EndPivot = _coinPivot, Container = _coinCounter} },
                { PrizeType.Gems, new ResourceData { Sprite = _energySprite, EndPivot = _energyPivot, Container = _energyCounter } },
            };
        }
        
        private void OnKeysBalanceChange(int oldBalance, int newGemsBalance)
        {
            _keyCounter.SetAmount(newGemsBalance);
        }

        private void OnGemsBalanceChange(int oldBalance, int newGemsBalance)
        {
            _energyCounter.SetAmount(newGemsBalance);
        }

        private void OnCoinsBalanceChange(int oldBalance, int newGemsBalance)
        {
            _coinCounter.SetAmount(newGemsBalance);
        }

        public async UniTask FlyFrom(PrizeType prizeType, Transform startPivot, int amountToAdd)
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
            var data = _resourceData[prizeType];
            
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

        private void OnDestroy()
        {
            _playerModel.CoinsBalanceChange -= OnCoinsBalanceChange;
            _playerModel.GemsBalanceChange -= OnGemsBalanceChange;
            _playerModel.KeysBalanceChange -= OnKeysBalanceChange;

            _flyingResourcePool.ClearAll();
        }

        #endregion
    }
}