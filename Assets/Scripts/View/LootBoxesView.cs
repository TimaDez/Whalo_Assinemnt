using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DataTypes;
using Infrastructure;
using Models;
using Navigation;
using UI;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityEngine.UI;
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
        #region Editor

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
        [SerializeField] private LootBox[] _boxes;
        [SerializeField] private Transform[] _viewsContainers;
        [SerializeField] private Button[] _boxButtons;
        
        #endregion

        #region Private Fields

        private LevelModel _levelModel;
        private Sprite _keySprite;
        private Sprite _coinsSprite;
        private Sprite _energySprite;
        private ObjectsPool<FlyingResource> _flyingResourcePool;
        private Dictionary<PrizeType, ResourceData> _resourcesData;
        
        private PlayerModelSingleton _playerModel;
        
        #endregion
        
        #region Methods

        private void Awake()
        {
            var instance = Models.PlayerModelSingleton.EnsureInstance();
            _playerModel = instance;
            //_playerModel = PlayerModelSingleton.Instance;
            _flyingResourcePool = new ObjectsPool<FlyingResource>(_flyingResourcePrefab, _poolObjectsParent);
        }

        public void Initialize(LevelModel levelModel)
        {
            _levelModel = levelModel;
            SubscribeModelEvens();
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
            _playerModel = null;
            _flyingResourcePool.ClearAll();
        }

        #endregion
    }
}