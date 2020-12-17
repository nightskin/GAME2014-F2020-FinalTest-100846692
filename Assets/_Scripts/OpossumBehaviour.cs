using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum RampDirection
{
    NONE,
    UP,
    DOWN
}


public class OpossumBehaviour : MonoBehaviour
{
    [Header("Movement")]
    public float runForce;
    public Rigidbody2D rigidbody2D;
    public Transform lookInFrontPoint;
    public Transform lookAheadPoint;
    public LayerMask collisionGroundLayer;
    public LayerMask collisionWallLayer;
    public bool isGroundAhead;
    public bool onRamp;
    public RampDirection rampDirection;

    [Header("AI")]
    public LOS opossumLOS;

    [Header("Bullet Firing")] 
    public Transform bulletSpawn;
    public float fireDelay;
    public PlayerBehaviour player;

    [Header("Abilities")]
    public int health;
    public BarController healthBar;

    [Header("Animation")] 
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private AudioSource hitSound;

    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        rigidbody2D = GetComponent<Rigidbody2D>();
        rampDirection = RampDirection.NONE;
        player = GameObject.FindObjectOfType<PlayerBehaviour>();
        hitSound = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_hasLOS())
        {
            _FireBullet();
        }

        _LookInFront();
        _LookAhead();
        _Move();
    }

    private void _FireBullet()
    {
        //delay bullet firing
        if (Time.frameCount % fireDelay == 0 && BulletManager.Instance().HasBullets(PoolType.ENEMY))
        {
            var playerPosition = player.transform.position;
            var firingDirection = Vector3.Normalize(playerPosition - bulletSpawn.position);

            BulletManager.Instance().GetBullet(PoolType.ENEMY, bulletSpawn.position, firingDirection);
        }

    }

    private bool _hasLOS()
    {
        if (opossumLOS.colliders.Count > 0)
        {
            if (opossumLOS.collidesWith.gameObject.name == "Player" || opossumLOS.colliders[0].gameObject.name == "Player")
            {
                return true;
            }
        }
        return false;
    }

    private void _LookInFront()
    {
        var wallHit = Physics2D.Linecast(transform.position, lookInFrontPoint.position, collisionWallLayer);
        if (wallHit)
        {
            if (!wallHit.collider.CompareTag("Ramps"))
            {
                if (!onRamp && transform.rotation.z == 0.0f)
                {
                        _FlipX();
   
                }

                rampDirection = RampDirection.DOWN;
            }
            else
            {
                rampDirection = RampDirection.UP;
            }
        }

        Debug.DrawLine(transform.position, lookInFrontPoint.position, Color.red);
    }

    private void _LookAhead()
    {
        var groundHit = Physics2D.Linecast(transform.position, lookAheadPoint.position, collisionGroundLayer);
        if (groundHit)
        {
            if (groundHit.collider.CompareTag("Ramps"))
            {
                onRamp = true;
            }

            if (groundHit.collider.CompareTag("Platforms"))
            {
                onRamp = false;
            }

            isGroundAhead = true;
        }
        else
        {
            isGroundAhead = false;
        }

        Debug.DrawLine(transform.position, lookAheadPoint.position, Color.green);
    }


    private void _Move()
    {
        if (isGroundAhead)
        {
            rigidbody2D.AddForce(Vector2.left * runForce * Time.deltaTime * transform.localScale.x);

            if (onRamp)
            {
                if (rampDirection == RampDirection.UP)
                {
                  rigidbody2D.AddForce(Vector2.up * runForce * 0.5f * Time.deltaTime);
                }
                else
                { 
                    rigidbody2D.AddForce(Vector2.down * runForce * 0.25f * Time.deltaTime);
                }

                StartCoroutine(Rotate());
            }
            else
            {
                StartCoroutine(Normalize());
            }
            

            rigidbody2D.velocity *= 0.90f;
        }
        else if (onRamp)
        {
            StartCoroutine(Rotate());
        }
        else
        {
            _FlipX();
        }
       
    }

    IEnumerator Rotate()
    {
        yield return new WaitForSeconds(0.05f);
        if ((transform.localScale.x == 1.0f) && (rampDirection == RampDirection.UP))
        {
            // left and up
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, -26.0f);
        }
        else if ((transform.localScale.x == 1.0f) && (rampDirection == RampDirection.DOWN))
        {
            // left and down
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 26.0f);
        }
        else if ((transform.localScale.x == -1.0f) && (rampDirection == RampDirection.UP))
        {
            // right and up
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 26.0f);
        }
        else if ((transform.localScale.x == -1.0f) && (rampDirection == RampDirection.DOWN))
        {
            // right and down
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, -26.0f);
        }
    }

    IEnumerator Normalize()
    {
        yield return new WaitForSeconds(0.05f);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    }

    private void _FlipX()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1.0f, transform.localScale.y, transform.localScale.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("DeathPlane"))
        {
            Dead();
        }

        if (other.gameObject.CompareTag("Acorn"))
        {
            TakeDamage(50);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.SetValue(health);
        hitSound.Play();
        

        if (health <= 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
