using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

  // Default values
  private static readonly float _default_speed = 8.0f;
  private static readonly float _default_fade_speed = 0.2f;
  private static readonly float _default_explodey_scale_factor = 0.2f;

  [SerializeField]
  private float _speed = _default_speed;
  [SerializeField]
  private float _fade_speed = _default_fade_speed;
  [SerializeField]
  private float _explodey_scale_factor = _default_explodey_scale_factor;

  [SerializeField]
  private bool _enemyLaser;
  private bool _dying;
  private EnemyBarrage _parent;
  private List<Laser> _siblings;

  void Start()
  {
    _siblings = new List<Laser>();
    // All this so we can know who our parent and sibling are.
    if (_enemyLaser)
    {
      int myIndex = transform.GetSiblingIndex();
      _parent = transform.GetComponentInParent<EnemyBarrage>();
      if (_parent != null)
      {
        Laser[] _allSiblings = _parent.GetComponentsInChildren<Laser>();
        if (_allSiblings != null)
        {
          foreach (Laser _sibling in _allSiblings)
          {
            int index = _sibling.transform.GetSiblingIndex();
            if (index != myIndex)
            {
              _siblings.Add(_sibling);
            }
          }
        }
        else
        {
          Debug.LogError("Could not get siblings from parent as an Enemy Laser");
        }
      }
      else
      {
        Debug.LogError("Could no find my parent as an Enemy Laser");
      }
    }
  }

  private void Reset()
  {
    _speed = _default_speed;
    _fade_speed = _default_fade_speed;
    _explodey_scale_factor = _default_explodey_scale_factor;
  }

  // Update is called once per frame
  void Update()
  {
    // Translate laser in the appropriate direction
    if (_enemyLaser)
    {
      transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }
    else
    {
      transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }

    // Destroy lasers if they get too far beyond the bounds of the game. We give
    // a good lower bound 
    if (transform.position.y > 18f || transform.position.y < Utilities.LOWER_BOUND - 20.0f)
    {
      DieRightNow();
    }

    if (_dying)
    {
      GetBusyDying();
    }
  }

  // If we've been told to die, get busy dying
  void GetBusyDying()
  {
    // We want to have this fade out and also increase it's scale/size while
    // it does so

    // Do the fading bit
    SpriteRenderer spriteRend = GetComponent<SpriteRenderer>();
    Color _color = spriteRend.material.color;
    _color.a = _color.a -= _fade_speed;
    spriteRend.material.color = _color;

    // If we've got to the point we should be invisible, destroy ourselves
    if (_color.a <= 0.0f)
    {
      Destroy(this.gameObject);
    }

    // Do the scaling bit
    transform.localScale += new Vector3(1f, 0f, 1f) * _explodey_scale_factor;
  }

  // Used to indicate this laser needs to go away in a cinematic fashion
  public void StartDying()
  {
    // Set boolean so we'll call GetBusyDying in the Update
    _dying = true;

    // Disable colliders so we don't trigger any other items...
    // If we are an enemy laser, we also want to disable our siblings
    if (_enemyLaser)
    {
      foreach (Laser _sibling in _siblings)
      {
        if (_sibling.AmIDying() == false)
        {
          _sibling.DieRightNow();
        }
      }
      _parent.DestroyMeAndMyChildren();
    }
    // Otherwise, we just need to disable ourselves
    else
    {
      BoxCollider2D collider = GetComponent<BoxCollider2D>();
      if (collider != null)
      {
        collider.enabled = false;
      }
      else
      {
        Debug.LogError("Could not get BoxCollider2D for Laser");
      }
    }
  }

  public bool AmIDying()
  {
    return _dying;
  }

  public void DieRightNow()
  {
    Destroy(gameObject);
  }
}
