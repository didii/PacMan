using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameInfo : MonoBehaviour {

    [Header("Game Info")]
    public int Score = 0;
    public int Lives = 3;

    [Header("UI Elements")]
    public Text ScoreText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    ScoreText.text = Score.ToString();
	}
}
