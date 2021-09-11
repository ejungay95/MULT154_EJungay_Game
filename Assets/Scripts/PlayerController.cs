using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  private const int MAX_JUMP_COUNT = 3;

  public float speed = 10.0f;
  public float jumpForce = 10.0f;
  public float airJumpForce = 15.0f;
  public bool isGrounded = false;
  public bool jumpPressed = false;
  public bool extraJump = false;
  public bool firstJump = false;
  public bool jumpHeld = false;
  public bool fallThrough = false;
  public float fallMultiplier = 2.5f;
  public float lowJumpMultiplier = 2f;
  public int jumpCount = MAX_JUMP_COUNT;
  public LayerMask whatIsGround;
  public Transform groundCheck;
  public float groundRadius = 0.0025f;
  public Transform hitBox;

  private Rigidbody2D rb;
  private BoxCollider2D col;
  private float horzInput;
  private Vector2 direction;

  // Start is called before the first frame update
  void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    col = GetComponent<BoxCollider2D>();
  }

  private void Update()
  {
    horzInput = Input.GetAxis("Horizontal");
    jumpHeld = Input.GetButton("Jump");

    direction = new Vector2(horzInput, rb.velocity.y);

    // jump code is really janky but works ¯\_("/)_/¯
    if(!Input.GetKey(KeyCode.S))
    {
      if (isGrounded)
      {
        if (Input.GetButtonDown("Jump"))
        {
          jumpPressed = true;
          extraJump = true;
        }
      } else
      {
        if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1"))
        {
          jumpCount -= 1;
          if (jumpCount < 0)
          {
            jumpPressed = false;
            extraJump = false;
            jumpCount = 0;
          } else
          {
            jumpPressed = true;
          }
        }
      }
    } else
    {
      if(Input.GetButton("Jump"))
      {
        fallThrough = true;
      } else
      {
        fallThrough = false;
      }
    }

    RefreshJumps();
  }

  // Update is called once per frame
  private void FixedUpdate()
  {
    CheckIfGrounded();

    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector3 mouseDirection = gameObject.transform.position - mousePos;
    mouseDirection.z = 0.0f;
    mouseDirection = mouseDirection.normalized;

    PlayerMovement();

    // Pressed the jump button
    if(jumpPressed)
    {
      // This is my first jump
      if(!firstJump)
      {
        rb.velocity = Vector2.zero;
        //rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        jumpPressed = false;
        extraJump = true;
        firstJump = true;
      }
      else
      {
        // The shockwave jumps
        if(extraJump && jumpCount >= 0)
        {
          rb.velocity = Vector2.zero;

          // push the player away from the direction of the mouse
          rb.AddForce(mouseDirection * airJumpForce , ForceMode2D.Impulse);
          jumpPressed = false;
        }
      }
    }

    // Better jump 
    if (rb.velocity.y < 0)
    {
      rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
    }
    else if (rb.velocity.y > 0 && !jumpHeld)
    {
      rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
    }
  }

  private void OnCollisionStay2D(Collision2D collision)
  {
    if (collision.gameObject.CompareTag("Fall Through") && fallThrough)
    {
      FallThroughPlatform();
      Invoke("FallThroughPlatform", .5f);
    }
  }

  private void PlayerMovement()
  {
    // Need this because of wonky x velocity when doing air jumps
    if (horzInput != 0)
    {
      if (extraJump && jumpCount < MAX_JUMP_COUNT)
      {
        rb.AddForce(new Vector2(direction.x * speed * Time.fixedDeltaTime, 0), ForceMode2D.Force);
      }
      else
      {
        rb.velocity = new Vector2(direction.x * speed * Time.fixedDeltaTime, rb.velocity.y);
      }      
    }

    // Stop sliding
    if(rb.velocity.x != 0 && horzInput == 0 && isGrounded) 
    {
      rb.velocity = new Vector2(0, rb.velocity.y);
    }
  }

  private void CheckIfGrounded()
  {
    isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
  }

  private void RefreshJumps()
  {
    if(isGrounded)
    {
      // Reset jump flags
      // --- Added condition of only checking when falling
      if(rb.velocity.y < 0.2f)
      {
        if(firstJump)
        {
          firstJump = false;
        }
        if(extraJump)
        {
          extraJump = false;
        }
      }
      jumpCount = MAX_JUMP_COUNT;
    }
  }

  private void FallThroughPlatform()
  {
    col.enabled = !col.enabled;
  }
}
