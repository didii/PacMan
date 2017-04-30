using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour {
    #region Fields
    /// <summary>
    ///     Counts the number of fixed frames that have passed. Used to ensure collisions aren't triggered twice
    /// </summary>
    public static int FixedFrameCount;

    /// <summary>
    ///     Amount of lives the player still has
    /// </summary>
    [Header("Player"), Range(0, 3)]
    public int PlayerLives;

    /// <summary>
    ///     Bottom part of the UI
    /// </summary>
    [Header("UI")]
    public GameObject BottomUI;

    /// <summary>
    ///     The amount of lives shown in the left-lower corner
    /// </summary>
    public GameObject LivesIndicatorPrefab;

    /// <summary>
    ///     How wide the lives indicator should be
    /// </summary>
    [Range(0, 100)]
    public float LivesIndicatorWidth;

    /// <summary>
    ///     A reference to the fruit indicator prefab
    /// </summary>
    public GameObject FruitIndicatorPrefab;

    /// <summary>
    ///     How wide the fruit indicator should be
    /// </summary>
    [Range(0, 100)]
    public float FruitIndicatorWidth;

    /// <summary>
    ///     The texture pack of the fruit
    /// </summary>
    public Texture2D FruitTexture;

    /// <summary>
    ///     All of the individual fruit sprites, taken from <see cref="FruitTexture"/>
    /// </summary>
    private Sprite[] _fruitSprites;

    /// <summary>
    ///     Points to all the life indicator objects
    /// </summary>
    private List<GameObject> _livesIndicators;

    /// <summary>
    ///     Points to all the fruit indicator objects
    /// </summary>
    private List<GameObject> _fruitIndicators;
    #endregion

    /// <summary>
    ///     Use this for initialization
    /// </summary>
    void Start() {
        //_fruitSprites = Resources.LoadAll<Sprite>(FruitTexture.name);
        if (_livesIndicators == null)
            _livesIndicators = new List<GameObject>();
        if (_fruitIndicators == null)
            _fruitIndicators = new List<GameObject>();
        _livesIndicators.AddRange(BottomUI.transform
                                          .GetAllChildren(child => child.tag == "LifeIndicator" &&
                                                                   !_livesIndicators.Contains(child.gameObject))
                                          .Select(child => child.gameObject));
        SetLivesIndicator();
    }

    /// <summary>
    ///     FixedUpdate is called once every physics update
    /// </summary>
    void FixedUpdate() {
        FixedFrameCount++;
    }

    #region Methods
    /// <summary>
    ///     Sets the correct amount of lives based on <see cref="PlayerLives"/>.
    /// </summary>
    public void SetLivesIndicator() {
#if UNITY_EDITOR
        if (_livesIndicators == null)
            _livesIndicators = new List<GameObject>();
        // Make sure no life indicators already exist not included in the list
        _livesIndicators.AddRange(BottomUI.transform
                                          .GetAllChildren(child => child.tag == "LifeIndicator" &&
                                                                   !_livesIndicators.Contains(child.gameObject))
                                          .Select(child => child.gameObject));
#endif

        // Remove all life indicators
        while (_livesIndicators.Count > 0) {
#if UNITY_EDITOR
            DestroyImmediate(_livesIndicators.Last());
#else
            Destroy(_livesIndicators.Last());
#endif
            _livesIndicators.RemoveLast();
        }

        // Add them all back
        while (PlayerLives > _livesIndicators.Count) {
            _livesIndicators.Add(Instantiate(LivesIndicatorPrefab));
            _livesIndicators.Last().transform.SetParent(BottomUI.transform, false);
            _livesIndicators.Last().GetComponent<RectTransform>().anchoredPosition =
                new Vector2((_livesIndicators.Count - 1) * LivesIndicatorWidth, 0);
        }
    }

    /// <summary>
    ///     Does some stuff I need right now
    /// </summary>
    public void DoRandomStuff() {
        var line1 = new LineSegment2D(new Vector2(0, 0), Vector2.right);
        var line2 = new LineSegment2D(Vector2.zero, 2 * Vector2.right);
        var line3 = new LineSegment2D(Vector2.zero, 2 * Vector2.right);
        var line4 = new LineSegment2D(Vector2.zero, 3 * Vector2.up);

        var lines = new List<LineSegment2D> {line2, line4, line1, line3};
        Debug.Log("(" + lines[0].Length + "," + lines[1].Length + "," + lines[2].Length + "," + lines[3].Length + ")");
        var lines2 = lines.OrderBy(line => line.End.magnitude).ToArray();
        Debug.Log("(" +
                  lines2[0].Length +
                  "," +
                  lines2[1].Length +
                  "," +
                  lines2[2].Length +
                  "," +
                  lines2[3].Length +
                  ")");
    }
    #endregion
}
