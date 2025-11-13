using UnityEngine;
using Whalo.Models;

namespace Models
{
    [CreateAssetMenu(menuName = "Models/Levels",  fileName = "LevelModel")]
    public class LevelModel : ScriptableObject
    {
        #region Editor

        [SerializeField] private int _keyStarterAmount;
        [SerializeField] private PrizeModel[]  _prizes;

        #endregion

        #region Private Members

        private int _currentKeyAmount;

        #endregion

        #region Properties

        public PrizeModel[] Prizes => _prizes;
        public int KeyStarterAmount => _keyStarterAmount;
        public int CurrentKeyAmount => _currentKeyAmount;

        #endregion

        #region Methods

        private void OnEnable()
        {
            _currentKeyAmount =  _keyStarterAmount;
        }
        
        public void RemoveKey()
        {
            _currentKeyAmount = Mathf.Max(0, --_currentKeyAmount);
        }

        public void AddKey(int amount)
        {
            _currentKeyAmount += amount;
        }
        
        #endregion
    }
}