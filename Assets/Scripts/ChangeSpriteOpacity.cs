using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpriteOpacity : MonoBehaviour
{
  private SpriteRenderer spriteRenderer;

  // Start is called before the first frame update
  void Start()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();      
  }

  // Update is called once per frame
  void Update()
  {
        
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if(collision.CompareTag("Player"))
    {
      spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, .8f);
    }
  }

  private void OnTriggerExit2D(Collider2D collision)
  {
    if (collision.CompareTag("Player"))
    {
      spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }
  }
}
