using System;
using System.Collections.Generic;
using DataTypes;
using UnityEngine;

namespace Models
{
    public class PlayerModelSingleton : MonoBehaviour
    {
        #region Singleton
        public static PlayerModelSingleton Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadFromPrefs();
        }
        #endregion

        #region Events
        public event Action<int, int> CoinsBalanceChange;
        public event Action<int, int> GemsBalanceChange;
        public event Action<int, int> KeysBalanceChange;
        #endregion

        #region Private Fields
        private int _coinsBalance;
        private int _gemsBalance;
        private int _keysBalance;
        private Dictionary<PrizeType, int> _balances;
        #endregion

        #region Properties
        public int CoinsBalance => _coinsBalance;
        public int GemsBalance => _gemsBalance;
        public int KeysBalance => _keysBalance;
        #endregion

        #region Constants
        private const string COINS_KEY = "Player_Coins";
        private const string GEMS_KEY  = "Player_Gems";
        private const string KEYS_KEY  = "Player_Keys";
        #endregion

        #region Public Methods
        public void Initialize(int coins, int gems, int keys)
        {
            _coinsBalance = coins;
            _gemsBalance = gems;
            _keysBalance = keys;

            _balances = new Dictionary<PrizeType, int>
            {
                { PrizeType.Coins, _coinsBalance },
                { PrizeType.Gems,  _gemsBalance },
                { PrizeType.Key,   _keysBalance },
            };

            SaveToPrefs();
        }

        public int GetBalance(PrizeType prizeType)
        {
            if (_balances != null && _balances.TryGetValue(prizeType, out var balance))
                return balance;

            Debug.LogError($"[PlayerModel] No balance found for {prizeType}");
            return 0;
        }

        public void AddPrize(PrizeType prizeType, int amount)
        {
            switch (prizeType)
            {
                case PrizeType.Key:   AddKeys(amount);  break;
                case PrizeType.Gems:  AddGems(amount);  break;
                case PrizeType.Coins: AddCoins(amount); break;
            }
        }

        public void AddCoins(int coinsToAdd) => SetCoinsBalance(_coinsBalance + coinsToAdd);
        public void AddKeys(int keysToAdd)   => SetKeysBalance(_keysBalance + keysToAdd);
        public void WithdrawKeys(int keys)   => SetKeysBalance(_keysBalance - keys);
        public void AddGems(int gemsToAdd)   => SetGemsBalance(_gemsBalance + gemsToAdd);
        #endregion

        #region Private Setters
        private void SetCoinsBalance(int newValue)
        {
            var old = _coinsBalance;
            _coinsBalance = Mathf.Max(0, newValue);
            if (old != _coinsBalance)
            {
                _balances[PrizeType.Coins] = _coinsBalance;
                CoinsBalanceChange?.Invoke(old, _coinsBalance);
                SaveToPrefs();
            }
        }

        private void SetKeysBalance(int newValue)
        {
            var old = _keysBalance;
            _keysBalance = Mathf.Max(0, newValue);
            if (old != _keysBalance)
            {
                _balances[PrizeType.Key] = _keysBalance;
                KeysBalanceChange?.Invoke(old, _keysBalance);
                SaveToPrefs();
            }
        }

        private void SetGemsBalance(int newValue)
        {
            var old = _gemsBalance;
            _gemsBalance = Mathf.Max(0, newValue);
            if (old != _gemsBalance)
            {
                _balances[PrizeType.Gems] = _gemsBalance;
                GemsBalanceChange?.Invoke(old, _gemsBalance);
                SaveToPrefs();
            }
        }
        #endregion

        #region Persistence
        private void SaveToPrefs()
        {
            PlayerPrefs.SetInt(COINS_KEY, _coinsBalance);
            PlayerPrefs.SetInt(GEMS_KEY, _gemsBalance);
            PlayerPrefs.SetInt(KEYS_KEY, _keysBalance);
            PlayerPrefs.Save();
        }

        private void LoadFromPrefs()
        {
            _coinsBalance = PlayerPrefs.GetInt(COINS_KEY, 0);
            _gemsBalance  = PlayerPrefs.GetInt(GEMS_KEY, 0);
            _keysBalance  = PlayerPrefs.GetInt(KEYS_KEY, 0);

            _balances = new Dictionary<PrizeType, int>
            {
                { PrizeType.Coins, _coinsBalance },
                { PrizeType.Gems,  _gemsBalance },
                { PrizeType.Key,   _keysBalance },
            };
        }

        public void ResetPrefs()
        {
            PlayerPrefs.DeleteKey(COINS_KEY);
            PlayerPrefs.DeleteKey(GEMS_KEY);
            PlayerPrefs.DeleteKey(KEYS_KEY);
            PlayerPrefs.Save();
        }
        #endregion
    }
}
