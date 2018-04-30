using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

  public BoardManager boardScript;
  public float turnDelay = .1f;
  public float levelStartDelay = 2f;
  public static GameManager instance = null;
  public int playerFoodPoints = 100;
  [HideInInspector] public bool playersTurn = true;

  private int level = 1;
  private List<Enemy> enemies;
  private bool enemiesMoving;
  private Text levelText;
  private GameObject levelImage;
  private bool doingSetup;

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
    else if (instance != this)
    {
      Destroy(gameObject);
    }
    // persist between scenes
    DontDestroyOnLoad(gameObject);
    enemies = new List<Enemy>();
    boardScript = GetComponent<BoardManager>();
    InitGame();
  }


  //This is called each time a scene is loaded.
  void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
  {
    //Add one to our level number.
    level++;
    //Call InitGame to initialize our level.
    InitGame();
  }

  void OnEnable()
  {
    //Tell our ‘OnLevelFinishedLoading’ function to start listening for a scene change event as soon as this script is enabled.
    SceneManager.sceneLoaded += OnLevelFinishedLoading;
  }
  
  void OnDisable()
  {
    //Tell our ‘OnLevelFinishedLoading’ function to stop listening for a scene change event as soon as this script is disabled.
    //Remember to always have an unsubscription for every delegate you subscribe to!
    SceneManager.sceneLoaded -= OnLevelFinishedLoading;
  }

  void InitGame()
  {
    doingSetup = true;

    levelImage = GameObject.Find("LevelImage");
    levelText = GameObject.Find("LevelText").GetComponent<Text>();
    levelText.text = "Day " + level;
    levelImage.SetActive(true);
    Invoke("HideLevelImage", levelStartDelay);

    // clear enemies from last level
    enemies.Clear();
    boardScript.SetupScene(level);
  }

  private void HideLevelImage()
  {
    levelImage.SetActive(false);
    doingSetup = false;
  }

  public void GameOver()
  {
    levelText.text = "After " + level + "days, you starved.";
    levelImage.SetActive(true);
    enabled = false;
  }

  // Update is called once per frame
  void Update()
  {
    if (playersTurn || enemiesMoving || doingSetup)
    {
      return;
    }

    StartCoroutine(MoveEnemies());
  }

  public void AddEnemyToList(Enemy script)
  {
    enemies.Add(script);
  }

  IEnumerator MoveEnemies()
  {
    enemiesMoving = true;
    // wtf?
    yield return new WaitForSeconds(turnDelay);
    if (enemies.Count == 0)
    {
      // wtf?
      yield return new WaitForSeconds(turnDelay);
    }

    for (int i = 0; i < enemies.Count; i++)
    {
      enemies[i].MoveEnemy();
      yield return new WaitForSeconds(enemies[i].moveTime);
    }

    playersTurn = true;
    enemiesMoving = false;
  }
}
