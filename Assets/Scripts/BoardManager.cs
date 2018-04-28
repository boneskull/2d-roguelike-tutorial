﻿using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
  [Serializable]
  public class Count
  {
    public int minimum;
    public int maximum;
    public Count(int min, int max)
    {
      minimum = min;
      maximum = max;
    }
  }

  public int columns = 8;
  public int rows = 8;

  // min of 5 walls, max of 9 walls per level
  public Count wallCount = new Count(5, 9);
  public Count foodCount = new Count(1, 5);
  public GameObject exit;
  public GameObject[] floorTiles;
  public GameObject[] wallTiles;
  public GameObject[] foodTiles;
  public GameObject[] enemyTiles;
  public GameObject[] outerWallTiles;

  private Transform boardHolder;
  // where the objects is at
  private List<Vector3> gridPositions = new List<Vector3>();

  void InitializeList()
  {
    gridPositions.Clear();

    // the offset is for the outer walls
    for (int x = 1; x < columns - 1; x++)
    {
      for (int y = 1; y < rows - 1; y++)
      {
        gridPositions.Add(new Vector3(x, y, 0f));
      }
    }
  }

  void BoardSetup()
  {
    boardHolder = new GameObject("Board").transform;

    // build floor

    // edge around active game board using outer wall objects
    for (int x = -1; x < columns + 1; x++)
    {
      for (int y = -1; y < rows + 1; y++)
      {
        // pick some random floor tile
        GameObject toInstantiate = (x == -1 || x == columns || y == -1 || y == rows)
          ? outerWallTiles[Random.Range(0, outerWallTiles.Length)]
          : floorTiles[Random.Range(0, floorTiles.Length)];
        GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
        instance.transform.SetParent(boardHolder);
      }
    }
  }

  Vector3 RandomPosition()
  {
    int randomIndex = Random.Range(0, gridPositions.Count);
    Vector3 randomPosition = gridPositions[randomIndex];
    gridPositions.RemoveAt(randomIndex);
    return randomPosition;
  }

  void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
  {
    // how many of a given object will be spawned
    int objectCount = Random.Range(minimum, maximum + 1);
    for (int i = 0; i < objectCount; i++)
    {
      Vector3 randomPosition = RandomPosition();
      GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
      Instantiate(tileChoice, randomPosition, Quaternion.identity);
    }
  }

  public void SetupScene(int level)
  {
    BoardSetup();
    InitializeList();
    LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
    LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

    // logarithmic ramp up of enemy count for difficulty
    int enemyCount = (int)Mathf.Log(level, 2f);
    LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
    Instantiate(exit, new Vector3(columns - 1, rows - 1, 0F), Quaternion.identity);
  }

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
}
