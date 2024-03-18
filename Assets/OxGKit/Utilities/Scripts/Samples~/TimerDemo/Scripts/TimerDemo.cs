using OxGKit.Utilities.Timer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TimerDemo : MonoBehaviour
{
    public float waitTime = 3f;
    public float tickTime = 2f;
    public int intervalTime = 1;
    public Slider timeScaleSlider = null;
    public Text timeScaleText = null;

    // Timer
    private RealTimer _realTimer;
    private DeltaTimer _deltaTimer;

    // Ticker
    private bool _isTickerRuning = false;
    private RealTimer _realTicker;
    private DeltaTimer _deltaTicker;

    // Updater
    private bool _isUpdaterRuning = false;
    private RTUpdater _rtUpdater;
    private DTUpdater _dtUpdater;

    // IntervalTimer
    private bool _isIntervalTimerRuning = false;
    private const int _intervalTimerId = 1;
    private IntervalTimer _intervalTimer;

    // NTP
    private bool _ntpTimePrint = false;

    private void Start()
    {
        // NTP Sync
        NtpTime.Synchronize();

        // Timer
        this._realTimer = new RealTimer();
        this._deltaTimer = new DeltaTimer();

        // Ticker
        this._realTicker = new RealTimer();
        this._deltaTicker = new DeltaTimer();

        // Updater
        this._rtUpdater = new RTUpdater();
        this._rtUpdater.onUpdate += this._OnRTUpdater;
        this._dtUpdater = new DTUpdater();
        this._dtUpdater.onUpdate += this._OnDTUpdater;

        // Slider event for Updater
        this.timeScaleSlider.value = 1f;
        this.timeScaleSlider.maxValue = 10f;
        this.timeScaleText.text = $"TimeScale: 1.0";
        this.timeScaleSlider.onValueChanged.RemoveAllListeners();
        this.timeScaleSlider.onValueChanged.AddListener((val) =>
        {
            // RTUpdater timeScale (also can set targetFrameRate)
            this._rtUpdater.timeScale = val;

            // DTUpdater timeScale (also can set targetFrameRate)
            // Note: The DTUpdater also will affect by Unity Time.timeScale
            this._dtUpdater.timeScale = val;

            // Refresh text
            this.timeScaleText.text = $"TimeScale: {val.ToString("f1")}";
        });

        // IntervalTimer
        this._intervalTimer = new IntervalTimer();
    }

    private void Update()
    {
        #region NTP Time
        if (Keyboard.current.bKey.wasReleasedThisFrame)
            this._ntpTimePrint = !this._ntpTimePrint;

        if (this._ntpTimePrint &&
            NtpTime.IsSynchronized())
        {
            Debug.Log($"<color=#fff929>NtpTime Now: {NtpTime.GetNow()}</color>");
            Debug.Log($"<color=#fff929>NtpTime UTC Now: {NtpTime.GetUtcNow()}</color>");
        }
        #endregion

        #region Timer
        // Drive deltaTimer
        this._deltaTimer.UpdateTimer(Time.deltaTime);

        if (Keyboard.current.zKey.wasReleasedThisFrame)
        {
            // RealTimer
            this._realTimer.SetTimer(this.waitTime);
            this._realTimer.Play();

            // DeltaTimer
            this._deltaTimer.SetTimer(this.waitTime);
            this._deltaTimer.Play();

            Debug.Log($"<color=#ffe400>Timer start! Current waitTime is <color=#ff5aaf>{this.waitTime} seconds</color>.</color>");
        }

        // Detecting realTimer
        if (this._realTimer.IsPlaying())
        {
            if (this._realTimer.IsTimerTimeout())
            {
                this._realTimer.Stop();
                Debug.Log($"<color=#a1ff00>[RealTimer] Time’s up and timer stop!!!</color>");
            }
        }

        // Detecting deltaTimer
        if (this._deltaTimer.IsPlaying())
        {
            if (this._deltaTimer.IsTimerTimeout())
            {
                this._deltaTimer.Stop();
                Debug.Log($"<color=#00e3ff>[DeltaTimer] Time’s up and timer stop!!!</color>");
            }
        }
        #endregion

        #region Ticker
        // Drive deltaTicker
        this._deltaTicker.UpdateTimer(Time.deltaTime);

        if (Keyboard.current.xKey.wasReleasedThisFrame)
        {
            this._isTickerRuning = !this._isTickerRuning;

            if (this._isTickerRuning)
            {
                // RealTicker
                this._realTicker.SetTick(this.tickTime);
                this._realTicker.Play();

                // DeltaTicker
                this._deltaTicker.SetTick(this.tickTime);
                this._deltaTicker.Play();

                Debug.Log($"<color=#ffe400>Ticker start! Current tickTime is <color=#ff5aaf>{this.tickTime} seconds</color>.</color>");
            }
            else
            {
                this._realTicker.Stop();
                this._deltaTicker.Stop();

                Debug.Log($"<color=#ffe400>Ticker stop!</color>");
            }
        }

        // Detecting realTicker
        if (this._realTicker.IsPlaying())
        {
            if (this._realTicker.IsTickTimeout())
            {
                Debug.Log($"<color=#a1ff00>[RealTicker] Tick check point!!!</color>");
            }
        }

        // Detecting deltaTicker
        if (this._deltaTicker.IsPlaying())
        {
            if (this._deltaTicker.IsTickTimeout())
            {
                Debug.Log($"<color=#00e3ff>[DeltaTicker] Tick check point!!!</color>");
            }
        }
        #endregion

        #region Updater
        if (Keyboard.current.cKey.wasReleasedThisFrame)
        {
            this._isUpdaterRuning = !this._isUpdaterRuning;
            if (this._isUpdaterRuning)
            {
                // Start updater
                this._rtUpdater.Start();
                this._dtUpdater.Start();

                Debug.Log($"<color=#ffe400>Updater start!</color>");
            }
            else
            {
                // Stop updater
                this._rtUpdater.Stop();
                this._dtUpdater.Stop();

                Debug.Log($"<color=#ffe400>Updater stop!</color>");
            }
        }
        #endregion

        #region IntervalTimer
        if (Keyboard.current.vKey.wasReleasedThisFrame)
        {
            this._isIntervalTimerRuning = !this._isIntervalTimerRuning;
            if (this._isIntervalTimerRuning)
            {
                // New intervalTimer
                this._intervalTimer.SetInterval(() => { Debug.Log($"<color=#a1ff00>[IntervalTimer] called!!!</color>"); }, this.intervalTime * 1000);

                // Created by IntervalSetter
                IntervalSetter.SetInterval(_intervalTimerId, () => { Debug.Log($"<color=#00e3ff>[IntervalTimer] Id: <{_intervalTimerId}> called!!! Created by IntervalSetter.</color>"); }, this.intervalTime * 1000);

                Debug.Log($"<color=#ffe400>IntervalTimer start! Current intervalTime is <color=#ff5aaf>{this.intervalTime} seconds</color>.</color>");
            }
            else
            {
                // New intervalTimer
                this._intervalTimer.StopInterval();

                // Created by IntervalSetter
                IntervalSetter.TryClearInterval(_intervalTimerId);

                Debug.Log($"<color=#ffe400>IntervalTimer stop!</color>");
            }
        }
        #endregion
    }

    private void _OnRTUpdater(float deltaTime)
    {
        Debug.Log($"<color=#a1ff00>[RTUpdater] DeltaTime: {deltaTime}</color>");
    }

    private void _OnDTUpdater(float deltaTime)
    {
        Debug.Log($"<color=#00e3ff>[DTUpdater] DeltaTime: {deltaTime}</color>");
    }
}
