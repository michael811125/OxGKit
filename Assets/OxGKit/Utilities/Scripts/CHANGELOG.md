## CHANGELOG

## [1.0.2] - 2024-03-18
- Added NtpTime (Network Time Protocol) to Utilities.Timer.
- Updated TimerDemo.

## [1.0.1] - 2024-03-15
- Modified set #ROOTNAMESPACE# symbol in script templates.

## [1.0.0] - 2024-02-02
- Stabled

## [0.0.14-preivew] - 2024-01-10
- Modified Utilities.Timer try safety.

## [0.0.13-preview] - 2023-12-23
- Added methods in IntervalSetter.
```C#
    public static IntervalTimer SetIntervalOnThread(Action action, int milliseconds, bool ignoreTimeScale = false)
    
    public static bool TryClearInterval(IntervalTimer intervalTimer)
    
    public static void ClearAllIntervalTimers()
```
- Modified TryClear moethods in IntervalSetter, will return value is bool.

## [0.0.12-preview] - 2023-12-11
- Modified Timer.RTUpdater, Timer.IntervalTimer supported switch to thread (based on UniTask).
```C#
    // RTUpdater
    public void StartOnThread()
    
    // IntervalTimer
    public void SetIntervalOnThread(Action action, int milliseconds, bool ignoreTimeScale = false)
```

## [0.0.11-preview] - 2023-10-28
- Added Logger (dependency LoggingSystem). 

## [0.0.10-preview] - 2023-10-20
- Added MonoSingleton demo.
- Redesigned MonoSingleton.

## [0.0.9-preview] - 2023-10-17
- Optmized Cacher (LRU, ARC) can handle Unity.Object to make sure release Unity.Object from memory.

## [0.0.8-preview] - 2023-10-16
- Added Remove method for Cacher.
- Added Remove method for Requester (can remove specific url from cache).
- Optmized RemoveFromCacheForTexture2d method in Requester (make sure to destory t2d).

## [0.0.7-preview] - 2023-09-11
- Added InterTimer.
- Added IntervalSetter (global factory).
- Removed SetInterval from Timer.
- Adjusted Timer and Updater.

## [0.0.6-preview] - 2023-09-10
- Added TimerDemo in Samples.
- Fixed Timer play bug issue.
- Fixed Updater cancel bug issue.
- Modifier Timer name of methods SetSpeed to SetTimeSpeed and GetSpeed to GetTimeSpeed.
- Modifier Updater name of methods StartUpdate to Start and StopUpdate to Stop.
- Renamed RTUpdate to RTUpdater.
- Renamed DTUpdate to DTUpdater.
- Optmized Updater.

## [0.0.5-preview] - 2023-09-09
- Added Requester has clear LRU and ARC methods.
- Added Cacher has clear methods.
- Modifier RealTimer create time by itself (don't need to init startup time by RealTime).
- Modifier RTUpdate create time by itself (don't need to init startup time by RealTime).
- Optmized Timer.

## [0.0.4-preview] - 2023-08-24
- Modifier UMT will auto create an instance use singleton mode.

## [0.0.3-preview] - 2023-08-22
- Renamed UnityMainThread changes to UMT.
- Fixed UMT access modifier issue.

## [0.0.2-preview] - 2023-07-09
- Fixed ButtonPlus ignoreTimescale option doesn't appear on inspector.

## [0.0.1-preview] - 2023-07-08
- Added Utilities.