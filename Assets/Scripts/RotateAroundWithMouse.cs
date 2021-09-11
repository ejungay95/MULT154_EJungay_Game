using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundWithMouse : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {
        
  }

  // Update is called once per frame
  void Update()
  {
    Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
    Vector3 dir = Input.mousePosition - pos;
    float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) + 90f;
    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
  }
}
