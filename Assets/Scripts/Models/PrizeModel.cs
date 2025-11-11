using System;
using DataTypes;
using UI;
using UnityEngine;

namespace Models
{
    [CreateAssetMenu(menuName = "Models/Prize", fileName = "PrizeModel")]
    public class PrizeModel : ScriptableObject
    {
        #region Editor

        [SerializeField] private PrizeType _type;
        [SerializeField] private int _amount;
        [SerializeField] private PrizeView _prizeViewPrefab;

        #endregion

        #region Properties

        public PrizeType Type => _type;
        public PrizeView PrizeViewPrefab => _prizeViewPrefab;
        public int Amount => _amount;

        #endregion

    }
}