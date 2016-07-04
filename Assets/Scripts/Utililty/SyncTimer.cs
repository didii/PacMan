using System;
using UnityEngine;

class SyncTimer {
    #region Fields

    private bool _enabled = false;
    private bool _initialRaise = false;

    private float _startTime;
    private float _intervalTime;
    private float _pauseTime;

    private bool _repeat = true;
    private int _repeatCount;
    private int _repeatCountMax;

    private bool _ignoreMultiStart = true;
    #endregion

    #region Properties
    /// <summary>
    /// Enable or disable the raising of <see cref="Elapsed"/>.
    /// </summary>
    public bool Enabled {
        get { return _enabled; }
        set {
            if (value)
                Start();
            else
                Stop();
        }
    }

    /// <summary>
    /// How much time should be between the event calls.
    /// </summary>
    public float IntervalTime {
        get { return _intervalTime; }
        set { _intervalTime = value; }
    }

    /// <summary>
    /// Set true to restart the timer after <see cref="Elapsed"/> has been triggered.
    /// </summary>
    /// <remarks>When set to false and <see cref="InitialRaise"/> set to true, the event will fire twice. Once when <see cref="Start"/> was called and once after <see cref="IntervalTime"/> seconds.</remarks>
    public bool Repeat {
        get { return _repeat; }
        set { _repeat = value; }
    }

    /// <summary>
    /// How many times should <see cref="Elapsed"/> be raised.
    /// </summary>
    public int RepeatCountMax {
        get { return _repeatCountMax; }
        set { _repeatCountMax = value; }
    }

    /// <summary>
    /// Set to true to immediately raise <see cref="Elapsed"/> on <see cref="Start"/>.
    /// </summary>
    public bool InitialRaise {
        get { return _initialRaise; }
        set { _initialRaise = value; }
    }

    /// <summary>
    /// Ignore a call to <see cref="Start"/> when the timer was already started. If not, the timer
    /// is reset every time when called.
    /// </summary>
    public bool IgnoreMultipleStart {
        get { return _ignoreMultiStart; }
        set { _ignoreMultiStart = value; }
    }

    #endregion

    #region Events
    /// <summary>
    /// The event raised every <see cref="IntervalTime"/> seconds.
    /// </summary>
    public event System.Action Elapsed;
    #endregion

    #region Constructor

    #endregion

    #region Methods

    /// <summary>
    /// Resets and starts the timer by raising the <see cref="Elapsed"/> event every
    /// <see cref="IntervalTime"/> seconds for <see cref="RepeatCount"/> times.
    /// </summary>
    /// <remarks>Set <see cref="RepeatCount"/> = 0 to repeat the event indefinately.</remarks>
    /// <seealso cref="IgnoreMultipleStart"/> <seealso cref="RepeatCount"/> <seealso cref="InitialRaise"/>
    public void Start() {
        if (_enabled && _ignoreMultiStart)
            return;

        if (Elapsed == null)
            throw new ArgumentNullException("Elapsed", "The event Elapsed cannot be null when starting the timer.");
        _enabled = true;
        _startTime = Time.time;
        _repeatCount = 0;
        if (_initialRaise)
            Elapsed();
    }

    /// <summary>
    /// Stops raising the <see cref="Elapsed"/> event.
    /// </summary>
    public void Stop() {
        _enabled = false;
    }

    /// <summary>
    /// Pauzes the timer. Resume it with <see cref="Resume"/>.
    /// </summary>
    public void Pause() {
        _pauseTime = Time.time;
    }

    /// <summary>
    /// Resumes the timer. Does not reset any setting.
    /// </summary>
    public void Resume() {
        _startTime += Time.time - _pauseTime;
    }

    /// <summary>
    /// Update the timer. This will call <see cref="Elapsed"/> when appropriate and should be
    /// called every frame.
    /// </summary>
    public void Update() {
        // Check if Elapsed can be raised (enabled, repeats left, time)
        if (_enabled && (_repeatCountMax <= 0 || _repeatCount < _repeatCountMax) && Time.time > _startTime+_intervalTime) {
            // Raise the event (cannot be null)
            Elapsed();
            // If no repeat, stop the timer and bail
            if (!_repeat) {
                Stop();
                return;
            }

            // Reset timer
            _startTime = Time.time;
            // Increment repeat
            if (_repeatCountMax > 0)
                _repeatCount++;
        }
    }

    #endregion
}
