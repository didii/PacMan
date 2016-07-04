using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class FruitSpawner : MonoBehaviour {

    public GameObject FruitPrefab;
    public Texture2D FruitTexture;

    public float SpawnTime = 20;
    public float DeltaSpawnTime = 3;
    public float Uptime = 10;
    public float DeltaUptime = 2;

    private float _spawnTime;
    private float _lastSpawnTime;
    private Fruit.EFruit _possibleSpawns = Fruit.EFruit.All;
    private Sprite[] _fruitSprites;

    private List<Fruit.EFruit> _possibleSpawnsList {
        get {
            return Enum.GetValues(typeof(Fruit.EFruit)).Cast<Fruit.EFruit>().Where(value => (_possibleSpawns & value) != 0 && value != Fruit.EFruit.All).ToList();
        }
    }

	// Use this for initialization
	void Start () {
	    _spawnTime = Time.time + SpawnTime + Random.Range(-DeltaSpawnTime, DeltaSpawnTime);
        _fruitSprites = Resources.LoadAll<Sprite>(FruitTexture.name);
    }
	
	// Update is called once per frame
	void Update () {
	    if (Time.time > _spawnTime) {
            SpawnFruit();
            ResetSpawnTimer();
        }
    }

    private void SpawnFruit() {
        if (_lastSpawnTime == _spawnTime)
            return;
        var obj = ((GameObject)Instantiate(FruitPrefab, transform.position, Quaternion.identity)).GetComponent<Fruit>();
        obj.SetTime(Time.time + Uptime + Random.Range(-DeltaUptime, DeltaUptime));
        var idx = Mathf.Log((float)_possibleSpawnsList.GetRandom(), 2);
        obj.GetComponent<SpriteRenderer>().sprite = _fruitSprites[(int)idx];
        _lastSpawnTime = _spawnTime;
    }

    private void DisappearFruit() {
        
    }

    public void ResetSpawnTimer() {
        _spawnTime = Time.time + SpawnTime + Random.Range(-DeltaSpawnTime, DeltaSpawnTime);
    }
}
