using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBarrage : MonoBehaviour
{

  [SerializeField]
  private bool _burningItAllDown = false;

  void Start()
  {
    StartCoroutine(CheckIfIHaveChildren());
  }

  IEnumerator CheckIfIHaveChildren()
  {
    while (true)
    {
      yield return new WaitForSeconds(2.0f);
      if (transform.childCount == 0)
      {
        Destroy(gameObject);
      }
    }
  }

  public void DestroyMeAndMyChildren()
  {
    _burningItAllDown = true;
    foreach (Transform child in transform)
    {
      Laser _child = child.gameObject.GetComponent<Laser>();
      if(_child != null)
      {
        _child.DieRightNow();
      }
      else
      {
        Debug.LogError("Could not find Laser component in child gameObject");
      }
    }
    Destroy(gameObject);
  }

  //// Update is called once per frame
  //void Update()
  //{

  //}
}
