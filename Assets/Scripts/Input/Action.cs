using System;
using UnityEngine;

public class Action {
    #region Structs/Enums
    /// <summary>
    ///     The types of actions that can happen. XXXTwice is an alias for MultiXXX with multi set to 2.
    /// </summary>
    public enum EActionType {
        Hold,
        PressOnce,
        ReleaseOnce,
        HoldTwice,
        PressTwice,
        ReleaseTwice,
        MultiHold,
        MultiPress,
        MultiRelease
    }
    #endregion

    #region Fields
    /// <summary>
    ///     Maximum time between 2 presses before it's considered a multi-press
    /// </summary>
    public static TimeSpan MultiDelayTime = TimeSpan.FromMilliseconds(200);

    /// <summary>
    ///     The master node
    /// </summary>
    private ActionNode _node;
    #endregion

    #region Constructor
    /// <summary>
    ///     Default constructor taking a key, actiontype and number of presses
    /// </summary>
    /// <param name="key"></param>
    /// <param name="type"></param>
    /// <param name="multi"></param>
    public Action(KeyCode key, EActionType type = EActionType.Hold, int multi = 0) {
        if ((int)type >= 3 && multi <= 1)
            multi = 2;

        _node = new ActionNodeManager(key, type, multi);
    }

    /// <summary>
    ///     Makes a shallow copy of the action.
    /// </summary>
    /// <param name="node"></param>
    private Action(ActionNode node) {
        _node = node;
    }
    #endregion

    #region Methods
    /// <summary>
    ///     Is the action currently active?
    /// </summary>
    /// <returns></returns>
    public bool IsActive() {
        return _node.IsActive();
    }
    #endregion

    #region Operators
    /// <summary>
    ///     Chain two actions where both need to be active. Short-circuits.
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static Action operator &(Action lhs, Action rhs) {
        return new Action(new AndActionNode(lhs._node, rhs._node));
    }

    /// <summary>
    ///     Chain two actions where one of both need to be active. Short-circuits.
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static Action operator |(Action lhs, Action rhs) {
        return new Action(new OrActionNode(lhs._node, rhs._node));
    }

    /// <summary>
    ///     Inverts the action
    /// </summary>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static Action operator !(Action rhs) {
        return new Action(new NotActionNode(rhs._node));
    }
    #endregion
}
