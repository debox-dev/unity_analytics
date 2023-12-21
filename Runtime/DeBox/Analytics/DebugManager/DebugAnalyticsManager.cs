using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DeBox.PlayerPrefsExtensions;
using Newtonsoft.Json;

namespace DeBox.Analytics.DebugManager
{
    /// <summary>
    /// A local analytics manager that sends data and events to the log for debugging
    ///
    /// DebugAnalyticsManager saves tracking data in the Unity PlayerPrefs 
    /// </summary>
    public class DebugAnalyticsManager : BaseAnalyticsManager
    {
        private PlayerPrefsString _playerDataStore; 
        private Dictionary<string, object> _playerData;
        private Dictionary<string, object> _superProps;
        
        /// <summary>
        /// Tracked account id
        /// </summary>
        public string AccountId { get; private set; }
        
        /// <summary>
        /// Create a new DebugAnalyticsManager
        /// </summary>
        public DebugAnalyticsManager() : base(BaseAnalyticsManager.AutoTimestampModeType.AutomaticallyTimestamp) {}
        
        /// <summary>
        /// Initialize this manager
        /// </summary>
        public override void Initialize()
        {
            _superProps = new Dictionary<string, object>();
        }

        /// <summary>
        /// Set the tracking id. This will create a data store in the PlayerPrefs for the tracked data
        /// of this account. Different account ids will resolve to different PlayerPrefs keys
        /// </summary>
        /// <param name="id">Id of the account</param>
        public override  void Identify(string id)
        {
            Debug.Log("DebugAnalyticsManager: Identifying as " + id);
            AccountId = id;
            _playerDataStore = new PlayerPrefsString("DeBox.Analytics.DebugManager,id=" + id, "{}");
            LoadPlayerData();
        }

        /// <summary>
        /// See IAnalytics Manager
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public override  void Increment(string attribute, int value)
        {
            int newVal = value;
            if (_playerData.TryGetValue(attribute, out var existing))
            {
                newVal = (int)(((System.Int64) existing) + value);
            }
            _playerData[attribute] = newVal;
            StorePlayerData();
            Debug.Log("DebugAnalyticsManager: INCREMENT: " + attribute + ": " + DictToString(_playerData));
        }

        /// <summary>
        /// See IAnalytics Manager
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public override  void Increment(string attribute, float value)
        {
            float newVal = value;
            if (_playerData.TryGetValue(attribute, out var existing))
            {
                newVal = (float) existing + value;
            }
            _playerData[attribute] = newVal;
            StorePlayerData();
            Debug.Log("DebugAnalyticsManager: INCREMENT: " + attribute + ": " + DictToString(_playerData));
        }

        /// <summary>
        /// See IAnalytics Manager
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public override  void Set(string attribute, object value)
        {
            _playerData[attribute] = value;
            Debug.Log("DebugAnalyticsManager: SET: "  + attribute + ": "+ DictToString(_playerData));
        }

        /// <summary>
        /// See IAnalytics Manager
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public override  void SetOnce(string attribute, object value)
        {
            Debug.Log("DebugAnalyticsManager: SET ONCE: "  + attribute + ": "+ DictToString(_playerData));
            if (_playerData.ContainsKey(attribute))
            {
                return;
            }
            Set(attribute, value);
        }

        /// <summary>
        /// See IAnalytics Manager
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="values"></param>
        public override  void AddToList(string attribute, params object[] values)
        {
            Debug.Log("DebugAnalyticsManager: ADD TO LIST: "  + attribute + ": "+ DictToString(_playerData));
            List<object> data = new List<object>(values);
            if (_playerData.TryGetValue(attribute, out var existing))
            {
                data.AddRange((List<object>)existing);
            }
            _playerData[attribute] = data;
            StorePlayerData();
        }

        /// <summary>
        /// See IAnalytics Manager
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="values"></param>
        public override  void UnionList(string attribute, params object[] values)
        {
            if (_playerData.TryGetValue(attribute, out var existing))
            {
                var existingList = (List<object>) existing;
                foreach (var val in values)
                {
                    if (!existingList.Contains(val)) existingList.Add(val);
                }
            }
            StorePlayerData();
        }

        /// <summary>
        /// See IAnalytics Manager
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="values"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override  void IntersectList(string attribute, params object[] values)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// See IAnalytics Manager
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="values"></param>
        public override  void RemoveFromList(string attribute, params object[] values)
        {
            if (_playerData.TryGetValue(attribute, out var existing))
            {
                var existingList = (List<object>) existing;
                foreach (var val in values)
                {
                    if (existingList.Contains(val)) existingList.Remove(val);
                }
            }
            StorePlayerData();
        }

        /// <summary>
        /// See IAnalytics Manager
        /// </summary>
        /// <param name="eventName"></param>
        protected override  void TrackEventInternal(string eventName)
        {
            TrackEvent(eventName, null);
        }

        /// <summary>
        /// See IAnalytics Manager
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="attributes"></param>
        protected override  void TrackEventInternal(string eventName, Dictionary<string, object> attributes)
        {
            var propsDict = new Dictionary<string, object>();
            if (attributes != null)
            {
                foreach (var p in attributes)
                {
                    propsDict[p.Key] = p.Value;
                }
            }
            foreach (var p in _superProps)
            {
                propsDict[p.Key] = p.Value;
            }
            var propsString = DictToString(propsDict);
            Debug.Log("DebugAnalyticsManager: EVENT: " + eventName  + " PROPS:" + propsString);
        }

        /// <summary>
        /// See IAnalytics Manager
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="value"></param>
        public override  void SetTrackingAttribute(string attributeName, object value)
        {
            _superProps[attributeName] = value;
        }

        /// <summary>
        /// Tracks an event to the log console
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="value"></param>
        public override  void SetTrackingAttributeOnce(string attributeName, object value)
        {
            if (_superProps.ContainsKey(attributeName)) return;
            SetTrackingAttribute(attributeName, value);
        }

        /// <summary>
        /// Tracks an event to the log console
        /// </summary>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        /// <param name="price"></param>
        public override  void TrackRevenue(string product, int quantity, float price)
        {
            Debug.LogError("DebugAnalyticsManager: TRACK REVENUE: " + product + ": " + price);
        }

        /// <summary>
        /// Does nothing except logging that a flush was requested.
        /// The message in the console will include all the player tracking data
        /// </summary>
        public override  void Flush()
        {
            Debug.Log("DebugAnalyticsManager: Flush! " + DictToString(_playerData));
        }

        private string DictToString(Dictionary<string, object> dict)
        {
            var result = "";
            if (dict == null)
            {
                return result;
            }
            foreach (var p in dict)
            {
                result += p.Key + "=" + p.Value + ", ";
            }

            if (result.Length > 2)
            {
                result = result.Substring(0, result.Length - 2);                
            }

            return result;
        }

        private void LoadPlayerData()
        {
            var data = _playerDataStore.Value;
            _playerData = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
            var keys = _playerData.Keys.ToArray();
            foreach (var k in keys)
            {
                var v = _playerData[k];
                if (v.GetType() == typeof(Newtonsoft.Json.Linq.JArray))
                {
                    v = ((Newtonsoft.Json.Linq.JArray)v).ToObject<List<object>>();
                    _playerData[k] = v;
                }
            }
        }

        private void StorePlayerData()
        {
            _playerDataStore.Value = JsonConvert.SerializeObject(_playerData);
        }

        /// <summary>
        /// See BaseAnalyticsManager
        /// </summary>
        /// <returns></returns>
        protected override AnalyticsFeatureType GetSupportedFeatures()
        {
            return AnalyticsFeatureType.AccountIncrement | AnalyticsFeatureType.AccountSet |
                   AnalyticsFeatureType.AccountTracking | AnalyticsFeatureType.NamedEvents |
                   AnalyticsFeatureType.RevenueTracking |
                   AnalyticsFeatureType.AccountSetOnce | AnalyticsFeatureType.AccountUnionList |
                   AnalyticsFeatureType.AccountAddToList | AnalyticsFeatureType.AccountRemoveFromList |
                   AnalyticsFeatureType.SetGlobalEventAttribute | AnalyticsFeatureType.NamedEventsWithNamedAttributes |
                   AnalyticsFeatureType.SetGlobalEventAttributeOnce;
        }
    }
    
}
