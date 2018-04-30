using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
  public int playerDamage;
  private Animator animator;
  private Transform target; // store player's position so enemy can chase
  private bool skipMove; // enemies move every other turn
  public AudioClip enemyAttack1;
  public AudioClip enemyAttack2;

  protected override void OnCantMove<T>(T component)
  {
    Player hitPlayer = component as Player;
    animator.SetTrigger("enemyAttack");
    SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    hitPlayer.LoseFood(playerDamage);
  }

  // Use this for initialization
  protected override void Start()
  {
    GameManager.instance.AddEnemyToList(this);
    animator = GetComponent<Animator>();
    target = GameObject.FindGameObjectWithTag("Player").transform;
    base.Start();
  }

  protected override void AttemptMove<T>(int xDir, int yDir)
  {
    if (skipMove)
    {
      skipMove = false;
      return;
    }
    base.AttemptMove<T>(xDir, yDir);
    skipMove = true;
  }

  public void MoveEnemy()
  {
    int xDir = 0;
    int yDir = 0;
    // are the x coords in the same column?
    if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
    {
      // move in direction of player up or down on column
      yDir = target.position.y > transform.position.y ? 1 : -1;
    }
    else
    {
      xDir = target.position.x > transform.position.x ? 1 : -1;
    }

    // WHY?
    AttemptMove<Player>(xDir, yDir);
  }
}
