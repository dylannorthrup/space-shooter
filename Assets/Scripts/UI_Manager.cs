using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
  // Handle for text
  [SerializeField]
  private Text _scoreText;
  private int _score = 0;
  [SerializeField]
  private Text _gameOverText;
  private bool _blinkGameOverText;
  [SerializeField]
  private Text _restartText;

  [SerializeField]
  private Sprite[] _livesSprites;
  [SerializeField]
  private Image _livesImg;

  private GameManager _gameManager;

  // Start is called before the first frame update
  void Start()
  {
    // Set GameManager reference for restart porpoises
    _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    Utilities.ComplainIfGameObjectIsNull(_gameManager.gameObject);

    // Set score text
    _scoreText = transform.Find("Score_text").GetComponent<Text>();
    Utilities.ComplainIfGameObjectIsNull(_scoreText.gameObject);
    _scoreText.text = "Score: " + _score;

    // Set and Deactivate Game Over text
    _gameOverText = transform.Find("Game_Over_text").GetComponent<Text>();
    Utilities.ComplainIfGameObjectIsNull(_gameOverText.gameObject);
    _gameOverText.gameObject.SetActive(false);

    // Set and Deactivate Game Over text
    _restartText = transform.Find("Restart_text").GetComponent<Text>();
    Utilities.ComplainIfGameObjectIsNull(_restartText.gameObject);
    _restartText.gameObject.SetActive(false);

    Utilities.ComplainIfGameObjectIsNull(_livesImg.gameObject);
  }

  public void RestartGame()
  {
    _gameOverText.gameObject.SetActive(false);
    _restartText.gameObject.SetActive(false);
    UpdateScore(0);
    // Stop any Coroutines that may be currently running
    StopAllCoroutines();
    UpdateLives(3);
  }

  private void Reset()
  {
    _scoreText = transform.Find("Score_text").GetComponent<Text>();
    // Assign text component to the handle
    if (_scoreText != null)
    {
      _scoreText.text = "Score: " + _score;
    }
    else
    {
      Debug.Log("_scoreText in UI_Manager is null!");
    }

    // Deactivate Game Over text
    _gameOverText = transform.Find("Game_Over_text").GetComponent<Text>();
    if (_gameOverText != null)
    {
      _gameOverText.gameObject.SetActive(false);
    }
    else
    {
      Debug.Log("_gameOverText in UI_Manager is null!");
    }

    _livesImg = transform.Find("Lives_Display_img").GetComponent<Image>();
    if (_livesImg == null)
    {
      Debug.Log("_livesImg in UI_Manager is null!");
    }

    _livesSprites = new Sprite[4];
    // This isn't working :(
    //_livesSprites[0] = Resources.Load("Images/Lives/no_lives.png") as Sprite;
    //_livesSprites[1] = Resources.Load("Images/Lives/One.png") as Sprite;
    //_livesSprites[2] = Resources.Load("Images/Lives/Two.png") as Sprite;
    //_livesSprites[3] = Resources.Load("Images/Lives/Three.png") as Sprite;
  }

  public void UpdateScore(int newScore)
  {
    _score = newScore;
    _scoreText.text = "Score: " + _score;
  }

  public void UpdateLives(int currentLives)
  {
    if (currentLives > 3)
    {
      currentLives = 3;
    }
    if (currentLives < 0)
    {
      currentLives = 0;
    }
    _livesImg.sprite = _livesSprites[currentLives];
    if (currentLives <= 0)
    {
      GameOverStuff();
    }
  }

  public void GameOverStuff()
  {
    _blinkGameOverText = true;
    _ = StartCoroutine(BlinkGameOver());
    _restartText.transform.gameObject.SetActive(true);
    _gameManager.GameOver();
  }

  IEnumerator BlinkGameOver()
  {
    while (_blinkGameOverText)
    {
      // Toggle active, then wait a second
      _gameOverText.transform.gameObject.SetActive(!_gameOverText.gameObject.activeSelf);
      yield return new WaitForSeconds(1);
    }
  }
}
