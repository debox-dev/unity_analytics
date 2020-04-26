using System;
using System.Collections.Generic;

namespace DeBox.Analytics
{
    /// <summary>
    /// An IAnalyticsManager that proxies several subordinate IAnalyticsManagers
    ///
    /// Illustration of an AnalyticManagerGroup that sends to both DebugAnalyticsManager and MixPanelAnalyticsManager
    /// <example>
    /// <code>
    ///                             +-----------+
    ///                             | Analytic  |
    ///                             | Manager   |
    ///                             | Group     |
    ///                             +-----+-----+
    ///                                   |
    ///               +-----------+       |       +-----------+
    ///               | Debug     |       |       | MixPanel  |
    ///               | Analytics +<------+------>+ Analytics |
    ///               | Manager   |               | Manager   |
    ///               +-----------+               +-----------+
    /// </code>
    /// </example>
    /// <example>
    /// Usage:
    /// <code>
    /// var debugAnalyticsManager = new DebugAnalyticsManager();
    /// var mixpanelAnalyticsManager = new MixPanelAnalyticsManager();
    /// var analyticsManagerGroup = new AnalyticsManagerGroup(debugAnalyticsManager, mixpanelAnalyticsManager);
    ///
    /// 
    /// analyticsManagerGroup.Initialize(); // Initialize all the managers in the group
    /// 
    /// analyticsManagerGroup.TrackEvent("myEvent"); // Send an event to all the managers in the group
    /// </code>
    /// </example>
    /// </summary>
    public class AnalyticManagerGroup : IAnalyticsManager
    {
        public bool IsStrict { get; private set; }
        private List<IAnalyticsManager> _internalManagers = new List<IAnalyticsManager>();

        /// <summary>
        /// Create a new 'strict' AnalyticManagerGroup that contains the specified IAnalyticManagers
        ///
        /// Strict groups will demand all contained managers will support all the performed operations
        /// or the operation will fail.
        /// 
        /// For example, in a strict group that contains two managers, one of which does not support AddToList,
        /// performing AddToList on the group will raise a NotImplementedException
        /// </summary>
        /// <param name="internalManagers">Collection of IAnalyticsManagers to contain in the group</param>
        public AnalyticManagerGroup(params IAnalyticsManager[] internalManagers) : this(true, internalManagers) {}
        
        /// <summary>
        /// Create a new AnalyticManagerGroup that contains the specified IAnalyticManagers
        /// </summary>
        /// <param name="internalManagers">Collection of IAnalyticsManagers to contain in the group</param>
        /// <param name="isStrict">
        /// If strict is true, the group will demand all contained managers will support all the performed operations
        /// or the operation will fail.
        ///
        /// For example, in a strict group that contains two managers, one of which does not support AddToList,
        /// performing AddToList on the group will raise a NotImplementedException
        ///
        /// If the group is not strict, then the group only sends the operations to the contained managers that support
        /// them, allowing us to build one full-featured analytics manager that is made out of several sub managers
        /// that each only supports part of the features.
        /// </param>
        public AnalyticManagerGroup(bool isStrict, params IAnalyticsManager[] internalManagers)
        {
            _internalManagers.AddRange(internalManagers);
            IsStrict = isStrict;
        }


        private static void ForEach<T>(IEnumerable<T> list, Action<T> action)
        {
            foreach (T item in list)
                action(item);
        }
        
        private void PerformForAll(IEnumerable<IAnalyticsManager> list, Action<IAnalyticsManager> action, AnalyticsFeatureType featureType)
        {
            bool didPerform = false;
            if (!IsStrict || AllSupportFeature(featureType))
            {
                ForEach(list, m =>
                {
                    if (m.SupportsFeature(featureType))
                    {
                        didPerform = true;
                        action(m);
                    }
                });
            }
            if (!didPerform)
            {
                throw new NotImplementedException("No Analytic managers in this group support operation: " 
                                                  + featureType);
            }
        }
        
        /// <summary>
        /// Initialize all the contained managers
        /// </summary>
        public void Initialize()
        {
            ForEach(_internalManagers, m => m.Initialize());
        }

        /// <summary>
        /// Identify the tracked account.
        ///
        /// If the group is strict - all contained managers must support the AnalyticsFeatureType.AccountTracking
        /// If the group is not strict - at least one contained manager must support the AnalyticsFeatureType.AccountTracking
        /// </summary>
        /// <param name="id">Account Id</param>
        /// <exception cref="NotImplementedException">
        /// If the group is strict - indicates at least one manager does not support AnalyticsFeatureType.AccountTracking
        /// If the group is not strict - indicates no managers support AnalyticsFeatureType.AccountTracking
        /// </exception>
        public void Identify(string id)
        {
            PerformForAll(_internalManagers, m => m.Identify(id), AnalyticsFeatureType.AccountTracking);
        }

        /// <summary>
        /// Perform Increment on all contained managers
        /// 
        /// If the group is strict - all contained managers must support the AnalyticsFeatureType.AccountIncrement
        /// If the group is not strict - at least one contained manager must support the AnalyticsFeatureType.AccountIncrement
        /// </summary>
        /// <param name="attribute">Name of attribute to increment</param>
        /// <param name="value">Value to add</param>
        /// <exception cref="NotImplementedException">
        /// If the group is strict - indicates at least one manager does not support AnalyticsFeatureType.AccountIncrement
        /// If the group is not strict - indicates no managers support AnalyticsFeatureType.AccountIncrement
        /// </exception> 
        public void Increment(string attribute, int value)
        {
            PerformForAll(_internalManagers, m => m.Increment(attribute, value), AnalyticsFeatureType.AccountIncrement);
        }

        /// <summary>
        /// Perform Increment on all contained managers
        /// 
        /// If the group is strict - all contained managers must support the AnalyticsFeatureType.AccountIncrement
        /// If the group is not strict - at least one contained manager must support the AnalyticsFeatureType.AccountIncrement
        /// </summary>
        /// <param name="attribute">Name of attribute to increment</param>
        /// <param name="value">Value to add</param>
        /// <exception cref="NotImplementedException">
        /// If the group is strict - indicates at least one manager does not support AnalyticsFeatureType.AccountIncrement
        /// If the group is not strict - indicates no managers support AnalyticsFeatureType.AccountIncrement
        /// </exception> 
        public void Increment(string attribute, float value)
        {
            PerformForAll(_internalManagers, m => m.Increment(attribute, value), AnalyticsFeatureType.AccountIncrement);
        }

        /// <summary>
        /// Perform Set on contained managers
        /// 
        /// If the group is strict - all contained managers must support the AnalyticsFeatureType.AccountSet
        /// If the group is not strict - at least one contained manager must support the AnalyticsFeatureType.AccountSet
        /// </summary>
        /// <param name="attribute">Name of attribute to set</param>
        /// <param name="value">Value to set</param>
        /// <exception cref="NotImplementedException">
        /// If the group is strict - indicates at least one manager does not support AnalyticsFeatureType.AccountSet
        /// If the group is not strict - indicates no managers support AnalyticsFeatureType.AccountSet
        /// </exception> 
        public void Set(string attribute, object value)
        {
            PerformForAll(_internalManagers, m => m.Set(attribute, value), AnalyticsFeatureType.AccountSet);
        }

        /// <summary>
        /// Perform SetOnce on contained managers
        /// 
        /// If the group is strict - all contained managers must support the AnalyticsFeatureType.AccountSetOnce
        /// If the group is not strict - at least one contained manager must support the AnalyticsFeatureType.AccountSetOnce
        /// </summary>
        /// <param name="attribute">Name of attribute to set</param>
        /// <param name="value">Value to set</param>
        /// <exception cref="NotImplementedException">
        /// If the group is strict - indicates at least one manager does not support AnalyticsFeatureType.AccountSetOnce
        /// If the group is not strict - indicates no managers support AnalyticsFeatureType.AccountSetOnce
        /// </exception> 
        public void SetOnce(string attribute, object value)
        {
            PerformForAll(_internalManagers, m => m.SetOnce(attribute, value), AnalyticsFeatureType.AccountSetOnce);
        }

        /// <summary>
        /// Perform AddToList on contained managers
        /// 
        /// If the group is strict - all contained managers must support the AnalyticsFeatureType.AccountAddToList
        /// If the group is not strict - at least one contained manager must support the AnalyticsFeatureType.AccountAddToList
        /// </summary>
        /// <param name="attribute">Name of attribute to set</param>
        /// <param name="values">Values to add</param>
        /// <exception cref="NotImplementedException">
        /// If the group is strict - indicates at least one manager does not support AnalyticsFeatureType.AccountAddToList
        /// If the group is not strict - indicates no managers support AnalyticsFeatureType.AccountAddToList
        /// </exception> 
        public void AddToList(string attribute, params object[] values)
        {
            PerformForAll(_internalManagers, m => m.AddToList(attribute, values), AnalyticsFeatureType.AccountAddToList);
        }

        /// <summary>
        /// Perform UnionList on contained managers
        /// 
        /// If the group is strict - all contained managers must support the AnalyticsFeatureType.AccountUnionList
        /// If the group is not strict - at least one contained manager must support the AnalyticsFeatureType.AccountUnionList
        /// </summary>
        /// <param name="attribute">Name of attribute</param>
        /// <param name="values">Values to union with</param>
        /// <exception cref="NotImplementedException">
        /// If the group is strict - indicates at least one manager does not support AnalyticsFeatureType.AccountUnionList
        /// If the group is not strict - indicates no managers support AnalyticsFeatureType.AccountUnionList
        /// </exception> 
        public void UnionList(string attribute, params object[] values)
        {
            PerformForAll(_internalManagers, m => m.UnionList(attribute, values), AnalyticsFeatureType.AccountUnionList);
        }

        /// <summary>
        /// Perform IntersectList on contained managers
        /// 
        /// If the group is strict - all contained managers must support the AnalyticsFeatureType.AccountIntersectList
        /// If the group is not strict - at least one contained manager must support the AnalyticsFeatureType.AccountIntersectList
        /// </summary>
        /// <param name="attribute">Name of attribute</param>
        /// <param name="values">Values to intersect with</param>
        /// <exception cref="NotImplementedException">
        /// If the group is strict - indicates at least one manager does not support AnalyticsFeatureType.AccountIntersectList
        /// If the group is not strict - indicates no managers support AnalyticsFeatureType.AccountIntersectList
        /// </exception> 
        public void IntersectList(string attribute, params object[] values)
        {
            PerformForAll(_internalManagers, m => m.IntersectList(attribute, values), AnalyticsFeatureType.AccountIntersectList);
        }

        /// <summary>
        /// Perform RemoveFromList on contained managers
        /// 
        /// If the group is strict - all contained managers must support the AnalyticsFeatureType.AccountRemoveFromList
        /// If the group is not strict - at least one contained manager must support the AnalyticsFeatureType.AccountRemoveFromList
        /// </summary>
        /// <param name="attribute">Name of attribute</param>
        /// <param name="values">Values to remove</param>
        /// <exception cref="NotImplementedException">
        /// If the group is strict - indicates at least one manager does not support AnalyticsFeatureType.AccountRemoveFromList
        /// If the group is not strict - indicates no managers support AnalyticsFeatureType.AccountRemoveFromList
        /// </exception> 
        public void RemoveFromList(string attribute, params object[] values)
        {
            PerformForAll(_internalManagers, m => m.RemoveFromList(attribute, values), AnalyticsFeatureType.AccountRemoveFromList);
        }

        /// <summary>
        /// Perform TrackEvent on contained managers
        /// 
        /// If the group is strict - all contained managers must support the AnalyticsFeatureType.NamedEvents
        /// If the group is not strict - at least one contained manager must support the AnalyticsFeatureType.NamedEvents
        /// </summary>
        /// <param name="eventName">Name of event</param>
        /// <exception cref="NotImplementedException">
        /// If the group is strict - indicates at least one manager does not support AnalyticsFeatureType.NamedEvents
        /// If the group is not strict - indicates no managers support AnalyticsFeatureType.NamedEvents
        /// </exception> 
        public void TrackEvent(string eventName)
        {
            PerformForAll(_internalManagers, m => m.TrackEvent(eventName), AnalyticsFeatureType.NamedEvents);
        }

        /// <summary>
        /// Perform TrackEvent on contained managers
        /// 
        /// If the group is strict - all contained managers must support the AnalyticsFeatureType.NamedEventsWithNamedAttributes
        /// If the group is not strict - at least one contained manager must support the AnalyticsFeatureType.NamedEventsWithNamedAttributes
        /// </summary>
        /// <param name="eventName">Name of event</param>
        /// <param name="attributes">Attributes to set for the event</param>
        /// <exception cref="NotImplementedException">
        /// If the group is strict - indicates at least one manager does not support AnalyticsFeatureType.NamedEventsWithNamedAttributes
        /// If the group is not strict - indicates no managers support AnalyticsFeatureType.NamedEventsWithNamedAttributes
        /// </exception> 
        public void TrackEvent(string eventName, Dictionary<string, object> attributes)
        {
            PerformForAll(_internalManagers, m => m.TrackEvent(eventName, attributes), AnalyticsFeatureType.NamedEventsWithNamedAttributes);
        }

        /// <summary>
        /// Perform SetTrackingAttribute on contained managers
        /// 
        /// If the group is strict - all contained managers must support the AnalyticsFeatureType.SetGlobalEventAttribute
        /// If the group is not strict - at least one contained manager must support the AnalyticsFeatureType.SetGlobalEventAttribute
        /// </summary>
        /// <param name="attributeName">Name of attribute</param>
        /// <param name="value">Attribute value to set</param>
        /// <exception cref="NotImplementedException">
        /// If the group is strict - indicates at least one manager does not support AnalyticsFeatureType.SetGlobalEventAttribute
        /// If the group is not strict - indicates no managers support AnalyticsFeatureType.SetGlobalEventAttribute
        /// </exception> 
        public void SetTrackingAttribute(string attributeName, object value)
        {
            PerformForAll(_internalManagers, m => m.SetTrackingAttribute(attributeName, value), AnalyticsFeatureType.SetGlobalEventAttribute);
        }

        /// <summary>
        /// Perform SetTrackingAttributeOnce on contained managers
        /// 
        /// If the group is strict - all contained managers must support the AnalyticsFeatureType.SetGlobalEventAttributeOnce
        /// If the group is not strict - at least one contained manager must support the AnalyticsFeatureType.SetGlobalEventAttributeOnce
        /// </summary>
        /// <param name="attributeName">Name of attribute</param>
        /// <param name="value">Attribute value to set</param>
        /// <exception cref="NotImplementedException">
        /// If the group is strict - indicates at least one manager does not support AnalyticsFeatureType.SetGlobalEventAttributeOnce
        /// If the group is not strict - indicates no managers support AnalyticsFeatureType.SetGlobalEventAttributeOnce
        /// </exception> 
        public void SetTrackingAttributeOnce(string attributeName, object value)
        {
            PerformForAll(_internalManagers, m => m.SetTrackingAttributeOnce(attributeName, value), AnalyticsFeatureType.SetGlobalEventAttributeOnce);
        }

        /// <summary>
        /// Perform TrackRevenue on contained managers
        /// 
        /// If the group is strict - all contained managers must support the AnalyticsFeatureType.RevenueTracking
        /// If the group is not strict - at least one contained manager must support the AnalyticsFeatureType.RevenueTracking
        /// </summary>
        /// <param name="product">The id or name of the product</param>
        /// <param name="quantity">Amount purchased</param>
        /// <param name="price">Total revenue gained</param>
        /// <exception cref="NotImplementedException">
        /// If the group is strict - indicates at least one manager does not support AnalyticsFeatureType.RevenueTracking
        /// If the group is not strict - indicates no managers support AnalyticsFeatureType.RevenueTracking
        /// </exception> 
        public void TrackRevenue(string product, int quantity, float price)
        {
            PerformForAll(_internalManagers, m => m.TrackRevenue(product, quantity, price), AnalyticsFeatureType.RevenueTracking);
        }

        /// <summary>
        /// Perform Flush on all contained managers
        /// </summary> 
        public void Flush()
        {
            ForEach(_internalManagers, m => m.Flush());
        }

        /// <summary>
        /// Tells if any of the contained managers in the group support this feature
        /// </summary>
        /// <param name="feature">Type of the analytics feature</param>
        /// <returns>true if one or more contained managers support the feature</returns>
        public bool SupportsFeature(AnalyticsFeatureType feature)
        {
            return IsStrict ? AllSupportFeature(feature) : AnySupportFeature(feature);
        }

        private bool AnySupportFeature(AnalyticsFeatureType feature)
        {
            bool doesSupport = false;
            foreach (var m in _internalManagers)
                doesSupport = doesSupport || m.SupportsFeature(feature);
            return doesSupport;
        }
        
        private bool AllSupportFeature(AnalyticsFeatureType feature)
        {
            bool doesSupport = false;
            foreach (var m in _internalManagers)
            {
                if (!m.SupportsFeature(feature))
                {
                    return false;
                }
                doesSupport = true;
            }

            return doesSupport;
        }
    }
    
}
