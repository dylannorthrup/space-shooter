using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
  private static readonly float _default_speed = 4.0f;
  private static readonly int _default_score = 10;
  private static readonly float _default_dying_slowdown = 0.3f;
  [SerializeField]
  private float _speed = _default_speed;
  [SerializeField]
  private int _myScore = _default_score;
  [SerializeField]
  private float _dying_slowdown = _default_dying_slowdown;

  [SerializeField]
  private bool _enemiesFireWeapons = false;
  private bool _weaponsCharged = true;
  [SerializeField]
  private float _weaponAvgChargeTime = 5.0f;
  [SerializeField]
  private float _weaponChargeTimeJitter = 2.0f;
  [SerializeField]
  private GameObject _enemyBarragePrefab;
  [SerializeField]
  private bool _relocatingSpawnsMoreEnemies = false;

  private Animator _anim;

  [SerializeField]
  private AudioClip _explosionSound;
  private AudioSource _audioSource;

  private bool _dying = false;

  // Start is called before the first frame update
  void Start()
  {
    Vector3 spawn_point = new Vector3(0, 0, 0)
    {
      x = Utilities.RandomX(),
      y = Utilities.RandomY()
    };
    transform.position = spawn_point;

    // Set up animation
    _anim = GetComponent<Animator>();
    if (_anim == null)
    {
      Debug.LogError("_anim for Enemy is null!");
    }

    // If our speed is 0 or null, set it to the _default_speed
    if (_speed == 0)
    {
      _speed = _default_speed;
    }

    // Set up audioSource
    _audioSource = GetComponent<AudioSource>();
    if (_audioSource == null)
    {
      Debug.LogError("_audioSource on Enemy is NULL!");
    }
    else
    {
      _audioSource.clip = _explosionSound;
    }

    _weaponsCharged = true;
  }

  // Reset default values
  private void Reset()
  {
    _speed = _default_speed;
    _dying_slowdown = _default_dying_slowdown;
    _enemyBarragePrefab = Resources.Load("Prefabs/Enemy/EnemyBarrage") as GameObject;
    if(_enemyBarragePrefab == null)
    {
      Debug.LogError("_enemyBarragePrefab is NULL!!");
    }
  }

  // Update is called once per frame
  void Update()
  {
    // Move enemy
    CalculateMovement();

    // If we're dying, do the "we're dying" part
    if (_dying)
    {
      GetBusyDying();
      return;
    }

    // Reappear if it goes beyond the screen bounds
    ReappearUpTop();

    // Fire Lasers if we are meant to do so
    if(_enemiesFireWeapons && _weaponsCharged)
    {
      StartCoroutine(FireWeapons());
    }

  }

  IEnumerator FireWeapons()
  {
    _weaponsCharged = false;
    float _delay = Random.Range(_weaponAvgChargeTime - _weaponChargeTimeJitter, _weaponAvgChargeTime + _weaponChargeTimeJitter);
    yield return new WaitForSeconds(_delay);
    // Double check to see if we still need to fire this weapon. Means we don't have to worry about tracking this Coroutine and/or stopping it if we no longer want to fire weapons.
    if (_enemiesFireWeapons)
    {
      Instantiate(_enemyBarragePrefab, transform.position, Quaternion.identity);
    }
    _weaponsCharged = true;
  }
  
  private void OnTriggerEnter2D(Collider2D other)
  {
    // If this is an EnemyLaser, ignore it
    if(other.CompareTag("EnemyLaser"))
    {
      return;
    }

    // If the other item is a player
    if (other.CompareTag("Player"))
    {
      // Damage player
      Player player = other.transform.GetComponent<Player>();
      if(player != null)
      {
        player.Damage();
      }

      //// Start our dying process
      StartDying();
    }

    // If the other item is a laser, start dying and tell the
    // laser to start dying as well.
    if (other.CompareTag("Laser"))
    {
      StartDying();
      // Get reference to player and invoke AddToScore(score)
      Player p = GameObject.Find("Player").GetComponent<Player>();
      if (p != null)
      {
        p.AddToScore(_myScore);
      }

      // Tell the laser to start dying
      other.BroadcastMessage("StartDying");
    }
  }

  // What to do when enemy needs to disappear/blow up/whatever
  private void GetBusyDying()
  {
    // Have the enemy slow down while it's dying, so it's not an abrupt halting
    if (_speed > 0f)
    {
      //Debug.Log("Slowing down from " + _speed);
      _speed -= _dying_slowdown;
      //Debug.Log("New speed is " + _speed);
    }
  }
  
  // Switch `_dying` boolean so we know we need to "GetBusyDying()"
  private void StartDying()
  {
    _dying = true;
    StopShooting();
    _anim.SetTrigger("OnEnemyDeath");
    Destroy(this.gameObject, 1.8f);
    PolygonCollider2D _pc = GetComponent<PolygonCollider2D>();
    if(_pc != null)
    {
      _pc.enabled = false;
    }
    // Start playing explosion sound
    _audioSource.Play();
  }

  // If we drop off the bottom of the screen, reappear at a random
  // X position at the top of the screen
  void ReappearUpTop()
  {
    if (transform.position.y < Utilities.LOWER_BOUND)
    {
      Vector3 here = transform.position;
      here.y = Utilities.RandomY();
      here.x = Utilities.RandomX();
      transform.position = here;
    }
  }

  void CalculateMovement()
  {
    transform.Translate(Vector3.down * _speed * Time.deltaTime);
  }

  public void StopShooting()
  {
    _enemiesFireWeapons = false;
  }
}
