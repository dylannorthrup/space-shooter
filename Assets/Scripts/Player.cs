using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO Features
// - Accuracy on shots (number of shots / number of enemy deaths) * 100
// - Increasing baddies depending on levels
// - Different enemy types (images, speed, size, shooters, etc) for different points
// - Levels?
// - Faster spawn rates?
// - Spawn additional enemy if an enemy makes it to the bottom of the screen
// - Lives on top of damage (3 hits = -1 life; 3 lives = game over)
// - Life powerup (if at less than three lives)

public class Player : MonoBehaviour
{
  // Set static defaults so we can use Reset() to get back to 
  // them.
  private static readonly float _default_speed = 3.5f;
  private static readonly float _default_fireRate = 0.2f;
  private static readonly float _default_laserOffset = .45f;
  private static readonly float _default_powerupDuration = 5f;
  private static readonly float _default_boosted_speed = 7f;
  [SerializeField]
  private float _speed = _default_speed;
  [SerializeField]
  private float _fireRate = _default_fireRate;
  [SerializeField]
  private float _laserOffset = _default_laserOffset;
  [SerializeField]
  private float _boosted_speed = _default_boosted_speed;

  [SerializeField]
  private bool _cheatCodesEnabled;

  [SerializeField]
  private int _lives = 3;

  private bool _speedUpActive;
  private bool _shieldActive;
  private bool _tripleShotActive;
  private int[] _powerUps;
  private float effective_speed = _default_speed;

  [SerializeField]
  private int _score;

  private Transform _shieldTransform;
  private Transform _rightEngineFadeIn, _leftEngineFadeIn;
  private Transform _rightEngine, _leftEngine;

  public GameObject _laserPrefab;
  public GameObject _tripleShotPrefab;

  private bool _inputDisabled = false;
  private float _nextfire = 0;
  private Vector3 location;
  private SpawnManager _spawnManager;
  [SerializeField]
  private UI_Manager _UI_Manager;

  private GameObject _explosionPrefab;
  private PolygonCollider2D _playerCollider;
  private SpriteRenderer _spriteRend;

  // Start is called before the first frame update
  void Start()
  {
    // Initialize Shield Visualizer and Make sure it is on top of us
    _shieldTransform = transform.GetChild(Utilities.CHILD_SHIELD_INDEX);
    _shieldTransform.position = transform.position;
    SetShieldVisualization(_shieldActive);

    _rightEngineFadeIn = transform.GetChild(Utilities.CHILD_RIGHTENGINEDAMAGEFADEIN_INDEX);
    _rightEngine = transform.GetChild(Utilities.CHILD_RIGHTENGINEDAMAGE_INDEX);
    _leftEngineFadeIn = transform.GetChild(Utilities.CHILD_LEFTENGINEDAMAGEFADEIN_INDEX);
    _leftEngine = transform.GetChild(Utilities.CHILD_LEFTENGINEDAMAGE_INDEX);

    // Get reference to SpawnManager
    _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    if (_spawnManager == null)
    {
      Debug.LogError("_spawnManager is NULL!");
    }

    // Get reference to UI Manager
    _UI_Manager = GameObject.Find("Canvas")
      .GetComponent<UI_Manager>();
    if (_UI_Manager == null)
    {
      Debug.Log("_UI_Manager is NULL!");
    }

    _explosionPrefab = Resources.Load("Prefabs/Explosion") as GameObject;
    if (_explosionPrefab == null)
    {
      Debug.LogError("_explosionPrefab for Player is null!!!");
    }

    _playerCollider = GetComponent<PolygonCollider2D>();
    if(_playerCollider == null)
    {
      Debug.LogError("_playerCollider is NULL!!!");
    }

    _spriteRend = GetComponent<SpriteRenderer>();
    if (_spriteRend == null)
    {
      Debug.LogError("_spriteRend for Player is NULL!!!");
    }

    RestartGame();
  }

  public void RestartGame()
  {
    this.gameObject.SetActive(true);
    // Take the current position = new Position (0,0,0)
    transform.position = new Vector3(0, 0, 0);
    // Turn off all power ups
    _powerUps = new int[Utilities.POWERUP_ARRAY_SIZE];
    for (int i = 0; i < Utilities.POWERUP_ARRAY_SIZE; i++)
    {
      _powerUps[i] = 0;
    }
    SyncPowerups();
    SetShieldVisualization(_shieldActive);

    // Reset lives and score
    _lives = 3;
    _score = 0;

  }

  public void DoTheRestartThing()
  {
    RestartGame();
    _UI_Manager.RestartGame();
    _spawnManager.RestartGame();
  }

  private void Reset()
  {

    _laserPrefab = Resources.Load("Prefabs/Laser") as GameObject;
    _tripleShotPrefab = Resources.Load("Prefabs/Triple_shot") as GameObject;
    // NULL checks
    if (_laserPrefab == null)
    {
      Debug.LogError("_laserPrefab for Player is null!!!");
    }
    if (_tripleShotPrefab == null)
    {
      Debug.LogError("_tripleShotPrefab for Player is null!!!");
    }

    _speed = _default_speed;
    _fireRate = _default_fireRate;
    _boosted_speed = _default_boosted_speed;
  }

  // Update is called once per frame
  void Update()
  {
    // Move player around
    CalculateMovement();

    // Fire LAZERZ!@!!!!
    if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1")) && Time.time > _nextfire)
    {
      FireLasers();
    }
    // POWERUP CHEAT CODE
    if (_cheatCodesEnabled && Input.GetKeyDown(KeyCode.P))
    {
      _spawnManager.MakeMeAPowerUp();
    }
    // LIVES CHEAT CODE
    if (_cheatCodesEnabled && Input.GetKeyDown(KeyCode.L))
    {
      _lives += 100;
    }
    // DAMAGE CHEAT CODE
    if (_cheatCodesEnabled && Input.GetKeyDown(KeyCode.G))
    {
      Damage();
    }
  }

  void FireLasers()
  {
    if(_inputDisabled)
    {
      return;
    }
    _nextfire = Time.time + _fireRate;
    if (_tripleShotActive)
    {
      location = transform.position;
      Instantiate(_tripleShotPrefab, location, Quaternion.identity);
      //Debug.Log("PEWPEWPEW! PEWPEWPEW!");
    }
    else
    {
      location = transform.position + new Vector3(0, _laserOffset, 0);
      Instantiate(_laserPrefab, location, Quaternion.identity);
      //Debug.Log("PEW! PEW!");
    }
  }


  public void ActivatePowerUp(int index, float duration = 0f)
  {
    if (duration == 0f)
    {
      duration = _default_powerupDuration;
    }
    // Check here to see if the _powerUps value for the index is true.
    // If so, find the coroutine in _powerUpTimers, invoke StopCoroutine() on it,
    // then start another CoRoutine to, effectively, restart the timer.
    _powerUps[index]++;
    SyncPowerups();
    if (duration > 0)
    {
      StartCoroutine(PowerUpTimer(duration, index));
    }
  }

  // Add a thing here so we can pass by reference the
  // variable we want to deactivate.
  public IEnumerator PowerUpTimer(float duration, int i)
  {
    yield return new WaitForSeconds(duration);
    DisablePowerup(i);
  }

  // Decrement value in powerup array, then sync flags
  private void DisablePowerup(int i)
  {
    // We don't want to go "negative" on power ups
    if (_powerUps[i] > 0)
    {
      _powerUps[i]--;
      SyncPowerups();
    }
  }

  // Utility function to modify boolean references based on values in our powerup tracker array
  private void ModifyPowerUps(int index, ref bool powerup)
  {
    if (_powerUps[index] > 0)
    {
      powerup = true;
    }
    else
    {
      powerup = false;
    }
  }

  private void SetShieldVisualization(bool active)
  {
    Debug.Log("Setting Shield Visualziation: " + active);
    GameObject shield = _shieldTransform.gameObject;
    shield.SetActive(active);
  }

  // Activate or deactivate power ups based on our tracking array
  private void SyncPowerups()
  {
    bool prevShieldSetting = _shieldActive;
    ModifyPowerUps(Utilities.TRIPLESHOT_POWERUP, ref _tripleShotActive);
    ModifyPowerUps(Utilities.SPEED_POWERUP, ref _speedUpActive);
    ModifyPowerUps(Utilities.SHIELD_POWERUP, ref _shieldActive);
    // If we had a change in shield setting, activate or deactivate the 
    // visualization as appropriate
    if (prevShieldSetting != _shieldActive)
    {
        SetShieldVisualization(_shieldActive);
    }
  }

  void CalculateMovement()
  {
    if(_inputDisabled)
    {
      return;
    }
    // Check to see if the speed power up is active and modify effective speed if it is
    if (_speedUpActive && effective_speed == _speed)
    {
      effective_speed = _boosted_speed;
    }
    else if (!_speedUpActive && effective_speed != _speed)
    {
      effective_speed = _speed;
    }

    // Use default input Axes to find out if we're getting input ('a', 'd', left/right arrows, or controller joysticks)
    float _horizontalInput = Input.GetAxis("Horizontal");
    // Same with up/down ('w', 'd', up/down arrow, or controller joystick)
    float _verticalInput = Input.GetAxis("Vertical");


    // An idiomatic way to do movement: create a new Vector3 with the _horizontalInput and _verticalInput directly like so...
    transform.Translate(new Vector3(_horizontalInput, _verticalInput, 0) * effective_speed * Time.deltaTime);

    // We are giong to constrain our movement.
    // Mathf.Clamp() can constrain values between an upper and lower bound. This will keep us from going outside the horizontal visual bounds of the screen
    transform.position = new Vector3(
      transform.position.x,
      Mathf.Clamp(transform.position.y, Utilities.LOWER_PLAYER_BOUND, Utilities.UPPER_PLAYER_BOUND),
      transform.position.z);

    // This will allow us to "wrap around" horizontally
    // Ideally we'd do a comparison of Mathf.abs(transform.position.x) to our boundary condition and, if we're outside the boundary, multiply transform.position.x by -1. But, if we do that, we're still outside the boundary on the other side.  This isn't an issue if we're still moving, but if we happen to stop just outside the boundary, we will be constantly flipping back and forth.  So, we do the positive and negative comparisons here and adjust by 0.1 or -0.1 respectiviely depending on which side of the boundary we're on.

    //  11.4 -> -11.3.... x * -1 + (0.1 * +1[from Mathf.Sign])
    // -11.4 ->  11.3.... x * -1 + (0.1 * -1[from Mathf.Sign])

    float adjustment = 0.1f * Mathf.Sign(transform.position.x);
    if (Mathf.Abs(transform.position.x) > Utilities.MAX_HORIZONTAL_BOUND)
    {
      Vector3 here = transform.position;
      here.x = (transform.position.x * -1) + adjustment;
      transform.position = here;
    }
  }

  // A bit of logic to play a FadeIn animation, then to play the looping animation for engine damage.
  IEnumerator EngineDamage(Transform fadeIn, Transform repeat)
  {
    fadeIn.gameObject.SetActive(true);
    yield return new WaitForSeconds(0.8f);
    repeat.gameObject.SetActive(true);
    fadeIn.gameObject.SetActive(false);
  }

  // Handle when the player is hit by an object
  public void Damage()
  {
    // If shields are active, reduce our number of shields, sync
    // up powerups, and return.
    if (_shieldActive)
    {
      _powerUps[Utilities.SHIELD_POWERUP]--;
      SyncPowerups();
      return;
    }

    // Otherwise, do the dying thing
    _lives--;
    _UI_Manager.UpdateLives(_lives);

    // If lives == 2, enable right engine
    if (_lives == 2)
    {
      StartCoroutine(EngineDamage(_rightEngineFadeIn, _rightEngine));
    }
    // If lives == 1, enable left engine
    if (_lives == 1)
    {
      StartCoroutine(EngineDamage(_leftEngineFadeIn, _leftEngine));
    }

    // Check if we're dead and destroy ourselves if we are
    if (_lives <= 0)
    {
      _spawnManager.OnPlayerDeath();
      GameObject _explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
      Utilities.ComplainIfGameObjectIsNull(_explosion);
      // Get the SpriteRenderer of the explosion so we can put it above everything else so it covers up the disappearing GameObjects of the player
      SpriteRenderer _esr = _explosion.GetComponent<SpriteRenderer>();
      if(_esr != null)
      {
        _esr.sortingOrder = 20;
      }
      StartCoroutine(MakeMeInactive());
      //Destroy(this.gameObject);
    }
    else
    // If we have lives left, make ourselves invincible for a second
    {
      StartCoroutine(ActivateInvincibilityFrames());
    }
  }

  IEnumerator ActivateInvincibilityFrames()
  {
    if(_playerCollider != null)
    {
      _playerCollider.enabled = false;
      yield return new WaitForSeconds(0.5f);
      _playerCollider.enabled = true;
    }
  }

  IEnumerator MakeMeInactive()
  {
    _inputDisabled = true;
    yield return new WaitForSeconds(1.0f);
    _inputDisabled = false;
    gameObject.SetActive(false);
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if(other.CompareTag("EnemyLaser"))
    {
      Laser _laser = other.GetComponent<Laser>();
      if(_laser != null)
      {
        _laser.StartDying();
      }
      Damage();
    }
  }

  public void AddToScore(int value = 10)
  {
    Debug.Log("Adding " + value + " to a score of " + _score);
    _score += value;
    Debug.Log("Score is now " + _score);
    _UI_Manager.UpdateScore(_score);
  }
}