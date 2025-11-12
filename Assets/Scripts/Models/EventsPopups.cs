using System;
using System.Collections.Generic;
using UnityEngine;
using Whalo.UI;

namespace Models
{
    [Serializable]
    public class EventsPopupData
    {
        public string Url;
        public EventPopupBase EventPopupPrefab;
    }
    
    [CreateAssetMenu(menuName = "Models/EventsPopups",  fileName = "EventsPopups")]
    public class EventsPopups : ScriptableObject
    {
        #region Editor

        [SerializeField] public EventsPopupData[] _popups;
        
        #endregion

        #region Private Fields

        private Dictionary<string, EventsPopupData> _popupsDictionary;

        #endregion
        
        #region Methods

        public EventsPopupData GetPopupData(string url)
        {
            ValidatePopupsData();
            if (_popupsDictionary.TryGetValue(url, out var data))
                return data;

            Debug.LogError($"[EventsPopups] GetPopupData() Didn't find popup for URL: {url}");
            return null;
        }
        
        private void ValidatePopupsData()
        {
            if(_popupsDictionary != null)
                return;

            _popupsDictionary = new Dictionary<string, EventsPopupData>();
            foreach (var popup in _popups)
            {
                _popupsDictionary.Add(popup.Url, popup);
            }
        }

        #endregion
    }
}