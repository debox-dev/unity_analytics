# DeBox Analytics

Meta Analtic Management system for various platforms

## Installation instructions
### Quick Installation
1. Install Json .net from the Asset Store

2. Put this in your `Packages/manigest.json` file
```
"com.debox.playerprefs": "https://github.com/debox-dev/playerprefs.git",
"com.debox.analytics": "https://github.com/debox-dev/unity_analytics.git",
```

## Requirements
- Unity 2019 or higher.
- DeBox PlayerPrefs Extensions
- NewtonSoft JSON

## Documentation
[Documentation link](https://debox-dev.github.io/unity_analytics/Docs/html/index.html)

## Examples

### Simple setup with the debug manager
```
var myAnalyticsManager = new DebugAnalyticsManager();
myAnalyticsManager.TrackEvent("myEvent");
```

### Set up an auto-flushing analytics manager
```
var debugAnalyticsManager = new DebugAnalyticsManager();
this.analyticsManager = new AutoFlushAnalyticsManager(debugAnalyticsManager, 30); // Flush every 30 seconds 
```

### Set up an auto-flushing, locally caching analytics manager
```
var debugAnalyticsManager = new DebugAnalyticsManager();
var cachedAnalyticsManager = new PlayerPrefsCachedAnalyticsManager("pprefKey", debugAnalyticsManager);
this.analyticsManager = new AutoFlushAnalyticsManager(cachedAnalyticsManager, 30); // Flush every 30 seconds 
```

### Grouping analytics managers
```
var myFbAnalyticsManager = new FBAnalyticsManager();
var myMixPanelAnalyticsManager = new MixPanelAnalyticsManager();

var mainAnalyticsManager = new AnalyticManagerGroup(myFbAnalyticsManager, myMixPanelAnalyticsManager);

mainAnalyticsManager.TrackEvent("myEvent"); // Will track myEvent to both Facebook and MixPanel
```

### Using the event builder
#### Send event to a specific manager
```
var myAnalyticsManager = new DebugAnalyticsManager();
myAnalyticsManager.Initialize();
 
new AnalyticsEvent("myEvent")
     .Track("param1", 1)
     .Track("param2", "Something something dark side")
     .Send(myAnalyticsManager);
```

#### Set up a default manager for events
```
var myAnalyticsManager = new DebugAnalyticsManager();
myAnalyticsManager.Initialize();

// Now calling Send() on AnalyticsEvent instances will send to this manager. 
AnalyticsEvent.SetDefaultAnalyticsManager(myAnalyticsManager);

new AnalyticsEvent("myEvent")
     .Track("param1", 1)
     .Track("param2", "Something something dark side")
     .Send();
```
