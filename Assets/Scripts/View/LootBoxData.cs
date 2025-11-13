using System;
using UnityEngine;
using UnityEngine.UI;
using Whalo.UI;

namespace Whalo.View
{
    [Serializable]
    public class LootBoxData
    {
        #region Private Members

        [SerializeField] private Transform _lootBoxDoor;
        [SerializeField] private Transform  _prizeContainer;
        [SerializeField] private CounterView  _counterView;
        [SerializeField] private Button _button;

        #endregion

        #region Properties
        
        public Transform LootBoxDoor => _lootBoxDoor;
        public Transform PrizeContainer => _prizeContainer;
        public Button Button => _button;

        #endregion
        
        #region Methods

        public void SetContainerView(CounterView counterView)
        {
            _counterView = counterView;
        }

        #endregion
    }
}