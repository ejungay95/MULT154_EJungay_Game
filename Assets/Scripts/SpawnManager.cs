using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour
{
  public enum SpawnState { SPAWNING, WAITING, COUNTING };

  [System.Serializable]
  public class Wave
  {
    public GameObject[] enemyTypes;
    public int count;
    public float rate;

    public List<GameObject> enemies = new List<GameObject>();
  }

  public LayerMask checkForPlayer;
  public float checkRadius = 1f;

  public Wave[] waves;
  public int waveIndex = 0;
  public int level = 1;

  public float timeBetweenWaves = 5f;
  public float waveCountdown;

  public Transform[] spawnPoints;

  public SpawnState currentState = SpawnState.COUNTING;

  // Start is called before the first frame update
  void Start()
  {
    waveCountdown = timeBetweenWaves;
  }

  // Update is called once per frame
  void Update()
  {
    RemoveNullFromList(waves[waveIndex]);
    if(currentState == SpawnState.WAITING)
    {
      if (!AreEnemiesAlive(waves[waveIndex]))
      {
        WaveCompleted();
      } else
      {
        return;
      }
    }

    if(waveCountdown <= 0)
    {
      if (currentState != SpawnState.SPAWNING)
      {
        // start spawning
        StartCoroutine(SpawnWave(waves[waveIndex]));
      }
    } else
    {
      waveCountdown -= Time.deltaTime;
    }
  }

  IEnumerator SpawnWave(Wave wave)
  {
    currentState = SpawnState.SPAWNING;

    for(int i = 0; i < wave.count; i++)
    {
      SpawnEnemy(wave);
      yield return new WaitForSeconds(1f / wave.rate);
    }

    currentState = SpawnState.WAITING;
     
    yield break;
  }

  void SpawnEnemy(Wave wave)
  {
    GameObject enemyType = wave.enemyTypes[Random.Range(0, wave.enemyTypes.Length)];
    GameObject temp;
    Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
    while (Physics2D.OverlapCircle(spawn.position, checkRadius, checkForPlayer) != null)
    {
      spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
    temp = Instantiate(enemyType, spawn.position, Quaternion.identity);
    wave.enemies.Add(temp);
  }

  bool AreEnemiesAlive(Wave wave)
  {
    if(wave.enemies.Count > 0)
    {
      return true;
    }

    return false;
  }

  void RemoveNullFromList(Wave wave)
  {
    wave.enemies.RemoveAll(enemy => enemy == null);
  }

  void WaveCompleted()
  {
    currentState = SpawnState.COUNTING; 
    
    waveCountdown = timeBetweenWaves;

    if(waveIndex + 1 > waves.Length - 1)
    {
      waveIndex = 0;
    }

    waveIndex++;
  }
}
