using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamage : MonoBehaviour
{
  public Animator anim;

  // Update is called once per frame
  void Update()
  {
        
  }

  private void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.gameObject.CompareTag("Player"))
    {
      collision.gameObject.GetComponent<PlayerController>().PlayerTakeDamage();
      anim.SetBool("enemyInRange", true);
    }
  }

  private void OnCollisionStay2D(Collision2D collision)
  {
    if (collision.gameObject.CompareTag("Player"))
    {
      collision.gameObject.GetComponent<PlayerController>().PlayerTakeDamage();
      anim.SetBool("enemyInRange", true);
    }
  }

  private void OnCollisionExit2D(Collision2D collision)
  {
    anim.SetBool("enemyInRange", false);
  }
}
