using System.Collections.Generic;
using UnityEngine;

namespace DeBox.Analytics
{
    /// <summary>
    /// A base helper class for building events
    /// </summary>
    public abstract class BaseAnalyticsEvent
    {
        /// <summary>
        /// Event name
        /// </summary>
        public readonly string Name;


        private static IAnalyticsManager _defaultAnalyticsManager = null;

        private Dictionary<string, object> _data;

        /// <summary>
        /// Set this to an analytics manager to use the "Send(void)" function
        /// </summary>
        /// <param name="manager">IAnalyticsManager implementation</param>
        public static void SetDefaultAnalyticsManager(IAnalyticsManager manager)
        {
            _defaultAnalyticsManager = manager;
        }
        
        /// <summary>
        /// Initialize the event with its name
        /// </summary>
        /// <param name="eventName">The name of the event</param>
        protected BaseAnalyticsEvent(string eventName)
        {
            Name = eventName;
        }

        /// <summary>
        /// Register an attribute to the event
        /// </summary>
        /// <param name="attribute">Attribute name</param>
        /// <param name="value">Attribute value</param>
        protected void TrackInternal(string attribute, object value)
        {
            if (_data == null)
            {
                _data = new Dictionary<string, object>();
            }
            _data[attribute] = value;
        }

        /// <summary>
        /// Send the event via the specified analytics manager
        /// </summary>
        /// <param name="manager">The analytics manager that will process the event</param>
        public void Send(IAnalyticsManager manager)
        {
            if (_data == null)
            {
                manager.TrackEvent(Name);  
            }
            else
            {
                manager.TrackEvent(Name, _data);
            }
        }

        /// <summary>
        /// Send the event via the default analytics manager
        /// </summary>
        public void Send()
        {
            if (_defaultAnalyticsManager == null)
            {
                Debug.LogError("Please set DeBox.Analytics.BaseAnalyticsEvent.SetDefaultAnalyticsManager before calling Send()");
                return;
            }
            Send(_defaultAnalyticsManager);
        }
    }
    
    /// <summary>
    /// A helper class for building events
    /// <example>
    /// Usage:
    /// <code>
    /// var e = new AnalyticsEvent("MyEvent1");
    /// e.Track("Level", 5)
    ///  .Track("Xp": 12545)
    ///  .Track("Name": "QuestMaster");
    /// e.Send(debugAnalyticsManager);
    /// </code>
    /// </example>
    /// </summary>
    public class AnalyticsEvent : BaseAnalyticsEvent
    {
        /// <summary>
        /// Create a new analytics event
        ///
        /// <example>
        /// Example:
        /// <code>
        /// // using DeBox.Analytics;
        ///
        /// var myAnalyticsManager = new DebugAnalyticsManager();
        /// myAnalyticsManager.Initialize();
        /// 
        /// new AnalyticsEvent("myEvent")
        ///     .Track("param1", 1)
        ///     .Track("param2", "Something something dark side")
        ///     .Send(myAnalyticsManager);
        /// </code>
        /// </example> 
        /// </summary>
        /// <param name="eventName">Name of the event</param>
        public AnalyticsEvent(string eventName) : base(eventName) {}

        /// <summary>
        /// Register an attribute to the event
        /// </summary>
        /// <param name="attribute">Attribute name</param>
        /// <param name="value">Attribute value</param>
        public AnalyticsEvent Track(string attribute, object value)
        {
            TrackInternal(attribute, value);
            return this;
        }
    }
}
