using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Fruit : MonoBehaviour {

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

        All =
            GreenApple + RedApple + Banana + Berry + BlackBerry + Cherry + Coconut + Kiwi + Melon + Orange + Pear +
            Pineapple + Pink + Strawberry + Watermelon
    }

    public float FlickerDuration = 3f;
    public float FlickerSpeed = 0.3f;
    public float LifeDuration = 10f;

    private bool _isFlickering = false;
    private bool _isDestroyed = false;
    private float _flickerTime, _disappearTime;
    private bool _timeSet = false;

    private SyncTimer _flickerTimer = new SyncTimer();

    private SpriteRenderer _spriteRenderer;

    #region Unity methods
    // Use this for initialization
    void Start () {
	    _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_flickerTime == 0 && _disappearTime == 0) {
            SetTime(Time.time+LifeDuration);
        }
        _flickerTimer.IntervalTime = FlickerSpeed;
        _flickerTimer.Elapsed += () => _spriteRenderer.enabled = !_spriteRenderer.enabled;
    }
	
	// Update is called once per frame
	void Update () {
	    if (Time.time > _flickerTime)
	        StartFlicker();
	    if (Time.time > _disappearTime)
            Disappear();
        _flickerTimer.Update();
    }
    #endregion

    #region Methods

    public void SetTime(float disappearTime) {
        if (_timeSet) return;
        _disappearTime = disappearTime;
        _flickerTime = _disappearTime - FlickerDuration;
    }
    #endregion

    #region Helper methods
    private void StartFlicker() {
        if (_isFlickering)
            return;
        _isFlickering = true;
        _flickerTimer.Start();
    }

    private void Disappear() {
        if (_isDestroyed)
            return;
        _isDestroyed = true;
        Destroy(gameObject);
    }
    #endregion

}
