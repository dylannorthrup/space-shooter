using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
  private static readonly float _default_enemySpawnRate = 2f;
  private static readonly int _default_maxEnemies = 100;
  private static readonly float _default_powerupSpawnRate = 25f;

  [SerializeField]
  private float _enemySpawnRate = _default_enemySpawnRate;
  [SerializeField]
  private int _maxEnemies = _default_maxEnemies;
  [SerializeField]
  private float _powerupSpawnRate = _default_powerupSpawnRate;
  [SerializeField]
  private GameObject _enemyContainer;

  private bool _stopSpawning = false;

  public GameObject enemyPrefab;
  public GameObject[] powerUps = new GameObject[3];
  [SerializeField]
  private GameObject _powerupContainer;
  [SerializeField]
  private Player _player;

  public void StartSpawing() { 
    StartCoroutine(SpawnEnemies());
    StartCoroutine(SpawnPowerups());
  }

  public void RestartGame()
  {
    // Stop any Coroutines that may currently be running
    StopAllCoroutines();

    // Delete any enemies and powerups that might currently be here.
    foreach (Transform child in _enemyContainer.transform)
    {
      Destroy(child.gameObject);
    }
    foreach (Transform child in _powerupContainer.transform)
    {
      Destroy(child.gameObject);
    }

    _stopSpawning = false;
    StartCoroutine(SpawnEnemies());
    StartCoroutine(SpawnPowerups());
  }


  // We do reset logic here because Spawnmanager never goes away
  private void Update()
  {
    // Have the Player reset everything
    //if (Input.GetKeyDown(KeyCode.R))
    //{
    //  if (_stopSpawning)
    //  {
    //    Utilities.ComplainIfGameObjectIsNull(_player.gameObject);
    //    _player.DoTheRestartThing();
    //  }
    //}
  }

  private void Reset()
  {
    _powerupContainer = gameObject.transform.Find("Powerup Container").gameObject;
    Utilities.ComplainIfGameObjectIsNull(_powerupContainer);
    _enemyContainer = gameObject.transform.Find("Enemy Container").gameObject;
    Utilities.ComplainIfGameObjectIsNull(_enemyContainer);
    enemyPrefab = Resources.Load("Prefabs/Enemy/Enemy") as GameObject;
    powerUps = new GameObject[3];
    powerUps[0] = Resources.Load("Prefabs/Powerups/TripleShotPowerup") as GameObject;
    powerUps[1] = Resources.Load("Prefabs/Powerups/SpeedPowerUp") as GameObject;
    powerUps[2] = Resources.Load("Prefabs/Powerups/ShieldPowerUp") as GameObject;
    _enemySpawnRate = _default_enemySpawnRate;
    _powerupSpawnRate = _default_powerupSpawnRate;
  }

  public void MakeMeAPowerUp()
  {
    int randomInt = Random.Range(0, 3);
    GameObject newPowerUp = Instantiate(powerUps[randomInt], new Vector3(0.0f, 20.0f), Quaternion.identity);
    newPowerUp.transform.parent = _powerupContainer.transform;
  }

  IEnumerator SpawnPowerups()
  {
    yield return new WaitForSeconds(_powerupSpawnRate);
    while (_stopSpawning == false)
    {
      MakeMeAPowerUp();
      float spawnDelay = Random.Range(_powerupSpawnRate - 5f, _powerupSpawnRate + 5f);
      yield return new WaitForSeconds(spawnDelay);
    }
  }

  public void SpawnEnemy()
  {
    GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(0.0f, 20.0f), Quaternion.identity);
    // Keep things tidy in the Scene Hierarchy
    newEnemy.transform.parent = _enemyContainer.transform;
  }

  // Spawn enemies periodically
  IEnumerator SpawnEnemies()
  {
    yield return new WaitForSeconds(3.0f);

    while (_stopSpawning == false)
    {
      int _numEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
      // If we're at max enemies, don't spawn more
      if (_numEnemies < _maxEnemies)
      {
        Debug.Log("Spawning a new enemy because we have " + _numEnemies + " enemies currently");
        SpawnEnemy();
      }
      yield return new WaitForSeconds(_enemySpawnRate);
    }
  }

  public void OnPlayerDeath()
  {
    _stopSpawning = true;
    Enemy[] _enemies = GetComponentsInChildren<Enemy>();
    foreach(Enemy _enemy in _enemies)
    {
      _enemy.StopShooting();
      _enemy.StopAllCoroutines();
    }
  }
}
