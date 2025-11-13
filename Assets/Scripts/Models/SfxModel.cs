using System;
using System.Collections.Generic;
using DataTypes;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class SfxData
    {
        public SfxType type;
        public AudioClip clip;
    }
    
    [CreateAssetMenu(menuName = "Models/SfxModel",  fileName = "SfxModel")]
    public class SfxModel : ScriptableObject
    {
        #region Editor

        [SerializeField] private SfxData[] _sfxData;

        #endregion

        #region Private Fields

        private Dictionary<SfxType, AudioClip> _clipsMap;

        #endregion

        #region Methods

        public AudioClip GetClip(SfxType type)
        {
            ValidateClipsMap();
            if(_clipsMap.TryGetValue(type, out var clip))
                return  clip;

            Debug.LogError($"[SfxModel] GetClip() No clip for Type: {type}");
            return null;
        }
        
        private void ValidateClipsMap()
        {
            if(_clipsMap != null)
                return;

            _clipsMap = new Dictionary<SfxType, AudioClip>();
            foreach (var data in _sfxData)
            {
                _clipsMap[data.type] = data.clip;
            }
        }

        #endregion
    }
}