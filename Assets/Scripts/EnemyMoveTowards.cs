using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveTowards : MonoBehaviour
{
  public Transform target;
  public float speed = 10f;
  public Animator anim;

  private Vector2 movement;

  private Rigidbody2D rb;
  // Start is called before the first frame update
  void Start()
  {
    target = GameObject.FindGameObjectWithTag("Player").transform;
    rb = GetComponent<Rigidbody2D>();

    Vector3 dir = target.position - transform.position;

    dir.Normalize();
    movement = dir;
  }

  // Update is called once per frame
  void Update()
  {
    
  }

  private void FixedUpdate()
  {
    MoveTowardsPlayer(movement);
  }

  private void MoveTowardsPlayer(Vector2 direction)
  {
    rb.MovePosition((Vector2)transform.position + (direction * speed * Time.fixedDeltaTime));
  }

  private void OnCollisionEnter2D(Collision2D collision)
  {
    if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Projectile")
    {
      Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }
    anim.SetTrigger("hitSomething");
    Destroy(gameObject, anim.GetCurrentAnimatorStateInfo(0).length);
  }
}
