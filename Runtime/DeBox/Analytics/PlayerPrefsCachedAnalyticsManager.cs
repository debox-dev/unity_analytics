using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using DeBox.PlayerPrefsExtensions;

namespace DeBox.Analytics
{
    /// <summary>
    /// A proxy IAnalyticsManager that caches all the analytics operations to the local Unity PlayerPrefs
    /// so the events and data will persist across crashes and restarts of the app
    ///
    /// <example>
    /// Example:
    /// <code>
    /// var concreteAnalyticsManager = new DebugAnalyticsManager();
    /// var cachedAnalyticsManager = new PlayerPrefsCachedAnalyticsManager("myKeyPrefix", concreteAnalyticsManager);
    ///
    /// cachedAnalyticsManager.TrackEvent("myEvent"); // Will cache myEvent
    /// cachedAnalyticsManager.Flush(); // Will send all events and data to `concreteAnalyticsManager` and Flush it.
    /// 
    /// </code>
    /// </example>
    /// </summary>
    public class PlayerPrefsCachedAnalyticsManager : IAnalyticsManager
    {
        private const string SPECIAL_NAME_ATTR_NAME = "__name__";
        private readonly string _name;

        private readonly PlayerPrefsString _incrementDataStore;
        private readonly PlayerPrefsString _setDataStore;
        private readonly PlayerPrefsString _setOnceDataStore;
        private readonly PlayerPrefsString _addToListDataStore;
        private readonly PlayerPrefsString _unionListDataStore;
        private readonly PlayerPrefsString _intersectListDataStore;
        private readonly PlayerPrefsString _removeFromListDataStore;
        private readonly PlayerPrefsQueue<PlayerPrefsString, string> _eventQueue;
        private readonly PlayerPrefsQueue<PlayerPrefsString, string> _revenueQueue;
        private readonly Dictionary<string, object> _incrementData;
        private readonly Dictionary<string, object> _setData;
        private readonly Dictionary<string, object> _setOnceData;
        private readonly Dictionary<string, object> _addToListData;
        private readonly Dictionary<string, object> _unionListData;
        private readonly Dictionary<string, object> _intersectListData;
        private readonly Dictionary<string, object> _removeFromListData;
        
        private readonly IAnalyticsManager _analyticsManager;
        
        /// <summary>
        /// Create a new PlayerPrefsCachedAnalyticsManager that caches for the given IAnalyticsManager
        /// </summary>
        /// <param name="name">PlayerPrefs key prefix for all the data of this manager</param>
        /// <param name="analyticsManager">The proxied IAnalyticsManager implementation</param>
        public PlayerPrefsCachedAnalyticsManager(string name, IAnalyticsManager analyticsManager)
        {
            _name = name;
            _eventQueue = new PlayerPrefsQueue<PlayerPrefsString, string>(name + ":eventQueue", 10000000);
            _revenueQueue = new PlayerPrefsQueue<PlayerPrefsString, string>(name + ":eventQueue", 10000000);
            _incrementDataStore = new PlayerPrefsString(name + ":increment", "{}");
            _setDataStore = new PlayerPrefsString(name + ":set", "{}");
            _setOnceDataStore = new PlayerPrefsString(name + ":set_once", "{}");
            _addToListDataStore = new PlayerPrefsString(name + ":add_to_list", "{}");
            _unionListDataStore = new PlayerPrefsString(name + ":union_list", "{}");
            _intersectListDataStore = new PlayerPrefsString(name + ":intersect_list", "{}");
            _removeFromListDataStore = new PlayerPrefsString(name + ":remove_from_list", "{}");
            _incrementData = LoadData(_incrementDataStore);
            _setData = LoadData(_setDataStore);
            _setOnceData = LoadData(_setOnceDataStore);
            _addToListData = LoadData(_addToListDataStore);
            _unionListData = LoadData(_unionListDataStore);
            _intersectListData = LoadData(_incrementDataStore);
            _removeFromListData = LoadData(_removeFromListDataStore);
            _analyticsManager = analyticsManager;
        }

        /// <summary>
        /// Perform Initialize on the contained manager
        /// </summary>
        public void Initialize()
        {
            _analyticsManager.Initialize();
        }

        /// <summary>
        /// Perform Identify on the contained manager
        /// </summary>
        public void Identify(string id)
        {
            _analyticsManager.Identify(id);
        }

        /// <summary>
        /// Cache an Increment operation. The operation will be sent on Flush()
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public void Increment(string attribute, int value)
        {
            if (!_incrementData.TryGetValue(attribute, out var existingObj))
            {
                _incrementData[attribute] = value;
                return;
            }

            _incrementData[attribute] = (int) existingObj + value;
            StoreData(_incrementData, _incrementDataStore);
        }

        /// <summary>
        /// Cache an Increment operation. The operation will be sent on Flush()
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public void Increment(string attribute, float value)
        {
            if (!_incrementData.TryGetValue(attribute, out var existingObj))
            {
                _incrementData[attribute] = value;
                return;
            }

            _incrementData[attribute] = (float) existingObj + value;
            StoreData(_incrementData, _incrementDataStore);
        }

        /// <summary>
        /// Cache an increment operation. The operation will be sent on Flush()
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public void Set(string attribute, object value)
        {
            _setData[attribute] = value;
            StoreData(_setData, _setDataStore);
        }

        /// <summary>
        /// Cache an increment operation. The operation will be sent on Flush()
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public void SetOnce(string attribute, object value)
        {
            if (_setOnceData.ContainsKey(attribute)) return;
            _setOnceData[attribute] = value;
            StoreData(_setOnceData, _setOnceDataStore);
        }

        /// <summary>
        /// Cache an increment operation. The operation will be sent on Flush()
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="values"></param>
        public void AddToList(string attribute, params object[] values)
        {
            List<object> list = GetOrCreateList(_addToListData, attribute);
            foreach (var value in values)
            {
                list.Add(value);
            }

            StoreData(_addToListData, _addToListDataStore);
        }

        /// <summary>
        /// Cache an increment operation. The operation will be sent on Flush()
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="values"></param>
        public void UnionList(string attribute, params object[] values)
        {
            List<object> list = GetOrCreateList(_unionListData, attribute);
            foreach (var value in values)
            {
                if (!list.Contains(value))
                {
                    list.Add(value);
                }
            }

            StoreData(_unionListData, _unionListDataStore);
        }

        /// <summary>
        /// Cache an increment operation. The operation will be sent on Flush()
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="values"></param>
        public void IntersectList(string attribute, params object[] values)
        {
            List<object> list = GetOrCreateList(_intersectListData, attribute);
            foreach (var value in values)
            {
                if (!list.Contains(value))
                {
                    list.Add(value);
                }
            }

            StoreData(_intersectListData, _intersectListDataStore);
        }

        /// <summary>
        /// Cache an increment operation. The operation will be sent on Flush()
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="values"></param>
        public void RemoveFromList(string attribute, params object[] values)
        {
            List<object> list = GetOrCreateList(_removeFromListData, attribute);
            foreach (var value in values)
            {
                if (!list.Contains(value))
                {
                    list.Add(value);
                }
            }

            StoreData(_removeFromListData, _removeFromListDataStore);
        }

        /// <summary>
        /// Cache an increment operation. The operation will be sent on Flush()
        /// </summary>
        /// <param name="eventName"></param>
        public void TrackEvent(string eventName)
        {
            PushEvent(eventName, null);
        }

        /// <summary>
        /// Cache an increment operation. The operation will be sent on Flush()
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="attributes"></param>
        public void TrackEvent(string eventName, Dictionary<string, object> attributes)
        {
            PushEvent(eventName, attributes);
        }

        /// <summary>
        /// Perform SetTrackingAttribute on the contained manager
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="value"></param>
        public void SetTrackingAttribute(string attributeName, object value)
        {
            _analyticsManager.SetTrackingAttribute(attributeName, value);
        }

        /// <summary>
        /// Perform SetTrackingAttributeOnce on the contained manager
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="value"></param>
        public void SetTrackingAttributeOnce(string attributeName, object value)
        {
            _analyticsManager.SetTrackingAttributeOnce(attributeName, value);
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        /// <param name="price"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void TrackRevenue(string product, int quantity, float price)
        {
            PushRevenue(product, quantity, price);
        }

        /// <summary>
        /// Flush all the cached operations to the contained manager, then flush the contained manager
        /// </summary>
        public void Flush()
        {
            FlushEvents();
            FlushIncrement();
            FlushSet();
            FlushSetOnce();
            FlushAddToList();
            FlushRemoveFromList();
            FlushUnionList();
            FlushIntersectList();
            FlushRevenue();
            _analyticsManager.Flush();
        }

        /// <summary>
        /// Returns SupportsFeature of the contained manager
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public bool SupportsFeature(AnalyticsFeatureType feature)
        {
            return _analyticsManager.SupportsFeature(feature);
        }

        private void FlushData(Dictionary<string, object> data, Action<string, object> flushAction, PlayerPrefsString store)
        {
            foreach (var p in data)
            {
                flushAction(p.Key, p.Value);
            }
            data.Clear();
            StoreData(data, store);
        }

        private void FlushDataList(Dictionary<string, object> data, Action<string, object[]> flushAction,
            PlayerPrefsString store)
        {
            FlushData(data,
                (s, o) => flushAction(s, ((List<object>)o).ToArray()), 
                store);
        }
        
        private void FlushSetOnce() => FlushData(_setOnceData, _analyticsManager.SetOnce, _setOnceDataStore);

        private void FlushSet() => FlushData(_setData, _analyticsManager.Set, _setDataStore);

        private void FlushAddToList() =>
            FlushDataList(_addToListData, _analyticsManager.AddToList, _addToListDataStore);

        private void FlushRemoveFromList() =>
            FlushDataList(_removeFromListData, _analyticsManager.RemoveFromList, _removeFromListDataStore);

        private void FlushUnionList() =>
            FlushDataList(_unionListData, _analyticsManager.UnionList, _unionListDataStore);

        private void FlushIntersectList() =>
            FlushDataList(_intersectListData, _analyticsManager.IntersectList, _intersectListDataStore);
        
        private void FlushIncrement()
        {
            FlushData(_setData, (n, v) =>
            {
                if (v is float)
                {
                    _analyticsManager.Increment(n, (float)v);                    
                }

                if (v is int)
                {
                    _analyticsManager.Increment(n, (int)v);
                }
            }, _setDataStore);
        }

        private void FlushEvents()
        {
            while (_eventQueue.Count > 0)
            {
                PopEvent(out var eventName, out var attributes);
                if (attributes == null)
                {
                    _analyticsManager.TrackEvent(eventName);                    
                }
                else
                {
                    _analyticsManager.TrackEvent(eventName, attributes);
                }
            }
        }
        
        private void FlushRevenue()
        {
            while (_revenueQueue.Count > 0)
            {
                PopRevenue(out var productName, out var quantity, out var price);
                _analyticsManager.TrackRevenue(productName, quantity, price);
            }
        }

        private void PushRevenue(string productName, int quantity, float price)
        {
            var data = new Dictionary<string, object>
            {
                ["productName"] = productName, ["quantity"] = quantity, ["price"] = price
            };
            var json = JsonConvert.SerializeObject(data);
            _revenueQueue.Enqueue(json);
        }

        private void PopRevenue(out string productName, out int quantity, out float price)
        {
            var json = _eventQueue.Dequeue();
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            productName = (string) (data["productName"]);
            quantity = (int)data["quantity"];
            price = (int)data["price"];
        }
        
        private void PushEvent(string eventName, Dictionary<string, object> attributes)
        {
            attributes[SPECIAL_NAME_ATTR_NAME] = eventName;
            var eventJson = JsonConvert.SerializeObject(attributes);
            _eventQueue.Enqueue(eventJson);
        }

        private void PopEvent(out string eventName, out Dictionary<string, object> attributes)
        {
            var eventJson = _eventQueue.Dequeue();
            var eventData = JsonConvert.DeserializeObject<Dictionary<string, object>>(eventJson);
            eventName = (string) (eventData[SPECIAL_NAME_ATTR_NAME]);
            eventData.Remove(SPECIAL_NAME_ATTR_NAME);
            attributes = eventData;
        }

        private List<object> GetOrCreateList(Dictionary<string, object> data, string listName)
        {
            List<object> list;
            if (!data.TryGetValue(listName, out var existingObj))
            {
                list = new List<object>();
                data[listName] = list;
            }
            else
            {
                list = (List<object>) existingObj;
            }

            return list;
        }

        private Dictionary<string, object> LoadData(PlayerPrefsString dataStore)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(dataStore.Value);
        }

        private void StoreData(Dictionary<string, object> data, PlayerPrefsString dataStore)
        {
            dataStore.Value = JsonConvert.SerializeObject(data);
        }
    }
}