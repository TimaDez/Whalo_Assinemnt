using UnityEngine;

namespace Models
{
    [CreateAssetMenu(menuName = "Models/Levels",  fileName = "LevelModel")]
    public class LevelModel : ScriptableObject
    {
        #region Editor

        [SerializeField] private PrizeModel[]  _prizes;

        #endregion

        #region Properties

        public PrizeModel[] Prizes => _prizes;

        #endregion
    }
}