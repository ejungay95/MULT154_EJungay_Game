using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
  public Transform target;
  //public Transform enemyGFX;
  public GameObject leftCheck;
  public GameObject rightCheck;
  public GameObject enemyGFX;
  public GameObject projectilePrefab;
  public Animator anim;
  public float speed = 200f;
  public LayerMask layers;

  float speedTemp;

  [Header("Waypoint modifiers")]
  public float nextWaypointDist = 3f;
  public float activationDistance = 5.0f;
  public float pathUpdateInSeconds = 0.5f;

  private Path path;
  private int currentWaypoint = 0;
  private bool reachedEndOfPath = false;

  [Header("Attack modifiers")]
  public float attackRange = 3.5f;
  public float fireRate = 1.0f;

  private float fireTimer = 0f;
  private bool canFire = true;

  [Header("Jump modifiers")]
  public float jumpHeightLimit = 0.8f;
  public float jumpForce = 1f;
  public float jumpCheckOffset = 0.1f;
  public float raycastDist = .1f; // Distance to check for holes on the left and right side of the enemy
  public Transform groundCheck;
  public float groundRadius = 0.0025f;
  public LayerMask whatIsGround;

  private const float JUMP_TIME = 2f;
  private float jumpCounter = JUMP_TIME;
  private bool isGrounded = false;
  private bool canJump = true;

  [Header("Wander modifiers")]
  public float wanderRepeatTime = .5f;
  public float wanderRadius = .5f;

  private Vector2 wanderVector;
  private float wanderCount = 0f;

  [Header("Behaviors")]
  public bool followEnabled = true;
  public bool jumpEnabled = true;
  public bool isAFlyer = true;
  public bool rangedAttacker = false;
  
  private Seeker seeker;
  private Rigidbody2D rb;
  private int lives = 1;
  private CapsuleCollider2D col;

  // Start is called before the first frame update
  void Start()
  {
    target = GameObject.FindGameObjectWithTag("Player").transform;
    seeker = GetComponent<Seeker>();
    rb = GetComponent<Rigidbody2D>();
    col = GetComponent<CapsuleCollider2D>();
    speedTemp = speed;
    if (!isAFlyer)
    {
      wanderVector = Vector2.left;
    }

    InvokeRepeating("UpdatePath", 0f, pathUpdateInSeconds); 
  }

  private void Update()
  {
    // Cooldown for enemy jump
    if (!canJump && jumpEnabled)
    {
      if(jumpCounter > 0)
      {
        jumpCounter -= Time.deltaTime;
      } else
      {
        canJump = true;
        jumpCounter = JUMP_TIME;
      }
    }

    if(!TargetInDistance())
    {
      if(isAFlyer)
      {
        // Pick random point to simulate wandering
        if (wanderCount < wanderRepeatTime)
        {
          wanderCount += Time.deltaTime;
        } else
        {
          wanderVector = PickRandomPointToWander();
          wanderCount = 0;
        }
      }
    }

    if(isAFlyer)
    {
      if (rangedAttacker)
      {
        if (!canFire)
        {
          fireTimer += Time.deltaTime;
          if (fireTimer >= fireRate)
          {
            canFire = true;
            fireTimer = 0f;
          }
        }
      }
    }   

    if(lives == 0)
    {
      anim.SetBool("isDead", true);
      rb.velocity = Vector2.zero;
      col.enabled = false;
      Destroy(gameObject, anim.GetCurrentAnimatorStateInfo(0).length);
    }
    anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

    CheckIfGrounded();
  }

  // Update is called once per frame
  void FixedUpdate()
  {
    if (followEnabled && TargetInDistance())
    {
      FollowThePath();
    } else if (!TargetInDistance())
    {
      Wander();
      FlipSprite();
    }
  }

  void FollowThePath()
  {
    if (path == null)
    {
      return;
    }
    if (currentWaypoint >= path.vectorPath.Count)
    {
      reachedEndOfPath = true;
      return;
    } else
    {
      reachedEndOfPath = false;
    }

    Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
    float dist = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

    // Do the jump
    if (!isAFlyer && jumpEnabled && isGrounded && canJump)
    {
      // Player is above the enemy, so jump
      if (direction.y > jumpHeightLimit)
      {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        canJump = false;
      } else if (!NoGroundAhead())
      {
        //if (direction.y > jumpHeightLimit)
        //{
          // There is no ground ahead so jump over
          rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
          canJump = false;
        //}
      }
    }

    // Move the enemy
    MoveTheEnemy(direction);

    if (dist < nextWaypointDist)
    {
      currentWaypoint++;
    }

    FlipSprite();
  }

  void OnPathComplete(Path p)
  {
    if(!p.error)
    {
      path = p;
      currentWaypoint = 0;
    }
  }

  void UpdatePath()
  {
    if(TargetInDistance() && seeker.IsDone())
    {
      seeker.StartPath(rb.position, target.position, OnPathComplete);
    }
  }

  void CheckIfGrounded()
  {
    //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
    isGrounded = col.IsTouchingLayers(whatIsGround);
  }

  bool TargetInDistance()
  {
    return Vector2.Distance(transform.position, target.transform.position) < activationDistance;
  }

  void MoveTheEnemy(Vector2 direction)
  {
    // move the enemy based on if its a flyer or walks on ground
    if (isAFlyer)
    {
      rb.AddForce(direction * speed * Time.fixedDeltaTime);

      if(rangedAttacker)
      {
        if(HasLineOfSight())
        { 
          // Stop moving when in attack range
          // dont know how to stop a path so just make speed 0  
          speed = 0;
          rb.velocity = Vector2.zero;
          if(canFire && lives != 0)
          {
            anim.SetBool("enemyInRange", true);
            GameObject temp = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            canFire = false;
          }
          
        } else
        {
          anim.SetBool("enemyInRange", false);
          speed = speedTemp;
        }
      }
    } else
    {
      if(anim.GetBool("enemyInRange"))
      {
        speed = 0;
        rb.velocity = Vector2.zero;
      } else
      {
        speed = speedTemp;
      }
      rb.AddForce(new Vector2(direction.x, 0) * speed * Time.fixedDeltaTime);
    }
  }

  bool TargetInAttackRange(float range)
  {
    return Vector2.Distance(transform.position, target.transform.position) <= range;
  }

  bool HasLineOfSight()
  {
    Vector2 dir = (target.position - transform.position).normalized;
    RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, dir, attackRange, layers);

    if(hitInfo.collider != null)
    {
      if (hitInfo.collider.tag == "Player")
      {
        return true;
      } else
      {
        return false;
      }
    } else
    {
      return false;
    }  
  }

  bool NoGroundAhead()
  {
    RaycastHit2D leftInfo = Physics2D.Raycast(leftCheck.transform.position, -Vector2.up, raycastDist);
    RaycastHit2D rightInfo = Physics2D.Raycast(rightCheck.transform.position, -Vector2.up, raycastDist);

    // hit nothing aka a hole
    if(isGrounded)
    {
      if (IsFacingRight())
      {
        if (rightInfo.collider != null)
        {
          return true;
        }
      } else
      {
        if (leftInfo.collider != null)
        {
          return true;
        }
      }
    }
    return false;
  }

  bool CheckAhead()
  {
    RaycastHit2D leftInfo = Physics2D.Raycast(leftCheck.transform.position, Vector2.left, raycastDist);
    RaycastHit2D rightInfo = Physics2D.Raycast(rightCheck.transform.position, Vector2.right, raycastDist);

    // hit a wall
    if(IsFacingRight())
    {
      if(rightInfo.collider != null)
      {
        return true;
      }
    } else
    {
      if(leftInfo.collider != null)
      {
        return true;
      }   
    }

    return false;
  }

  void Wander()
  {
    if(!isAFlyer)
    {
      // Wander for walkers
      // Move left until they hit a wall
      // Then turn around and keep moving
      if (!NoGroundAhead() && canJump && isGrounded)
      {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        canJump = false;
      }
      if (CheckAhead())
      {
        if(IsFacingRight())
        {
          wanderVector = Vector2.left;
        } else
        {
          wanderVector = Vector2.right;
        }
      }
      rb.AddForce(wanderVector * speed * Time.fixedDeltaTime);
     
    } else
    {
      // Wander for flyers
      // Pick a random point near the flyer and move that direction
      rb.AddForce(wanderVector * speed * Time.fixedDeltaTime);
    }
  }

  Vector2 PickRandomPointToWander()
  {
    return Random.insideUnitCircle;
  }

  bool IsFacingRight()
  {
    // check if x scale is positive for facing right
    return enemyGFX.GetComponent<SpriteRenderer>().flipX == true ? false:true;
  }

  void FlipSprite()
  {
    if(rb.velocity.x > Mathf.Epsilon)
    {
      enemyGFX.GetComponent<SpriteRenderer>().flipX = false;
    } else if(rb.velocity.x < -Mathf.Epsilon)
    {
      enemyGFX.GetComponent<SpriteRenderer>().flipX = true;
    }
  }
  public void SubtractHealth()
  {
    lives--;
  }
}
