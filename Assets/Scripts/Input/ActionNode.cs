using System;
using System.ComponentModel;
using UnityEngine;

/// <summary>
///     Abstract Action type. Allows to combine multiple keypresses in a single command
/// </summary>
abstract class ActionNode {
    /// <summary>
    ///     Is the current node active?
    /// </summary>
    /// <returns></returns>
    public abstract bool IsActive();

    /// <summary>
    ///     Forces a check to update the IsActive state
    /// </summary>
    public abstract void ForceUpdate();
}

/// <summary>
///     The default node. Allows for different action types and multiplier (double, triple etc. presses)
/// </summary>
class ActionNodeManager : ActionNode {
    #region Fields
    private ActionNode _node;
    #endregion

    #region Constructor
    public ActionNodeManager(KeyCode key, Action.EActionType type = Action.EActionType.Hold, int multi = 2) {
        // Create the correct ActionNode type depending on the EActionType given
        switch (type) {
        case Action.EActionType.Hold:
        case Action.EActionType.PressOnce:
        case Action.EActionType.ReleaseOnce:
            _node = new SimpleActionNode(key, type);
            break;
        case Action.EActionType.HoldTwice:
        case Action.EActionType.MultiHold:
            _node = new MultiHoldActionNode(key, multi);
            break;
        case Action.EActionType.PressTwice:
        case Action.EActionType.MultiPress:
            _node = new MultiPressActionNode(key, multi);
            break;
        case Action.EActionType.ReleaseTwice:
        case Action.EActionType.MultiRelease:
            _node = new MultiReleaseActionNode(key, multi);
            break;
        default:
            throw new ArgumentOutOfRangeException("type");
        }
    }
    #endregion

    #region Methods
    public override bool IsActive() {
        return _node.IsActive();
    }

    public override void ForceUpdate() { }
    #endregion

    #region Subclasses
    /// <summary>
    ///     Defines a single press/hold/release action
    /// </summary>
    private class SimpleActionNode : ActionNode {
        private KeyCode _key;
        private Action.EActionType _type;

        public SimpleActionNode(KeyCode key, Action.EActionType type) {
            // If a multi-press was given, throw an exception
            if ((int)type >= 3)
                throw new InvalidEnumArgumentException("type");
            _key = key;
            _type = type;
        }

        public override bool IsActive() {
            switch (_type) {
            case Action.EActionType.Hold:
                return Keyboard.IsKeyDown(_key);
            case Action.EActionType.PressOnce:
                return Keyboard.IsKeyPressed(_key);
            case Action.EActionType.ReleaseOnce:
                return Keyboard.IsKeyReleased(_key);
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        public override void ForceUpdate() { }
    }

    /// <summary>
    ///     Defines a multi-press action node (double, triple, etc. press). Abstract class
    /// </summary>
    private abstract class MultiActionNode : ActionNode {
        #region Fields
        /// <summary>
        ///     The key
        /// </summary>
        protected KeyCode _key;

        /// <summary>
        ///     Number of times key has to be pressed
        /// </summary>
        protected int _multiTarget;

        /// <summary>
        ///     Current amount of presses
        /// </summary>
        protected int _multiCurrent;

        /// <summary>
        ///     Time of first click (-1 is never)
        /// </summary>
        protected float _startTime = -1;

        /// <summary>
        ///     If IsActive() should return true
        /// </summary>
        protected bool _isActive;

        /// <summary>
        ///     Last frame that <see cref="IsActive"/> was called
        /// </summary>
        private int _lastFrameUpdate;
        #endregion

        #region Properties
        /// <summary>
        ///     Time that have passed since the first click
        /// </summary>
        protected TimeSpan TimeSinceFirstClick {
            get { return TimeSpan.FromSeconds(Time.time - _startTime); }
        }
        #endregion

        #region Constructor
        /// <summary>
        ///     Immediately set the key and target number of presses
        /// </summary>
        /// <param name="key"></param>
        /// <param name="multiTarget"></param>
        public MultiActionNode(KeyCode key, int multiTarget = 2) {
            _key = key;
            _multiTarget = multiTarget;
        }
        #endregion

        #region Methods
        public override bool IsActive() {
            if (_lastFrameUpdate != Time.frameCount) {
                Update();
                _lastFrameUpdate = Time.frameCount;
            }
            return _isActive;
        }

        public override void ForceUpdate() {
            Update();
        }

        /// <summary>
        ///     Update the <see cref="IsActive"/> state, is called once per frame
        /// </summary>
        protected void Update() {
            // Reset if timer has passed
            if (_startTime < 0 || TimeSinceFirstClick > Action.MultiDelayTime.Multiply(_multiTarget))
                Reset();

            // Do every frame
            UpdateFrame();

            // When key is pressed
            if (Keyboard.IsKeyPressed(_key))
                Next();

            // Do every frame after everything else
            LateUpdateFrame();
        }

        /// <summary>
        ///     Do every frame (before <see cref="Next"/>)
        /// </summary>
        protected virtual void UpdateFrame() { }

        /// <summary>
        ///     Do when the key is pressed
        /// </summary>
        protected virtual void Next() {
            if (_multiCurrent == 0)
                _startTime = Time.time;
            _multiCurrent++;
        }

        /// <summary>
        ///     Do every frame (after <see cref="Next"/>)
        /// </summary>
        protected virtual void LateUpdateFrame() { }

        /// <summary>
        ///     Do if timer has passed (before anything else)
        /// </summary>
        protected virtual void Reset() {
            _startTime = -1;
            _multiCurrent = 0;
        }
        #endregion
    }

    /// <summary>
    ///     Multi-press, then hold. Stays active until the user releases the button
    /// </summary>
    private class MultiHoldActionNode : MultiActionNode {
        #region Constructor
        /// <summary>
        ///     Immediately set the key and target number of presses
        /// </summary>
        /// <param name="key"></param>
        /// <param name="multiTarget"></param>
        public MultiHoldActionNode(KeyCode key, int multiTarget) : base(key, multiTarget) { }
        #endregion

        #region Methods
        protected override void Next() {
            base.Next();
            if (_multiCurrent == _multiTarget)
                _isActive = true;
        }

        protected override void UpdateFrame() {
            base.UpdateFrame();
            if (Keyboard.IsKeyReleased(_key))
                _isActive = false;
        }
        #endregion
    }

    /// <summary>
    ///     Multi-press. Is active in the frame that the user pressed the button x-times.
    /// </summary>
    private class MultiPressActionNode : MultiActionNode {
        private int _frameActive;

        public MultiPressActionNode(KeyCode key, int multiTarget) : base(key, multiTarget) { }

        #region Methods
        protected override void UpdateFrame() {
            base.UpdateFrame();
            _isActive = false;
            if (_frameActive + 1 == Time.frameCount)
                _multiCurrent = 0;
        }

        protected override void Next() {
            base.Next();
            if (_multiCurrent == _multiTarget) {
                _isActive = true;
                _frameActive = Time.frameCount;
            }
        }

        protected override void Reset() {
            base.Reset();
            _isActive = false;
        }
        #endregion
    }

    /// <summary>
    ///     Multi-press, then release. Is active in the frame that the user released the button x-times.
    /// </summary>
    private class MultiReleaseActionNode : MultiActionNode {
        private bool _isHolding;
        private int _frameActive;

        public MultiReleaseActionNode(KeyCode key, int multiTarget) : base(key, multiTarget) { }

        #region Methods
        protected override void Next() {
            base.Next();
            if (_multiCurrent == _multiTarget) {
                _isHolding = true;
            }
        }

        protected override void UpdateFrame() {
            base.UpdateFrame();
            if (_frameActive + 1 == Time.frameCount)
                _multiCurrent = 0;
            if (_isHolding && Keyboard.IsKeyReleased(_key)) {
                _isHolding = false;
                _isActive = true;
                _frameActive = Time.frameCount;
            } else
                _isActive = false;
        }
        #endregion
    }
    #endregion
}

/// <summary>
///     Chains two actions that both need to be active
/// </summary>
class AndActionNode : ActionNode {
    private ActionNode _lhs, _rhs;

    public AndActionNode(ActionNode lhs, ActionNode rhs) {
        _lhs = lhs;
        _rhs = rhs;
    }

    public override bool IsActive() {
        return _lhs.IsActive() && _rhs.IsActive();
    }

    public override void ForceUpdate() { }
}

/// <summary>
///     Chains two actions where one of the two needs to be active
/// </summary>
class OrActionNode : ActionNode {
    private ActionNode _lhs, _rhs;

    public OrActionNode(ActionNode lhs, ActionNode rhs) {
        _lhs = lhs;
        _rhs = rhs;
    }

    public override bool IsActive() {
        return _lhs.IsActive() || _rhs.IsActive();
    }

    public override void ForceUpdate() { }
}

/// <summary>
///     Inverts an action
/// </summary>
class NotActionNode : ActionNode {
    private ActionNode _node;

    public NotActionNode(ActionNode node) {
        _node = node;
    }

    public override bool IsActive() {
        return !_node.IsActive();
    }

    public override void ForceUpdate() { }
}