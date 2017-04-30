using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

/// <summary>
/// Spawns the fruit at a set interval in seconds
/// </summary>
public class FruitSpawner : MonoBehaviour {

    /// <summary>
    /// Prefab of the fruit
    /// </summary>
    public GameObject FruitPrefab;

    /// <summary>
    /// Texture pack of all the different fruit sprites
    /// </summary>
    public Texture2D FruitTexture;

    /// <summary>
    /// Time to spawn a piece
    /// </summary>
    public float SpawnTime = 20;
    /// <summary>
    /// Random amount to be added or substracted from the time
    /// </summary>
    public float DeltaSpawnTime = 3;
    /// <summary>
    /// Time to keep available
    /// </summary>
    public float Uptime = 10;
    /// <summary>
    /// Randomness in availability of the fruit
    /// </summary>
    public float DeltaUptime = 2;

    private float _spawnTime;
    private float _lastSpawnTime;
    private Fruit.EFruit _possibleSpawns = Fruit.EFruit.All;
    private Sprite[] _fruitSprites;

    /// <summary>
    /// Possible fruits that can spawn in this level
    /// </summary>
    private List<Fruit.EFruit> _possibleSpawnsList {
        get {
            return Enum.GetValues(typeof(Fruit.EFruit)).Cast<Fruit.EFruit>().Where(value => (_possibleSpawns & value) != 0 && value != Fruit.EFruit.All).ToList();
        }
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start () {
	    _spawnTime = Time.time + SpawnTime + Random.Range(-DeltaSpawnTime, DeltaSpawnTime);
        _fruitSprites = Resources.LoadAll<Sprite>(FruitTexture.name);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update () {
	    if (Time.time > _spawnTime) {
            SpawnFruit();
            ResetSpawnTimer();
        }
    }

    /// <summary>
    /// Spawn a piece of fruit!
    /// </summary>
    private void SpawnFruit() {
        // Make sure there is no fruit already spawned
        if (_lastSpawnTime == _spawnTime)
            return;
        var obj = Instantiate(FruitPrefab, transform.position, Quaternion.identity).GetComponent<Fruit>();
        obj.SetTime(Time.time + Uptime + Random.Range(-DeltaUptime, DeltaUptime));
        var idx = Mathf.Log((float)_possibleSpawnsList.GetRandom(), 2);
        obj.GetComponent<SpriteRenderer>().sprite = _fruitSprites[(int)idx];
        _lastSpawnTime = _spawnTime;
    }

    private void DisappearFruit() {
        
    }

    /// <summary>
    /// Resets the timer to make another fruit spawn
    /// </summary>
    public void ResetSpawnTimer() {
        _spawnTime = Time.time + SpawnTime + Random.Range(-DeltaSpawnTime, DeltaSpawnTime);
    }
}
