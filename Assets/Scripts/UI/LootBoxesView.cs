using Cysharp.Threading.Tasks;
using Infrastructure;
using Models;
using Navigation;
using UI;
using UnityEngine;

namespace Whalo.UI
{
    public class LootBoxesView : MonoBehaviour
    {
        #region Editor

        [Header("UI View")]
        [SerializeField] private ViewCounter _keyCounter;
        [SerializeField] private ViewCounter _energyCounter;
        [SerializeField] private ViewCounter _coinCounter;

        #endregion

        #region Private Fields

        private PlayerModel _playerModel;
        private LevelModel _levelModel;

        #endregion
        
        #region Methods

        public void Initialize(LevelModel levelModel, PlayerModel playerModel)
        {
            _levelModel = levelModel;
            _playerModel = playerModel;
        }
        
        public async UniTask InitView()
        {
            var token = this.GetCancellationTokenOnDestroy();

            var (keySprite, coinsSprite, energySprite) = await UniTask.WhenAll(
                SpriteLoader.GetSpriteAsync(NetworkNavigation.KEY_IMAGE_LINK, token),
                SpriteLoader.GetSpriteAsync(NetworkNavigation.COINS_IMAGE_LINK, token),
                SpriteLoader.GetSpriteAsync(NetworkNavigation.ENERGY_IMAGE_LINK, token)
            );

            _playerModel.Initialize(0, 0 ,_levelModel.KeyStarterAmount);
            _playerModel.CoinsBalanceChange += OnCoinsBalanceChange;
            _playerModel.GemsBalanceChange += OnGemsBalanceChange;
            _playerModel.KeysBalanceChange += OnKeysBalanceChange;
            
            _keyCounter.Init(keySprite, _levelModel.KeyStarterAmount);
            _coinCounter.Init(coinsSprite);
            _energyCounter.Init(energySprite);
        }

        
        private void OnKeysBalanceChange(int oldBalance, int newGemsBalance)
        {
            _keyCounter.AddAmount(newGemsBalance);
        }

        private void OnGemsBalanceChange(int oldBalance, int newGemsBalance)
        {
            _energyCounter.AddAmount(newGemsBalance);
        }

        private void OnCoinsBalanceChange(int oldBalance, int newGemsBalance)
        {
            _coinCounter.AddAmount(newGemsBalance);
        }

        #endregion
    }
}