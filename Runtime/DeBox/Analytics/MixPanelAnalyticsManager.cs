using System;
using System.Linq;
using System.Collections.Generic;

#if DEBOX_ANALYTICS_MIXPANEL
using mixpanel;


namespace DeBox.Analytics.MixPanelManager
{
    /// <summary>
    /// IAnalyticsManager that uses the mixpanel SDK
    ///
    /// Enable this manager by defininig the 'DEBOX_ANALYTICS_MIXPANEL' pre-processor define 
    /// </summary>
    public class MixPanelAnalyticsManager : BaseAnalyticsManager
    {
        private const string MIXPANEL_EVENT_TIME_ATTRIBUTE_NAME = "time";
        
        /// <summary>
        /// Create a new MixPanelAnalyticsManager
        /// </summary>
        /// <param name="timestampModeType">See BaseAnalyticsManager</param>
        public MixPanelAnalyticsManager(AutoTimestampModeType timestampModeType) : base(timestampModeType)
        {
        }
        
        /// <summary>
        /// Does nothing
        /// </summary>
        public  override void Initialize()
        {
        }

        /// <summary>
        /// Activate account tracking
        /// </summary>
        /// <param name="id">account id</param>
        public override  void Identify(string id)
        {
            Mixpanel.Identify(id);
        }

        /// <summary>
        /// Increment attribute value in MixPanel People
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public override  void Increment(string attribute, int value)
        {
            Mixpanel.People.Increment(attribute, value);
        }

        /// <summary>
        /// Increment attribute value in MixPanel People
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public override  void Increment(string attribute, float value)
        {
            Mixpanel.People.Increment(attribute, value);
        }

        /// <summary>
        /// Set attribute value in MixPanel People
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public override  void Set(string attribute, object value)
        {
            Mixpanel.People.Set(attribute, ObjectToValue(value));
        }

        /// <summary>
        /// Set attribute value once in MixPanel People
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public override  void SetOnce(string attribute, object value)
        {
            Mixpanel.People.SetOnce(attribute, ObjectToValue(value));
        }

        /// <summary>
        /// Append list attribute values in MixPanel People
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="values"></param>
        public override  void AddToList(string attribute, params object[] values)
        {
            foreach (var valueObj in values)
            {
                Mixpanel.People.Append(attribute, ObjectToValue(valueObj));    
            }
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="values"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override  void UnionList(string attribute, params object[] values)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="values"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override  void IntersectList(string attribute, params object[] values)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="values"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override  void RemoveFromList(string attribute, params object[] values)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Track an event
        /// </summary>
        /// <param name="eventName"></param>
        protected override void TrackEventInternal(string eventName)
        {
            Mixpanel.Track(eventName);
        }
        
        /// <summary>
        /// Track an event with attributes
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="attributes"></param>
        protected override void TrackEventInternal(string eventName, Dictionary<string, object> attributes)
        {
            Mixpanel.Track(eventName, DictionaryToMixPanelValue(attributes));
        }

        /// <summary>
        /// Set a Super Property for tracked events
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="value"></param>
        public override void SetTrackingAttribute(string attributeName, object value)
        {
            Mixpanel.Register(attributeName, ObjectToValue(value));
        }

        /// <summary>
        /// Set a Super Property once for tracked events
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="value"></param>
        public override void SetTrackingAttributeOnce(string attributeName, object value)
        {
            Mixpanel.RegisterOnce(attributeName, ObjectToValue(value));
        }

        /// <summary>
        /// Not supported yet
        /// </summary>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        /// <param name="price"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void TrackRevenue(string product, int quantity, float price)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Call Mixpanel.Flush()
        /// </summary>
        public override void Flush()
        {
            Mixpanel.Flush();
        }

        protected override string GetTimestampAttributeName()
        {
            return MIXPANEL_EVENT_TIME_ATTRIBUTE_NAME;
        }
        
        private Value DictionaryToMixPanelValue(Dictionary<string, object> dict)
        {
            var props = new Value();
            foreach (var pair in dict)
            {
                props.Add(pair.Key, ObjectToValue(pair.Value));
            }
            return props;
        }

        private Value ObjectToValue(object obj)
        {
            if (obj is DateTime dt)
            {
                return dt.ToString("yyyy-dd-MM") + "T" + dt.ToString("HH:mm:ss");
            }
            switch (obj)
            {
                case int i:
                    return new Value(i);
                case float f:
                    return new Value(f);
                case bool b:
                    return new Value(b);
                case string s:
                    return new Value(s);
                case long l:
                    return new Value(l);
                case List<object> list:
                    var valueList = list.Select(ObjectToValue);
                    return new Value(valueList);
                case Dictionary<string, object> dict:
                    var valueDict = new Dictionary<string, Value>();
                    foreach (var pair in dict)
                    {
                        valueDict[pair.Key] = ObjectToValue(pair.Value);
                    }
                    return new Value(valueDict);
                default:
                    throw new Exception("Type is not supported by mixpanel: " + obj.GetType().ToString());
            }
        }
        
        /// <summary>
        /// Mask of supported features of this manager
        /// </summary>
        /// <returns></returns>
        protected override AnalyticsFeatureType GetSupportedFeatures()
        {
            return AnalyticsFeatureType.AccountIncrement | AnalyticsFeatureType.AccountSet |
                   AnalyticsFeatureType.AccountTracking | AnalyticsFeatureType.NamedEvents |
                   AnalyticsFeatureType.AccountSetOnce |
                   AnalyticsFeatureType.AccountAddToList |
                   AnalyticsFeatureType.SetGlobalEventAttribute | 
                   AnalyticsFeatureType.NamedEventsWithNamedAttributes |
                   AnalyticsFeatureType.SetGlobalEventAttributeOnce;
        }
    }
}
#endif