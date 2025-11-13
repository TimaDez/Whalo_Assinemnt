using System;
using System.Collections.Generic;
using DataTypes;
using UnityEngine;
using UnityEngine.Assertions;

namespace Models
{
    [CreateAssetMenu(menuName = "Models/Player Model", fileName = "Player Model")]
    public class PlayerModel : ScriptableObject
    {
        /// <summary>
        /// Can be used for local tests
        /// </summary>
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

        #region Properties

        public int CoinsBalance => _coinsBalance;
        public int GemsBalance => _gemsBalance;
        public int KeysBalance => _keysBalance;

        #endregion

        #region Private Fields

        private Dictionary<PrizeType, int> _balances;

        #endregion
        
        #region Methods

        public void Initialize(int coinsBalance, int gemsBalance,  int keysBalance)
        {
            _coinsBalance = coinsBalance;
            _gemsBalance = gemsBalance;
            _keysBalance = keysBalance;

            _balances = new Dictionary<PrizeType, int>
            {
                { PrizeType.Key, _keysBalance },
                { PrizeType.Coins, _coinsBalance },
                { PrizeType.Gems, _gemsBalance },
            };
        }

        public int GetBalance(PrizeType prizeType)
        {
            if(_balances.TryGetValue(prizeType, out var balance))
                return balance;

            Debug.LogError($"[PlayerModel] GetBalance() No data for type: {prizeType}");
            return 0;
        }
        
        public void AddPrize(PrizeType prizeType, int amount)
        {
            Debug.Log($"[BoxingLootController] OnButtonClicked() type: {prizeType}, amount: {amount}");
            switch (prizeType)
            {
                case PrizeType.Key:
                    AddKeys(amount);
                    break;
                case PrizeType.Gems:
                    AddGems(amount);
                    break;
                case PrizeType.Coins:
                    AddCoins(amount);
                    break;
                default:
                    Debug.LogError($"[PlayerModel] AddPrize() Not prize with Type: {prizeType}");
                    break;
            }
        }
        
        public void AddCoins(int coinsToAdd)
        {
            var newBalance = _coinsBalance + coinsToAdd;
            SetCoinsBalance(newBalance);
        }
        
        public void AddKeys(int coinsToAdd)
        {
            var newBalance = _keysBalance + coinsToAdd;
            SetKeysBalance(newBalance);
        }

        public void WithdrawKeys(int coinsToTake)
        {
            var newBalance = _keysBalance - coinsToTake;
            SetKeysBalance(newBalance);
        }
        
        public void AddGems(int gemsToAdd)
        {
            var newBalance = _gemsBalance + gemsToAdd;
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