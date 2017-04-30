using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// General info about the game!
/// </summary>
public class GameInfo : MonoBehaviour {

    [Header("Game Info")]
    public int Score = 0;
    public int Lives = 3;

    [Header("UI Elements")]
    public Text ScoreText;

	// Update is called once per frame
	void Update () {
	    ScoreText.text = Score.ToString();
	}
}
