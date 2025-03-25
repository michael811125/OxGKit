## CHANGELOG

## [1.4.5] - 2025-03-26
- Added IRemoveCacheHandler interface for Cacher.
- Modified Requester can new (instance).
- Fixed LRUK minCounter bug issue.

## [1.4.4] - 2025-03-26
- Added Tests.
- Renamed UMT.CancelAllCoroutines().
- Optimized Cacher and added thread lock feature.

## [1.4.3] - 2025-03-25
- Modified Requester's errorAction to return ErrorInfo.
- Fixed handling of successAction and errorAction invocation logic.
- Optmized Requester code.

## [1.4.2] - 2025-03-21
- Fixed CancellationTokenSource disposal bug.
- Optimized Requester code.

## [1.4.1] - 2025-03-20
- Modified the Requester to support a timeout feature, with a default timeout of 180 seconds.

## [1.4.0] - 2025-03-19
- Renamed namespace OxGKit.Utilities.Request to OxGKit.Utilities.Requester.
- Removed Cursor will be separated into an independent CursorSystem (namespace OxGKit.Utilities.CursorAnim -> OxGKit.CursorSystem).
- Removed ButtonPlus will be separated into an independent ButtonSystem (namespace OxGKit.Utilities.Button -> OxGKit.ButtonSystem).
- Removed Timer will be separated into an independent TimeSystem (namespace OxGKit.Utilities.Timer -> OxGKit.TimeSystem).
- Removed Singleton will be separated into an independent SingletonSystem (namespace OxGKit.Utilities.Singleton -> OxGKit.SingletonSystem).
- Removed NodePool will be separated into an independent PoolSystem (namespace OxGKit.Utilities.Pool -> OxGKit.PoolSystem).

## [1.3.1] - 2025-03-18
- Added EasyAnim (AnimEnd = key event).
- Added DontDestroy.
- Fixed Cursors namespace.

## [1.3.0] - 2025-03-10
- Added onLateUpdate callback in DTUpdater and RTUpdater.
- Added IsRunning() method in DTUpdater, RTUpdater, IntervalTimer.
- Added CheckIsRunning() method in IntervalSetter.

## [1.2.2] - 2025-02-11
- Added delayFrameAfterSpawnCount for NodePool (Allows setting a delay after producing N objects, where a delay of N frames occurs after every N objects are produced).

## [1.2.1] - 2025-01-13
- Fixed the issue where TextureAnimation spIdx was not reset to zero, and adjusted it to execute ResetAnim in OnDisable.

## [1.2.0] - 2025-01-10
- Added Cursors APIs to replace the original CursorManager.GetInstance().
- Added NodePoolDemo, which can be imported from the Samples in the Package Manager.
- Modified the access modifier of CursorManager.GetInstance(), please use Cursors for invocation.
- Optimized NodePool, supports loading across frames, supports setting a limit for the max size, and supports manual initialization.
- Removed the Size() method from NodePool, replaced it with Count().

## [1.1.2] - 2024-10-22
- Added ScalingEnabled trigger in CursorManager.
- Optimized memory allocation and reuse in CursorManager t2d.

## [1.1.1] - 2024-10-22
- Modified CursorManager can set lock mode and visible, also can set cursor mode.
- Modified CursorManager demo in Samples.

## [1.1.0] - 2024-10-21
- Added CursorManager support for both static and dynamic modes, and allow setting the cursor's animation state.
- Added CursorManager prefab in Samples.
- Added CursorManager demo in Samples.

## [1.0.5] - 2024-06-20
- Modified NewSingleton.
- Modified MonoSingleton.

## [1.0.4] - 2024-05-12
- Fixed UniTask.SwitchToThreadPool() not supported on WebGL issue.
- Modified NtpTime so that when performing a sync it will wait to ensure the sync has occurred..

## [1.0.3] - 2024-03-20
- Fixed NtpTime switch back to main thread.

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