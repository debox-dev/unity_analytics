#if DEBOX_ANALYTICS_APPSFLYER

using System.Collections.Generic;
using AppsFlyerSDK;

namespace DeBox.Analytics.AppsFlyerManager
{
    /// <summary>
    /// An IAnalyticsManager that uses the AppsFlyer SDK as the backend for tracking events
    ///
    /// To use this manager you must install the AppsFlyer SDK. This manager should work with the V5 SDK.
    ///
    /// AppsFlyer do not supply their SDK via the new Unity Packaging system yet,
    /// Because of that, depending on what kind of project you are working on (UPackage or Game) you may have to create an
    ///  assembly definition for the AppsFlyer SDK. If so, call it "AppsFlyerSDK".
    ///
    /// Make sure to add <b>DEBOX_ANALYTICS_APPSFLYER</b> to your symbol definitions when using this manager
    /// </summary>
    public class AppsFlyerAnalyticsManager : BaseAnalyticsManager
    {
        private readonly string _devKey;
        private readonly string _appId;
        private readonly bool _initializeSDK;

        private readonly Dictionary<string, object> _globalTrackingAttributes = new Dictionary<string, object>();

        /// <summary>
        /// Create a new AppsFlyerAnalyticsManager that assumes the AppsFlyer SDK is already initialized and started
        /// </summary>
        /// <param name="timestampModeType"></param>
        public AppsFlyerAnalyticsManager(AutoTimestampModeType timestampModeType) : base(timestampModeType)
        {
            _initializeSDK = false;
        }

        /// <summary>
        /// Create a new AppsFlyerAnalyticsManager that initializes the AppsFlyerSDK on startup
        /// </summary>
        /// <param name="devKey">The AppsFlyer dev key</param>
        /// <param name="appId">The AppsFlyer app Id</param>
        /// <param name="timestampModeType">See documentation of BaseAnalyticsManager</param>
        public AppsFlyerAnalyticsManager(string devKey, string appId,AutoTimestampModeType timestampModeType) : base(timestampModeType)
        {
            _devKey = devKey;
            _appId = appId;
            _initializeSDK = true;
        }
        
        /// <summary>
        /// If we are to initialize the AppsFlyer SDK ourselves, inits the AppsFlyer SDK.
        /// </summary>
        public override void Initialize()
        {
            if (_initializeSDK)
            {
                AppsFlyer.initSDK(_devKey, _appId);
                AppsFlyer.startSDK();
            }
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void Identify(string id)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void Increment(string attribute, int value)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void Increment(string attribute, float value)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void Set(string attribute, object value)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void SetOnce(string attribute, object value)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="values"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void AddToList(string attribute, params object[] values)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="values"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void UnionList(string attribute, params object[] values)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="values"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void IntersectList(string attribute, params object[] values)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="values"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void RemoveFromList(string attribute, params object[] values)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Set a global tracking attribute to be set with all events
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="value">Value to set</param>
        public override void SetTrackingAttribute(string attributeName, object value)
        {
            _globalTrackingAttributes[attributeName] = value;
        }

        /// <summary>
        /// Set a global tracking attribute to be set with all events, but only if it was not previously set
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="value">Value to set</param>
        public override void SetTrackingAttributeOnce(string attributeName, object value)
        {
            if (!_globalTrackingAttributes.ContainsKey(attributeName))
            {
                _globalTrackingAttributes[attributeName] = value;
            }
                
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        /// <param name="price"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void TrackRevenue(string product, int quantity, float price)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Does nothing when using AppsFlyer
        /// </summary>
        public override void Flush()
        {
        }
        
        /// <summary>
        /// Send an AppsFlyer event with no attributes
        /// </summary>
        /// <param name="eventName">Event name</param>
        protected override void TrackEventInternal(string eventName)
        {
            AppsFlyer.sendEvent(eventName, new Dictionary<string, string>());
        }

        /// <summary>
        /// Send an AppsFlyer event with attributes
        /// </summary>
        /// <param name="eventName">Event name</param>
        /// <param name="attributes">Attributes</param>
        protected override void TrackEventInternal(string eventName, Dictionary<string, object> attributes)
        {
            var appsflyerEventAttribs = new Dictionary<string, string>();
            foreach (var p in _globalTrackingAttributes)
            {
                appsflyerEventAttribs[p.Key] = p.Value.ToString();
            }
            foreach (var p in attributes)
            {
                appsflyerEventAttribs[p.Key] = p.Value.ToString();
            }
            AppsFlyer.sendEvent(eventName, appsflyerEventAttribs);
        }

        /// <summary>
        /// Indicates that the AppsFlyerManager only supports global event attributes and sending events
        /// </summary>
        /// <returns>Mask of supported SDK features</returns>
        protected override AnalyticsFeatureType GetSupportedFeatures()
        {
            return AnalyticsFeatureType.SetGlobalEventAttribute | AnalyticsFeatureType.SetGlobalEventAttributeOnce |
                   AnalyticsFeatureType.NamedEvents | AnalyticsFeatureType.NamedEventsWithNamedAttributes;
        }
    }
}

#endif