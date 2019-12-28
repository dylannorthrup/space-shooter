using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{

  private static readonly float _default_backgroundSpeed = 30f;
  [SerializeField]
  private float _backgroundSpeed = _default_backgroundSpeed;

  private GameObject _backgroundPrefab;
  private GameObject[] _backgroundInstances;

  // Start is called before the first frame update
  void Start()
  {
    _backgroundPrefab = Resources.Load("Prefabs/Background") as GameObject;
    if (_backgroundPrefab == null)
    {
      Debug.LogError("_backgroundPrefab is null!!");
    }
    _backgroundInstances = new GameObject[2];
    _backgroundInstances[0] = Instantiate(_backgroundPrefab, new Vector3(0f, Utilities.BACKGROUND_LOWER_OFFSET, 0f), Quaternion.identity);
    _backgroundInstances[0].transform.SetParent(this.transform);
    _backgroundInstances[1] = Instantiate(_backgroundPrefab, new Vector3(0f, Utilities.BACKGROUND_UPPER_OFFSET, 0f), Quaternion.identity);
    _backgroundInstances[1].transform.SetParent(this.transform);
    // Fade in the first background to half opacity
    SpriteRenderer _spriteRend = _backgroundInstances[0].GetComponent<SpriteRenderer>();
    if (_spriteRend != null)
    {
      // Start this off transparent
      Color _c = _spriteRend.material.color;
      _c.a = 0f;
      _spriteRend.material.color = _c;
    }
    StartCoroutine(FadeIn());
    // Set the second background to half opacity right off the bat
    _spriteRend = _backgroundInstances[1].GetComponent<SpriteRenderer>();
    if (_spriteRend != null)
    {
      Color _c = _spriteRend.material.color;
      _c.a = 0.3f;
      _spriteRend.material.color = _c;
    }
  }

  private void Reset()
  {
    _backgroundSpeed = _default_backgroundSpeed;
  }

  IEnumerator FadeIn()
  {
    float alphaValue = 0f;
    Color _c;
    SpriteRenderer _spriteRend = _backgroundInstances[0].GetComponent<SpriteRenderer>();
    while (alphaValue < 0.3f)
    {
      _c = _spriteRend.material.color;
      _c.a += 0.001f;
      alphaValue = _c.a;
      _spriteRend.material.color = _c;
    }
    yield return 0;
  }

  private void MoveAndRelocateIfNecessary(GameObject _bi)
  {
    _bi.transform.Translate(Vector3.down * _backgroundSpeed * Time.deltaTime);
    if (_bi.transform.position.y < -Utilities.BACKGROUND_LOWER_OFFSET)
    {
      _bi.transform.Translate(Vector3.up * Utilities.BACKGROUND_RELOCATION_DISTANCE);
    }
  }


  // Update is called once per frame
  void Update()
  {
    MoveAndRelocateIfNecessary(_backgroundInstances[0]);
    MoveAndRelocateIfNecessary(_backgroundInstances[1]);
  }
}