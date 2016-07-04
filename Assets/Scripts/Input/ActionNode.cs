using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.Director;

abstract class ActionNode {
    public abstract bool IsActive();
    public abstract void ForceUpdate();
}

class ActionNodeManager : ActionNode {
    private ActionNode _node;

    public ActionNodeManager(KeyCode key, Action.EActionType type =Action.EActionType.Hold, int multi =2) {
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

    public override bool IsActive() {
        return _node.IsActive();
    }

    public override void ForceUpdate() {}

    #region Subclasses
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

        public override void ForceUpdate() {}
    }

    private abstract class MultiActionNode : ActionNode {
        #region Fields
        protected KeyCode _key; // The key
        protected int _multiTarget = 2; // Number of times key has to be pressed
        protected int _multiCurrent = 0; // Current amount of presses
        protected float _startTime = -1; // Time of first click
        protected bool _isActive; // If IsActive() should return true

        private int _lastFrameUpdate; // Last frame IsActive() was called
        #endregion

        #region Properties
        // Time that have passed since the first click
        protected TimeSpan TimeSinceFirstClick {
            get { return TimeSpan.FromSeconds(Time.time - _startTime); }
        }
        #endregion

        #region Constructor
        // Immediately set the key and target number of presses
        public MultiActionNode(KeyCode key, int multiTarget =2) {
            _key = key;
            _multiTarget = multiTarget;
        }
        #endregion

        #region Methods
        // If action is currently active
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

        // Update the _isActive state, is called once per frame
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

        // Do every frame (before Next())
        protected virtual void UpdateFrame() {}
        // Do when the key is pressed
        protected virtual void Next() {
            if (_multiCurrent == 0)
                _startTime = Time.time;
            _multiCurrent++;
        }
        // Do every frame (after Next())
        protected virtual void LateUpdateFrame() {}
        // Do if timer has passed (before anything else)
        protected virtual void Reset() {
            _startTime = -1;
            _multiCurrent = 0;
        }
        #endregion
    }

    private class MultiHoldActionNode : MultiActionNode {

        public MultiHoldActionNode(KeyCode key, int multiTarget)
            : base(key, multiTarget) {}

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

    private class MultiPressActionNode : MultiActionNode {
        private int _frameActive;

        public MultiPressActionNode(KeyCode key, int multiTarget)
            : base(key, multiTarget) {}

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

    private class MultiReleaseActionNode : MultiActionNode {
        private bool _isHolding;
        private int _frameActive;

        public MultiReleaseActionNode(KeyCode key, int multiTarget)
            : base(key, multiTarget) {}

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


class AndActionNode : ActionNode {
    private ActionNode _lhs, _rhs;

    public AndActionNode(ActionNode lhs, ActionNode rhs) {
        _lhs = lhs;
        _rhs = rhs;
    }

    public override bool IsActive() {
        return _lhs.IsActive() && _rhs.IsActive();
    }

    public override void ForceUpdate() {}
}

class OrActionNode : ActionNode {
    private ActionNode _lhs, _rhs;

    public OrActionNode(ActionNode lhs, ActionNode rhs) {
        _lhs = lhs;
        _rhs = rhs;
    }

    public override bool IsActive() {
        return _lhs.IsActive() || _rhs.IsActive();
    }
    public override void ForceUpdate() {}
}

class NotActionNode : ActionNode {
    private ActionNode _node;

    public NotActionNode(ActionNode node) {
        _node = node;
    }

    public override bool IsActive() {
        return !_node.IsActive();
    }
    public override void ForceUpdate() {}
}