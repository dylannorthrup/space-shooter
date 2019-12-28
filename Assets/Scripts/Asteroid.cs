using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
  private static readonly float _defaultRotateSpeed = 20.0f;
  [SerializeField]
  private float _rotateSpeed = _defaultRotateSpeed;

  [SerializeField]
  private GameObject _explosionPrefab;
  [SerializeField]
  private SpawnManager _spawnManager;

  private void Start()
  {
    _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
  }

  private void Reset()
  {
    _rotateSpeed = _defaultRotateSpeed;
    _explosionPrefab = Resources.Load("Prefabs/Explosion") as GameObject;
    if (_explosionPrefab == null)
    {
      Debug.LogError("_explosionPrefab is null!!!");
    }
  }

  // Update is called once per frame
  void Update()
  {
    transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Laser"))
    {
      // Instantiate explosion
      GameObject asplosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
      // Destroy ourselves after a short delay (so the explosion can cover us completely)
      Destroy(this.gameObject, 0.5f);
      // And disable our collider
      CircleCollider2D _cc = this.GetComponent<CircleCollider2D>();
      if(_cc != null)
      {
        _cc.enabled = false;
      }
      // And destroy the laser
      Destroy(other.gameObject);
      // And, finally, start spawning bad guys
      _spawnManager.StartSpawing();
    }
  }
}
