using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
  private const int MAX_JUMP_COUNT = 3;
  private const int MAX_HEALTH = 3;

  public bool isInvulnerable = false;
  public float invulnerabilityTimeInSeconds = .5f;
  public bool isCoroutineRunning = false;
  public Animator anim;
  public Transform shockwaveSpawnLocation;
  public GameObject shockwavePrefab;

  public AudioClip jumpClip;
  public AudioSource audioSource;

  public Image[] hearts;

  public int currentHealth = 0;
  public bool isAlive = true;
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
  public float terminalVel = -45f;
  public int jumpCount = MAX_JUMP_COUNT;
  public LayerMask whatIsGround;
  public Transform groundCheck;
  public float groundRadius = 0.0025f;
  public BoxCollider2D hitBox;

  private Rigidbody2D rb;
  private CapsuleCollider2D col;
  private float horzInput;
  private Vector2 direction;
  private Vector3 mouseDirection;
  public SpriteRenderer sprite;

  // Start is called before the first frame update
  void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    col = GetComponent<CapsuleCollider2D>();
    currentHealth = MAX_HEALTH;
  }

  private void Update()
  {

    horzInput = Input.GetAxis("Horizontal");
    jumpHeld = Input.GetButton("Jump");
    GetMouseLocation();

    direction = new Vector2(horzInput, rb.velocity.y);
    anim.SetFloat("Speed",Mathf.Abs(horzInput));

    // jump code is really janky but works ¯\_("/)_/¯
    if (!Input.GetKey(KeyCode.S))
    {
      if (isGrounded)
      {
        if (Input.GetButtonDown("Jump"))
        {
          jumpPressed = true;
          extraJump = true;
          audioSource.PlayOneShot(jumpClip);
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
            audioSource.PlayOneShot(jumpClip);
            ParticleSystem temp = Instantiate(shockwavePrefab, shockwaveSpawnLocation.position, shockwaveSpawnLocation.rotation).GetComponent<ParticleSystem>();
            temp.Play();
            Destroy(temp.gameObject, temp.main.duration + 1.0f);
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
    DetermineAnimationState();
    FlipSprite();
  }

  // Update is called once per frame
  private void FixedUpdate()
  {
    CheckIfGrounded();

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
          Attack();
          Invoke("Attack", .05f);
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

    // limit terminal velocity
    if(rb.velocity.y <= terminalVel)
    {
      rb.velocity = new Vector2(rb.velocity.x, terminalVel);
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
        Vector2 currentVel = rb.velocity;
        rb.AddForce(new Vector2(direction.x * speed * Time.fixedDeltaTime, 0) - currentVel, ForceMode2D.Force);
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

  private void GetMouseLocation()
  {
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mouseDirection = gameObject.transform.position - mousePos;
    mouseDirection.z = 0.0f;
    mouseDirection = mouseDirection.normalized;
  }

  private void CheckIfGrounded()
  {
    //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
    isGrounded = col.IsTouchingLayers(whatIsGround);
  }

  private void RefreshJumps()
  {
    if(isGrounded)
    {
      // Reset jump flags
      // --- Added condition of only checking when falling
     if(rb.velocity.y < Mathf.Epsilon)
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

  private void Attack()
  {
    hitBox.enabled = !hitBox.enabled;
  }

  private void FlipSprite()
  {
    if(horzInput > 0)
    {
      sprite.flipX = false;
    } else if(horzInput < 0)
    {
      sprite.flipX = true;
    }
  }

  public void PlayerTakeDamage()
  {
    if (!isInvulnerable && currentHealth > 0)
    {
      currentHealth--;
      hearts[currentHealth].enabled = false;
    }
    
    if(currentHealth <= 0)
    {
      currentHealth = 0;
      isAlive = false;
      anim.SetBool("isDead", true);
      return;
    }

    if(!isCoroutineRunning)
    {
      StartCoroutine(PlayerInvulnerableAfterDamage());
    }
  }

  private IEnumerator PlayerInvulnerableAfterDamage()
  {
    isInvulnerable = true;
    isCoroutineRunning = true;

    for (float i = 0; i < invulnerabilityTimeInSeconds; i += .1f)
    {
      if (sprite.color == Color.white)
      {
        sprite.color = Color.red;
      } else
      {
        sprite.color = Color.white;
      }
      yield return new WaitForSeconds(invulnerabilityTimeInSeconds);
    }

    sprite.color = Color.white;
    isInvulnerable = false;
    isCoroutineRunning = false;
  }

  void DetermineAnimationState()
  {
    if(rb.velocity.y > Mathf.Epsilon)
    {
      if(!isGrounded)
      {
        anim.SetBool("isJumping", true);
        anim.SetBool("isAttackJump", true);
      }
      else
      {
        anim.SetBool("isJumping", false);
        anim.SetBool("isAttackJump", false);
      }
    }
    else if(rb.velocity.y < Mathf.Epsilon)
    {
      anim.SetBool("isJumping", false);
      anim.SetBool("isAttackJump", false);
      if(!isGrounded)
      {
        anim.SetBool("isFalling", true);
      } else
      {
        anim.SetBool("isFalling", false);
      }
      
    }
    else if(rb.velocity.y == 0)
    {
      anim.SetBool("isFalling", false);
    }
  }
}
