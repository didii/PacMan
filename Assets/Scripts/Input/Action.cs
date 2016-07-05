using System;
using UnityEngine;
using System.Collections;

public class Action {

    #region Structs/Enums
    public enum EActionType {
        Hold, PressOnce, ReleaseOnce, HoldTwice, PressTwice, ReleaseTwice, MultiHold, MultiPress, MultiRelease
    }
    #endregion

    #region Fields
    // Maximum time between 2 presses before it's considered a multi-press
    public static TimeSpan MultiDelayTime = TimeSpan.FromMilliseconds(200);
    private ActionNode _node;
    #endregion

    #region Properties

    #endregion

    #region Constructor

    public Action(KeyCode key, EActionType type = EActionType.Hold, int multi =0) {
        if ((int) type >= 3 && multi <= 1)
            multi = 2;

        _node = new ActionNodeManager(key, type, multi);
    }

    private Action(ActionNode node) {
        _node = node;
    }
    #endregion

    #region Methods

    public bool IsActive() {
        return _node.IsActive();
    }
    #endregion

    #region Operators

    static public Action operator &(Action lhs, Action rhs) {
        return new Action(new AndActionNode(lhs._node, rhs._node));
    }

    static public Action operator |(Action lhs, Action rhs) {
        return new Action(new OrActionNode(lhs._node, rhs._node));
    }

    static public Action operator !(Action rhs) {
        return new Action(new NotActionNode(rhs._node));
    }
    #endregion
}
