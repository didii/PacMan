using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour {

    #region Fields

    public static int FixedFrameCount = 0;
    [Header("Player"),Range(0,3)]
    public int PlayerLives;

    [Header("UI")]
    public GameObject BottomUI;
    public GameObject LivesIndicatorPrefab;
    [Range(0, 100)]
    public float LivesIndicatorWidth;
    public GameObject FruitIndicatorPrefab;
    [Range(0, 100)]
    public float FruitIndicatorWidth;
    public Texture2D FruitTexture;

    private Sprite[] _fruitSprites;
    private List<GameObject> _livesIndicators;
    private List<GameObject> _fruitIndicators;
    #endregion

    #region Properties

    #endregion

    // Use this for initialization
    void Start () {
        //_fruitSprites = Resources.LoadAll<Sprite>(FruitTexture.name);
        if (_livesIndicators == null)
            _livesIndicators = new List<GameObject>();
        if (_fruitIndicators == null)
            _fruitIndicators = new List<GameObject>();
        _livesIndicators.AddRange(
            BottomUI.transform.GetAllChildren(
                child => child.tag == "LifeIndicator" && !_livesIndicators.Contains(child.gameObject))
                    .Select(child => child.gameObject));
        SetLivesIndicator();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    // Fixed update is called once per fixed interval
    void FixedUpdate() {
        FixedFrameCount++;
    }

    #region Helper methods

    public void SetLivesIndicator() {
#if UNITY_EDITOR
        if (_livesIndicators == null)
            _livesIndicators = new List<GameObject>();
        // Make sure no life indicators already exist not included in the list
        _livesIndicators.AddRange(
            BottomUI.transform.GetAllChildren(
                child => child.tag == "LifeIndicator" && !_livesIndicators.Contains(child.gameObject))
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
                new Vector2((_livesIndicators.Count - 1)*LivesIndicatorWidth, 0);
        }
    }
#endregion


    public void DoRandomStuff() {
        var line1 = new LineSegment2D(new Vector2(0, 0), Vector2.right);
        var line2 = new LineSegment2D(Vector2.zero, 2*Vector2.right);
        var line3 = new LineSegment2D(Vector2.zero, 2*Vector2.right);
        var line4 = new LineSegment2D(Vector2.zero, 3*Vector2.up);

        var lines = new List<LineSegment2D> {
            line2, line4, line1, line3
        };
        Debug.Log("(" + lines[0].Length + "," + lines[1].Length + "," + lines[2].Length + "," + lines[3].Length + ")");
        var lines2 = lines.OrderBy(line => line.End.magnitude).ToArray();
        Debug.Log("(" + lines2[0].Length + "," + lines2[1].Length + "," + lines2[2].Length + "," + lines2[3].Length + ")");
    }
}
