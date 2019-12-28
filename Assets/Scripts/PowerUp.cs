using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

  private static readonly float _default_speed = 3.0f;
  private static readonly float _default_fade_speed = 0.2f;
  private static readonly float _default_explodey_scale_factor = 0.2f;
  private static readonly float _default_powerupDuration = 15f;

  [SerializeField]
  private float _speed = _default_speed;
  [SerializeField]
  private float _fade_speed = _default_fade_speed;
  [SerializeField]
  private float _explodey_scale_factor = _default_explodey_scale_factor;
  [SerializeField]
  private float _powerupDuration = _default_powerupDuration;

  // Make IDs for power ups
  // 0 = Triple Shot
  // 1 = Speed
  // 2 = Shields
  [SerializeField]
  private int powerupID;

  private bool _dying;

  [SerializeField]
  private AudioClip _powerUpSound;
  private AudioSource _audioSource;

  // Start is called before the first frame update
  void Start()
  {
    Vector3 spawn_point = new Vector3(0, 0, 0)
    {
      x = Utilities.RandomX(),
      y = Utilities.RandomY()
    };
    transform.position = spawn_point;

    _audioSource = GetComponent<AudioSource>();
    if(_audioSource == null)
    {
      Debug.LogError("_audioSource is NULL");
    }
    else
    {
      _audioSource.clip = _powerUpSound;
    }
  }

  private void Reset()
  {
    _speed = _default_speed;
  }

  // Update is called once per frame
  void Update()
  {
    if (_dying)
    {
      GetBusyDying();
      return;
    }

    // Move down at a speed of 3
    CalculateMovement();

    // Destroy when we leave the screen
    if (transform.position.y < Utilities.LOWER_BOUND)
    {
      Destroy(gameObject);
    }
  }

  private void CalculateMovement()
  {
    transform.Translate(Vector3.down * _speed * Time.deltaTime);
  }

  private void GetBusyDying()
  {
    SpriteRenderer spriteRend = GetComponent<SpriteRenderer>();
    Color _color = spriteRend.material.color;
    _color.a = _color.a -= _fade_speed;
    spriteRend.material.color = _color;

    if (_color.a <= 0.0f)
    {
      Destroy(this.gameObject, 1.0f);
      spriteRend.enabled = false;
      return;
    }

    // Do the scaling bit
    transform.localScale += new Vector3(1f, 0f, 1f) * _explodey_scale_factor;
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    // Only be collectible by the Player
    if (other.CompareTag("Player"))
    {
      Player player = other.transform.GetComponent<Player>();
      if(player != null)
      {
        player.ActivatePowerUp(powerupID, _powerupDuration);
        _dying = true;
        _audioSource.Play();
        //AudioSource.PlayClipAtPoint(_powerUpSound, transform.position);
        CircleCollider2D _cc = GetComponent<CircleCollider2D>();
        if(_cc != null)
        {
          _cc.enabled = false;
        }
      }
    }
  }
}
