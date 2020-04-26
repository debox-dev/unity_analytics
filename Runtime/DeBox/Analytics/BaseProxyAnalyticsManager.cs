using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeBox.Analytics
{

    /// <summary>
    /// An abstract proxy analytics manager so various proxy classes can be easily implemented 
    /// </summary>
    public abstract class BaseProxyAnalyticsManager : IAnalyticsManager
    {
        public readonly IAnalyticsManager InternalManager;
        
        /// <summary>
        /// Create a new proxy analytics manager
        /// </summary>
        /// <param name="internalManager">The proxied analytics manager</param>
        protected BaseProxyAnalyticsManager(IAnalyticsManager internalManager)
        {
            InternalManager = internalManager;
        }
        
        /// <summary>
        /// Perform Initialize on contained manager
        /// </summary>
        public virtual void Initialize()
        {
            InternalManager.Initialize();
        }

        /// <summary>
        /// Perform Identify on contained manager
        /// </summary>
        public virtual void Identify(string id)
        {
            InternalManager.Identify(id);
        }

        /// <summary>
        /// Perform Increment on contained manager
        /// </summary>
        public virtual void Increment(string attribute, int value)
        {
            InternalManager.Increment(attribute, value);
        }

        /// <summary>
        /// Perform Increment on contained manager
        /// </summary>
        public virtual void Increment(string attribute, float value)
        {
            InternalManager.Increment(attribute, value);
        }

        /// <summary>
        /// Perform Set on contained manager
        /// </summary>
        public virtual void Set(string attribute, object value)
        {
            InternalManager.Set(attribute, value);
        }

        /// <summary>
        /// Perform SetOnce on contained manager
        /// </summary>
        public virtual void SetOnce(string attribute, object value)
        {
            InternalManager.SetOnce(attribute, value);
        }
        
        /// <summary>
        /// Perform AddToList on contained manager
        /// </summary>
        public virtual void AddToList(string attribute, params object[] values)
        {
            InternalManager.AddToList(attribute, values);
        }

        /// <summary>
        /// Perform UnionList on contained manager
        /// </summary>
        public virtual void UnionList(string attribute, params object[] values)
        {
            InternalManager.UnionList(attribute, values);
        }

        /// <summary>
        /// Perform IntersectList on contained manager
        /// </summary>
        public virtual void IntersectList(string attribute, params object[] values)
        {
            InternalManager.IntersectList(attribute, values);
        }

        /// <summary>
        /// Perform RemoveFromList on contained manager
        /// </summary>
        public virtual void RemoveFromList(string attribute, params object[] values)
        {
            InternalManager.RemoveFromList(attribute, values);
        }

        /// <summary>
        /// Perform TrackEvent on contained manager
        /// </summary>
        public virtual void TrackEvent(string eventName)
        {
            InternalManager.TrackEvent(eventName);
        }

        /// <summary>
        /// Perform TrackEvent on contained manager
        /// </summary>
        public virtual void TrackEvent(string eventName, Dictionary<string, object> attributes)
        {
            InternalManager.TrackEvent(eventName, attributes);
        }

        /// <summary>
        /// Perform SetTrackingAttribute on contained manager
        /// </summary>
        public virtual void SetTrackingAttribute(string attributeName, object value)
        {
            InternalManager.SetTrackingAttribute(attributeName, value);
        }

        /// <summary>
        /// Perform SetTrackingAttributeOnce on contained manager
        /// </summary>
        public virtual void SetTrackingAttributeOnce(string attributeName, object value)
        {
            InternalManager.SetTrackingAttributeOnce(attributeName, value);
        }

        /// <summary>
        /// Perform TrackRevenue on contained manager
        /// </summary>
        public virtual void TrackRevenue(string product, int quantity, float price)
        {
            InternalManager.TrackRevenue(product, quantity, price);
        }

        /// <summary>
        /// Perform Identify on contained manager
        /// </summary>
        public virtual void Flush()
        {
            InternalManager.Flush();
        }

        /// <summary>
        /// Tells if the contained manager supports the specified feature
        /// </summary>
        /// <param name="feature">AnalyticsFeatureType</param>
        /// <returns>True if feature is supported by the contained manager</returns>
        public bool SupportsFeature(AnalyticsFeatureType feature)
        {
            return InternalManager.SupportsFeature(feature);
        }
    }
    
}
