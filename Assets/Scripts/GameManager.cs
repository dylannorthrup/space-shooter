using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  [SerializeField]
  private bool _isGameOver;

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.R) && _isGameOver)
    {
      SceneManager.LoadScene(Utilities.GAME_SCENE); // Current Game Scene
    }

    if (Input.GetKeyDown(KeyCode.Escape) && _isGameOver)
    {
      Application.Quit();
    }
  }

  public void GameOver()
  {
    _isGameOver = true;
  }
}
