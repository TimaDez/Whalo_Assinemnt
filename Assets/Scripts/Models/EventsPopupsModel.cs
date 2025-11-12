using System;
using System.Collections.Generic;
using UnityEngine;
using Whalo.UI;

namespace Models
{
    [Serializable]
    public class EventsPopupData
    {
        public string Name;
        public string Url;
        public int Order;
        public EventPopupBase EventPopupPrefab;
    }
    
    [CreateAssetMenu(menuName = "Models/Events Popups Model",  fileName = "EventsPopupsModel")]
    public class EventsPopupsModel : ScriptableObject
    {
        #region Editor

        [SerializeField] public EventsPopupData[] _popups;
        
        #endregion

        #region Private Fields

        private Dictionary<string, EventsPopupData> _popupsDictionary;

        #endregion
        
        #region Methods

        public List<EventsPopupData> GetOrderedPopupsData()
        {
            ValidatePopupsData();
            var allPopups = new List<EventsPopupData>(_popupsDictionary.Values);
            allPopups.Sort((a, b) => b.Order.CompareTo(a.Order));
            
            return allPopups;
        }

        public EventsPopupData GetPopupData(string url)
        {
            ValidatePopupsData();
            if (_popupsDictionary.TryGetValue(url, out var popupData))
                return popupData;
            
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