using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Models
{
    [CreateAssetMenu(menuName = "Models/Player Model", fileName = "Player Model")]
    public class PlayerModel : ScriptableObject
    {
        #region Events

        public event Action<int, int> CoinsBalanceChange;
        public event Action<int, int> GemsBalanceChange;
        public event Action<int, int> KeysBalanceChange;

        #endregion
        
        #region Editor

        [SerializeField] private int _coinsBalance;
        [SerializeField] private int _gemsBalance;
        [SerializeField] private int _keysBalance;

        #endregion

        #region Private Fields

        private bool _isInitialized;

        #endregion
        
        #region Properties

        public int CoinsBalance => _coinsBalance;

        public int GemsBalance => _gemsBalance;
        public int KeysBalance => _keysBalance;

        #endregion
        
        #region Methods

        public void Initialize(int coinsBalance, int gemsBalance,  int keysBalance)
        {
            _coinsBalance = coinsBalance;
            _gemsBalance = gemsBalance;
            _keysBalance = keysBalance;
            _isInitialized = true;
        }

        public void AddCoins(int coinsToAdd)
        {
            Assert.IsTrue(_isInitialized);
            var newBalance = _coinsBalance + coinsToAdd;
            SetCoinsBalance(newBalance);
        }

        public void WithdrawCoins(int coinsToTake)
        {
            Assert.IsTrue(_isInitialized);
            var newBalance = _coinsBalance - coinsToTake;
            SetCoinsBalance(newBalance);
        }
        
        public void AddKeys(int coinsToAdd)
        {
            Assert.IsTrue(_isInitialized);
            var newBalance = _keysBalance + coinsToAdd;
            SetKeysBalance(newBalance);
        }

        public void WithdrawKeys(int coinsToTake)
        {
            Assert.IsTrue(_isInitialized);
            var newBalance = _keysBalance - coinsToTake;
            SetKeysBalance(newBalance);
        }
        
        public void AddGems(int gemsToAdd)
        {
            Assert.IsTrue(_isInitialized);
            var newBalance = _gemsBalance + gemsToAdd;
            SetGemsBalance(newBalance);
        }

        public void WithdrawGems(int gemsToTake)
        {
            Assert.IsTrue(_isInitialized);
            var newBalance = _gemsBalance - gemsToTake;
            SetGemsBalance(newBalance);
        }

        private void SetGemsBalance(int newGemsBalance)
        {
            var oldBalance = _gemsBalance;
            _gemsBalance = Mathf.Max(0, newGemsBalance);
            if (oldBalance != _gemsBalance)
            {
                GemsBalanceChange?.Invoke(oldBalance, _gemsBalance);
            }
        }

        private void SetCoinsBalance(int newCoinsBalance)
        {
            var oldBalance = _coinsBalance;
            _coinsBalance = Mathf.Max(0, newCoinsBalance);
            if (oldBalance != _coinsBalance)
            {
                CoinsBalanceChange?.Invoke(oldBalance, _coinsBalance);
            }
        }
        
        private void SetKeysBalance(int newCoinsBalance)
        {
            var oldBalance = _coinsBalance;
            _keysBalance = Mathf.Max(0, newCoinsBalance);
            if (oldBalance != _keysBalance)
            {
                KeysBalanceChange?.Invoke(oldBalance, _keysBalance);
            }
        }
        
        #endregion
        
    }
}