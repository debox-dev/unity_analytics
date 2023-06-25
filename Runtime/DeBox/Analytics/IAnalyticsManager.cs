using System;
using System.Collections.Generic;

namespace DeBox.Analytics
{
    /// <summary>
    /// Describes various feature types that implementations of IAnalyticsManager can support
    /// </summary>
    [Flags]
    public enum AnalyticsFeatureType
    {
        None = 0,
        
        /// <summary>
        /// Account tracking feature 
        /// </summary>
        AccountTracking = 1,
        
        /// <summary>
        /// Increment the numeric value of an attribute of the tracked account
        /// </summary>
        AccountIncrement = 2,
        
        /// <summary>
        /// Set the value of an attribute of the tracked account
        /// </summary>
        AccountSet = 4,
        
        /// <summary>
        /// Set the value of an attribute of the tracked account, only if not yet set
        /// </summary>
        AccountSetOnce = 8,
        
        /// <summary>
        /// Add values to a list attribute of the tracked account
        /// </summary>
        AccountAddToList = 16,
        
        /// <summary>
        /// Add unique values to a list attribute of the tracked account
        /// </summary>
        AccountUnionList = 32,
        
        /// <summary>
        /// Keep only intersecting values in a list attribute of the tracked account
        /// </summary>
        AccountIntersectList = 64,
        
        /// <summary>
        /// Remove values from a list attribute of the tracked account
        /// </summary>
        AccountRemoveFromList = 128,
        
        /// <summary>
        /// Send a tracking event with only a name
        /// </summary>
        NamedEvents = 256,
        
        /// <summary>
        /// Send a tracking event with named attributes
        /// </summary>
        NamedEventsWithNamedAttributes = 1024,
        
        /// <summary>
        /// Set a global tracking attribute to include in all tracked events
        /// </summary>
        SetGlobalEventAttribute = 2048,
        
        /// <summary>
        /// Set a global tracking attribute to include in all tracked events, but only if the attribute
        /// was not yet set since the last initialize
        /// </summary>
        SetGlobalEventAttributeOnce = 4096,
        
        /// <summary>
        /// Tracking transaction revenue
        /// </summary>
        RevenueTracking = 8192,
    }
    
    /// <summary>
    /// Generic AnalyticsManager to wrap specific Analytics SDKs for simplified use
    /// </summary>
    public interface IAnalyticsManager
    {
        /// <summary>
        /// Initialize the analytics manager
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Identify a specific account unique Id to track
        /// </summary>
        /// <param name="id">The unique Id of the account</param>
        void Identify(string id);
        
        /// <summary>
        /// Increment an integer value for the tracked account
        /// </summary>
        /// <param name="attribute">Attribute name to increment</param>
        /// <param name="value">Increment amount</param>
        void Increment(string attribute, int value);

        /// <summary>
        /// Increment a float value for the tracked account
        /// </summary>
        /// <param name="attribute">Attribute name to increment</param>
        /// <param name="value">Increment amount</param>
        void Increment(string attribute, float value);
        
        /// <summary>
        /// Set an attribute value for the tracked account
        /// </summary>
        /// <param name="attribute">Attribute name to set</param>
        /// <param name="value">Value to set</param>
        void Set(string attribute, object value);

        /// <summary>
        /// Set an attribute value for the tracked account, but only if not yet set
        /// </summary>
        /// <param name="attribute">Attribute name to set</param>
        /// <param name="value">Value to set</param>
        void SetOnce(string attribute, object value);

        /// <summary>
        /// Add a value to a list attribute
        /// </summary>
        /// <param name="attribute">Attribute name</param>
        /// <param name="values">Value to add</param>
        void AddToList(string attribute, params object[] values);
        
        /// <summary>
        /// Perform a union of the list in the specified attribute and the given list
        /// </summary>
        /// <param name="attribute">Attribute name to add the values to</param>
        /// <param name="values">List of values to union with</param>
        void UnionList(string attribute, params object[] values);

        /// <summary>
        /// Perform am intersection of the list in the specified attribute and the given list
        /// </summary>
        /// <param name="attribute">Attribute to intersect</param>
        /// <param name="values">List of values to use for the intersection</param>
        void IntersectList(string attribute, params object[] values);

        /// <summary>
        /// Remove values from an attribute containing a list of values
        /// </summary>
        /// <param name="attribute">Attribute name</param>
        /// <param name="values">Values to remove</param>
        void RemoveFromList(string attribute, params object[] values);
        
        /// <summary>
        /// Submit a new event
        /// </summary>
        /// <param name="eventName">Name of the event</param>
        void TrackEvent(string eventName);

        /// <summary>
        /// Submit a new event with additional attributes
        /// </summary>
        /// <param name="eventName">Name of the event</param>
        /// <param name="attributes">Attributes dictionary for the event</param>
        void TrackEvent(string eventName, Dictionary<string, object> attributes);

        /// <summary>
        /// Set a global tracking attribute to send with all the events
        /// </summary>
        /// <param name="attributeName">Name of the attribute</param>
        /// <param name="value">Attribute value</param>
        void SetTrackingAttribute(string attributeName, object value);

        /// <summary>
        /// Set a global tracking attribute to send with all the events, if not already set
        /// </summary>
        /// <param name="attributeName">Name of the attribute</param>
        /// <param name="value">Attribute value</param>
        void SetTrackingAttributeOnce(string attributeName, object value);

        /// <summary>
        /// Track a revenue transaction
        /// </summary>
        /// <param name="product">Purchased product ID</param>
        /// <param name="quantity">Amount of product purchased</param>
        /// <param name="price">Amount of cash spend on product</param>
        void TrackRevenue(string product, int quantity, float price);

        /// <summary>
        /// Immediately force submission of all events and properties set since the last Flush
        /// </summary>
        void Flush();

        /// <summary>
        /// Indicates whether or not the implementing class supports an analytics feature
        /// </summary>
        /// <param name="feature">The type of feature</param>
        /// <returns>true if the feature is supported</returns>
        bool SupportsFeature(AnalyticsFeatureType feature);

    }
}
