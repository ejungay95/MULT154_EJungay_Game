using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWrapAround : MonoBehaviour
{
  public Transform player;
  public float xConstraint = 17.5f;
  public float yConstraint = 10f;

  // Start is called before the first frame update
  void Start()
  {
        
  }

  // Update is called once per frame
  void Update()
  {
    if(player.transform.position.x > xConstraint)
    {
      player.transform.position = new Vector2(-xConstraint, player.transform.position.y);
    }
    else if(player.transform.position.x < -xConstraint)
    {
      player.transform.position = new Vector2(xConstraint, player.transform.position.y);
    }

    if(player.transform.position.y > yConstraint)
    {
      player.transform.position = new Vector2(player.transform.position.x, -yConstraint);
    }
    else if(player.transform.position.y < -yConstraint)
    {
      player.transform.position = new Vector2(player.transform.position.x, yConstraint);
    }
  }
}
