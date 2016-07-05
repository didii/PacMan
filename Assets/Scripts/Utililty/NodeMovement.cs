using System;
using System.Linq;
using UnityEngine;

public class NodeMovement : MonoBehaviour {

    #region Fields
    
    public float Speed;
    public bool AllowReverse = true;

    public event Action<Utility.EDirection4> DirectionChanged;

    private Utility.EDirection4 _currDirection = Utility.EDirection4.None;
    private Utility.EDirection4 _nextDirection = Utility.EDirection4.None;
    private Utility.EDirection4[] _allowedDirections = null;

    private Rigidbody2D _rigidbody2D;
    #endregion

    #region Properties

    public bool Pauzed {
        get { return _allowedDirections != null; }
    }

    /// <summary>
    /// Get the current direction of movement. Set to force a direction.
    /// </summary>
    public Utility.EDirection4 CurrentDirection {
        get { return _currDirection; }
        set {
            // Issue the new direction
            switch (value) {
                case Utility.EDirection4.None:
                    _rigidbody2D.velocity = Vector2.zero;
                    break;
                case Utility.EDirection4.Up:
                    _rigidbody2D.velocity = Vector2.up * Speed;
                    break;
                case Utility.EDirection4.Right:
                    _rigidbody2D.velocity = Vector2.right * Speed;
                    break;
                case Utility.EDirection4.Down:
                    _rigidbody2D.velocity = Vector2.down * Speed;
                    break;
                case Utility.EDirection4.Left:
                    _rigidbody2D.velocity = Vector2.left * Speed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            // Invoke event
            if (DirectionChanged != null && value != CurrentDirection)
                DirectionChanged.Invoke(value);
            // Set the new direction
            _currDirection = value;
        }
    }

    /// <summary>
    /// Gets or sets the next direction of movement. CurrentDirection will be updated if allowed.
    /// 
    /// <remarks>
    /// <para> <see cref="CurrentDirection"/> is immediately updated if <see cref="Wait"/> was not recently used and
    /// <list type="bullet">
    /// <item><see cref="CurrentDirection"/> is set to <see cref="Utility.EDirection4.None"/></item>
    /// <item><paramref name="value"/> is the opposite of <see cref="CurrentDirection"/> when
    /// <see cref="AllowReverse"/> was specified</item>
    /// </list>
    /// In all other cases when <see cref="Wait"/> was not recently used, <paramref name="value"/> is discarded if it
    /// is the same as <see cref="CurrentDirection"/> and stored if not.</para>
    /// 
    /// <para>If <see cref="Wait"/> was recently used, <see cref="CurrentDirection"/> will always immeidately update if
    /// the direction is allowed.</para>
    /// </remarks>
    /// </summary>
    public Utility.EDirection4 NextDirection {
        get { return _nextDirection; }
        set {
            if (Pauzed) {
                _nextDirection = value;
                return;
            }
            // If allowed directions was set
            if (_allowedDirections != null) {
                if (_allowedDirections.Contains(value)) {
                    CurrentDirection = value;
                    _nextDirection = Utility.EDirection4.None;
                    _allowedDirections = null;
                }
                return;
            }
            // Check if CurrentDirection is allowed to change to value immediately
            if (CurrentDirection == Utility.EDirection4.None || (AllowReverse && value.IsOpposite(CurrentDirection))) {
                CurrentDirection = value;
                _nextDirection = Utility.EDirection4.None;
            } // If same direction was pressed as CurrentDirection
            else if (value == CurrentDirection)
                _nextDirection = Utility.EDirection4.None;
            // If direction may not be applied immediately
            else
                _nextDirection = value; // store it
        }
    }

    #endregion

    #region Initialisation Methods

    // Use this for initialization
    void Start() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    #endregion

    #region Runtime Methods

    // Update is called once per frame
    void Update() {}

    #endregion

    #region Events

    #endregion

    #region Methods

    public void Wait() {
        if (AllowReverse)
            _allowedDirections = new Utility.EDirection4[2];
        else
            _allowedDirections = new Utility.EDirection4[1];
        _allowedDirections[0] = CurrentDirection;
        if (AllowReverse)
            _allowedDirections[1] = CurrentDirection.Opposite();
        CurrentDirection = Utility.EDirection4.None;
    }

    public void Resume() {
        if (_allowedDirections == null) return;
        CurrentDirection = _allowedDirections[0];
        _allowedDirections = null;

        if (AllowReverse && _nextDirection.IsOpposite(_currDirection))
            CurrentDirection = _nextDirection;
    }
    #endregion

    #region Helper Methods

    #endregion
}
