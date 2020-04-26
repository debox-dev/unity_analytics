using System;
using System.Collections.Generic;

namespace DeBox.Analytics
{
    /// <summary>
    /// Common base class for analytic managers with common functionality
    /// </summary>
    public abstract class BaseAnalyticsManager : IAnalyticsManager
    {
        public enum AutoTimestampModeType
        {
            AutomaticallyTimestamp,
            DontAutomaticallyTimestamp
        }

        /// <summary>
        /// The attribute that should be used for event timestamps when using DeBox.Analytics managers
        /// </summary>
        private const string DEFAULT_EVENT_TIME_ATTRIBUTE_NAME = "timestamp";

        /// <summary>
        /// If this is true, this manager auto-stamps the current time to tracked events
        /// </summary>
        public bool AutoTimestampEvents { get; protected set; }

        /// <summary>
        /// Initialize this base with the specified mode 
        /// </summary>
        /// <param name="timestampModeType">Indicates if this manager auto-timestamps events or not</param>
        protected BaseAnalyticsManager(AutoTimestampModeType timestampModeType)
        {
            AutoTimestampEvents = timestampModeType == AutoTimestampModeType.AutomaticallyTimestamp;
        }

        /// <summary>
        /// See IAnalyticsManager
        /// </summary>
        public abstract void Initialize();
        
        /// <summary>
        /// See IAnalyticsManager
        /// </summary>
        public abstract void Identify(string id);
        
        /// <summary>
        /// See IAnalyticsManager
        /// </summary>
        public abstract void Increment(string attribute, int value);
        
        /// <summary>
        /// See IAnalyticsManager
        /// </summary>
        public abstract void Increment(string attribute, float value);
        
        /// <summary>
        /// See IAnalyticsManager
        /// </summary>
        public abstract void Set(string attribute, object value);
        
        /// <summary>
        /// See IAnalyticsManager
        /// </summary>
        public abstract void SetOnce(string attribute, object value);
        
        /// <summary>
        /// See IAnalyticsManager
        /// </summary>
        public abstract void AddToList(string attribute, params object[] values);
        
        /// <summary>
        /// See IAnalyticsManager
        /// </summary>
        public abstract void UnionList(string attribute, params object[] values);
        
        /// <summary>
        /// See IAnalyticsManager
        /// </summary>
        public abstract void IntersectList(string attribute, params object[] values);
        
        /// <summary>
        /// See IAnalyticsManager
        /// </summary>
        public abstract void RemoveFromList(string attribute, params object[] values);

        /// <summary>
        /// See IAnalyticsManager
        /// </summary>
        public abstract void SetTrackingAttribute(string attributeName, object value);
        
        /// <summary>
        /// See IAnalyticsManager
        /// </summary>
        public abstract void SetTrackingAttributeOnce(string attributeName, object value);
        
        /// <summary>
        /// See IAnalyticsManager
        /// </summary>
        public abstract void TrackRevenue(string product, int quantity, float price);
        
        /// <summary>
        /// See IAnalyticsManager
        /// </summary>
        public abstract void Flush();
        
        /// <summary>
        /// Returns the current date-time.
        ///
        /// Can be overridden
        /// </summary>
        /// <returns></returns>
        protected virtual DateTime GetCurrentTime()
        {
            return DateTime.UtcNow;
        }

        /// <summary>
        /// Returns the timestamp attribute this manager uses. Defaults to DEFAULT_EVENT_TIME_ATTRIBUTE_NAME
        ///
        /// Can be overridden
        /// </summary>
        /// <returns>Attribute name</returns>
        protected virtual string GetTimestampAttributeName()
        {
            return DEFAULT_EVENT_TIME_ATTRIBUTE_NAME;
        }
        
        /// <summary>
        /// Performs TrackEventInternal
        ///
        /// If this manager auto-timestamps, the event will be added with the timestamp attribute set to current time 
        /// </summary>
        /// <param name="eventName">Name of the event to track</param>
        public void TrackEvent(string eventName)
        {
            if (AutoTimestampEvents)
            {
                var attrs = new Dictionary<string, object>();
                attrs[GetTimestampAttributeName()] = GetCurrentTime();
                TrackEventInternal(eventName, attrs);
            }
            else
            {
                TrackEventInternal(eventName);
            }
        }

        /// <summary>
        /// Performs TrackEventInternal
        /// 
        /// If this manager auto-timestamps, the event will be added with the timestamp attribute set to current time 
        /// </summary>
        /// <param name="eventName">Name of the event to track</param>
        /// <param name="attributes">Event parameters</param>
        public void TrackEvent(string eventName, Dictionary<string, object> attributes)
        {
            if (AutoTimestampEvents && !attributes.ContainsKey(GetTimestampAttributeName()))
            {
                attributes[GetTimestampAttributeName()] = DateTime.UtcNow;
            }
            TrackEventInternal(eventName, attributes);
        }

        /// <summary>
        /// Implement this instead of IAnalyticsManager.TrackEvent 
        /// </summary>
        /// <param name="eventName">Name of the event to track</param>
        protected abstract void TrackEventInternal(string eventName);

        /// <summary>
        /// Implement this instead of IAnalyticsManager.TrackEvent 
        /// </summary>
        /// <param name="eventName">Name of the event to track</param>
        /// <param name="attributes">Event parameters</param>
        protected abstract void TrackEventInternal(string eventName, Dictionary<string, object> attributes);

        /// <summary>
        /// Returns a mask describing the features supported by this manager.
        /// </summary>
        /// <returns></returns>
        protected abstract AnalyticsFeatureType GetSupportedFeatures();
        
        /// <summary>
        /// Returns true if the manager supports the specified feature
        /// </summary>
        /// <param name="feature">AnalyticsFeatureType type</param>
        /// <returns>true if specified AnalyticsFeatureType is supported</returns>
        public virtual bool SupportsFeature(AnalyticsFeatureType feature)
        {
            return ((feature & GetSupportedFeatures()) > 0);
        }
    }
}
