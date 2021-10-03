using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveTowards : MonoBehaviour
{
  public Transform target;
  public float speed = 10f;

  private float followRange;
  private Vector2 movement;

  private Rigidbody2D rb;
  // Start is called before the first frame update
  void Start()
  {
    target = GameObject.FindGameObjectWithTag("Player").transform;
    rb = GetComponent<Rigidbody2D>();
  }

  // Update is called once per frame
  void Update()
  {
    Vector3 dir = target.position - transform.position;

    followRange = Vector2.Distance(target.position, transform.position);


    dir.Normalize();
    movement = dir;
  }

  private void FixedUpdate()
  {
    if (followRange < 25f)
    {
      MoveTowardsPlayer(movement);
    }
  }

  private void MoveTowardsPlayer(Vector2 direction)
  {
    rb.MovePosition((Vector2)transform.position + (direction * speed * Time.fixedDeltaTime));
  }
}
