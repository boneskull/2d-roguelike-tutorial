using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{

  // time it takes for object to move (in seconds)
  public float moveTime = 0.1f;
  // layer for collision check to see if we can move there
  public LayerMask blockingLayer;

  private BoxCollider2D boxCollider;
  private Rigidbody2D rb2D;
  private float inverseMoveTime;

  // Use this for initialization
  protected virtual void Start()
  {
    boxCollider = GetComponent<BoxCollider2D>();
    rb2D = GetComponent<Rigidbody2D>();
    inverseMoveTime = 1f / moveTime;
  }

  protected IEnumerator SmoothMovement(Vector3 end)
  {
    // computationally cheaper than magnitude
    float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
    while (sqrRemainingDistance > float.Epsilon)
    {
      Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
      rb2D.MovePosition(newPosition);
      sqrRemainingDistance = (transform.position - end).sqrMagnitude;
      yield return null;
    }
  }

  // "out" is a reference or something
  protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
  {
    // discard Z-axis
    Vector2 start = transform.position;
    Vector2 end = start + new Vector2(xDir, yDir);
    // don't want to hit our own collider when casting the ray
    boxCollider.enabled = false;
    hit = Physics2D.Linecast(start, end, blockingLayer);
    boxCollider.enabled = true;
    if (hit.transform == null)
    {
      // open
      StartCoroutine(SmoothMovement(end));
      return true;
    }
    return false;
  }

  // T is the thing that we are interacting with if blocked
  protected virtual void AttemptMove<T>(int xDir, int yDir) where T : Component
  {
    RaycastHit2D hit;
    bool canMove = Move(xDir, yDir, out hit);
    if (hit.transform != null)
    {
      T hitComponent = hit.transform.GetComponent<T>();
      if (!canMove && hitComponent != null)
      {
        OnCantMove(hitComponent);
      }
    }
  }

  protected abstract void OnCantMove<T>(T component) where T : Component;
}
