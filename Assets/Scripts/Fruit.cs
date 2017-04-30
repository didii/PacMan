using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Fruit : MonoBehaviour {
    /// <summary>
    ///     Different types of fruit that can spawn
    /// </summary>
    [Flags]
    public enum EFruit {
        None = 0,
        GreenApple = 1,
        RedApple = 2,
        Banana = 4,
        Berry = 8,
        BlackBerry = 16,
        Cherry = 32,
        Coconut = 64,
        Kiwi = 128,
        Melon = 256,
        Orange = 512,
        Pear = 1024,
        Pineapple = 2048,
        Pink = 4096,
        Strawberry = 8192,
        Watermelon = 16384,

        All = GreenApple +
              RedApple +
              Banana +
              Berry +
              BlackBerry +
              Cherry +
              Coconut +
              Kiwi +
              Melon +
              Orange +
              Pear +
              Pineapple +
              Pink +
              Strawberry +
              Watermelon
    }

    /// <summary>
    ///     How long should the fruit be flickering before disappearing
    /// </summary>
    public float FlickerDuration = 3f;

    /// <summary>
    ///     How quick should the fruit flicker
    /// </summary>
    public float FlickerSpeed = 0.3f;

    /// <summary>
    ///     How long does the fruit live
    /// </summary>
    public float LifeDuration = 10f;

    private bool _isFlickering;
    private bool _isDestroyed;
    private float _flickerTime, _disappearTime;
    private bool _timeSet = false;

    private SyncTimer _flickerTimer = new SyncTimer();

    private SpriteRenderer _spriteRenderer;

    #region Unity methods
    /// <summary>
    ///     Use this for initialization
    /// </summary>
    void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_flickerTime == 0 && _disappearTime == 0) {
            SetTime(Time.time + LifeDuration);
        }
        _flickerTimer.IntervalTime = FlickerSpeed;
        _flickerTimer.Elapsed += () => _spriteRenderer.enabled = !_spriteRenderer.enabled;
    }

    /// <summary>
    ///     Update is called once per frame
    /// </summary>
    void Update() {
        if (Time.time > _flickerTime)
            StartFlicker();
        if (Time.time > _disappearTime)
            Disappear();
        _flickerTimer.Update();
    }
    #endregion

    #region Methods
    /// <summary>
    ///     Set the times to correctly start flickering and disappearing
    /// </summary>
    /// <param name="disappearTime"></param>
    public void SetTime(float disappearTime) {
        if (_timeSet)
            return;
        _disappearTime = disappearTime;
        _flickerTime = _disappearTime - FlickerDuration;
    }
    #endregion

    #region Helper methods
    /// <summary>
    ///     Start the flickering
    /// </summary>
    private void StartFlicker() {
        if (_isFlickering)
            return;
        _isFlickering = true;
        _flickerTimer.Start();
    }

    /// <summary>
    ///     Disappear from the scene
    /// </summary>
    private void Disappear() {
        if (_isDestroyed)
            return;
        _isDestroyed = true;
        Destroy(gameObject);
    }
    #endregion
}
