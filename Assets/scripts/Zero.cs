using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zero : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpSpeed;
    [Range(0.1f, 0.5f)]
    [SerializeField] float dashTime;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject death_vfx;

    [SerializeField] AudioClip[] sfx_jumps;
    [SerializeField] AudioClip sfx_dashBegins;
    [SerializeField] AudioClip sfx_dashStops;
    [SerializeField] AudioClip sfx_death;
    [SerializeField] AudioClip[] sfx_attacks;
    [SerializeField] AudioClip sfx_defaultAttack;
    [SerializeField] AudioClip sfx_beamdown;
    [SerializeField] AudioClip[] sfx_damage;
    [SerializeField] AudioClip sfx_landing;



    float NextFire;
    float shootCooldown = 0.3f;
    public bool pause = false;

    Collider2D myCollider;
    //BoxCollider2D myFeet;
    Rigidbody2D myBody;
    Animator myAnimator;
    Animator dasher;
    AudioSource myAudio;
    GameManager myManager;

    float startTime = 0f;
    float direction = 1f;
    bool inAir = false;
    bool isTakingOf = false;
    bool isLanding = false;

    private bool isAttacking = false;
    private int attackCount = 0;
    private bool canChainCombo = true;

    private float attackTimer = 0f;
    private float attackCooldown = 5f;

    bool introduction = true;

    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<CapsuleCollider2D>();
        myBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        //myFeet = GetComponent<BoxCollider2D>();
        dasher = transform.GetChild(0).GetComponent<Animator>();
        myAudio = GetComponent<AudioSource>();

        myManager = (GameObject.Find("GameManager")).GetComponent<GameManager>();
        pause = false;
        PlaySound(sfx_beamdown);
        direction = transform.localScale.x;


    }

    // Update is called once per frame
    void Update()
    {
        if(!pause && !introduction)
        {
            Run();
            Jump();
            Fire();
            Dash();
            Attack();
        }
        
    }
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    public void PlaySound(AudioClip sfx)
    {
        myAudio.PlayOneShot(sfx);
    }

    private void Fire()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            float direction = transform.localScale.x;
            GameObject bulletObject = Instantiate(bullet, transform.position + new Vector3(direction,-0.3f), transform.rotation);
            Disparo myBullet = bulletObject.GetComponent<Disparo>();

            myBullet.Shoot(direction, speed * 2);
            myAnimator.SetLayerWeight(1, 1);

            NextFire = Time.time + shootCooldown;
        }
        else if(Time.time > NextFire)
            myAnimator.SetLayerWeight(1, 0);
    }

    private void Run()
    {
        float h = Input.GetAxisRaw("Horizontal");
        bool isMoving = h != 0;
        myBody.velocity = new Vector2(h * speed, myBody.velocity.y);

        myAnimator.SetBool("running", isMoving);

        if (isMoving)
        {
            isLanding = false;
            transform.localScale = new Vector2(Mathf.Sign(h), 1);
            direction = transform.localScale.x;
        }
            
    }

    private void Attack()
    {
        
        if (Input.GetKeyDown(KeyCode.C) && attackCount < 3 && canChainCombo)
        {
            isAttacking = true;
            attackCount++;
            attackTimer = 0f;
            Debug.Log("Attack phase: "+attackCount);

            if (attackCount == 1)
            {
                myAnimator.SetInteger("AttackCount", attackCount);
                //trigger attack state
                myAnimator.SetBool("Attack", true);
                Debug.Log("Begin combo");

            }
        }

        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                Debug.Log("Out of time. Cant chain combo");
                //run out of time to next attack
                canChainCombo = false;
                //myAnimator.SetInteger("AttackCount", attackCount);
            }
        }
        
    }

    public void ContinueCombo(int from)
    {
        Debug.Log("lets try to continue the combo from: "+from);
        if(from == 1  && attackCount > 1 && isAttacking)
        {
            Debug.Log("go to attack 2");
            myAnimator.SetInteger("AttackCount", 2);
        }
        else if(from == 2 && attackCount > 2 && isAttacking)
        {
            Debug.Log("go to attack 3");
            myAnimator.SetInteger("AttackCount", 3);
            canChainCombo = false;
        }
        else
        {
            canChainCombo = false;
        }
        
    }

    public void LastChanceCombo(int from)
    {
        if(canChainCombo)
        {
            if (from == 1 && attackCount > 1)
            {
                myAnimator.SetInteger("AttackCount", 2);
            }
            else if (from == 2 && attackCount > 2)
            {
                myAnimator.SetInteger("AttackCount", 3);
            }
            else
            {
                myAnimator.SetBool("Attack", false);
                attackCount = 0;
                myAnimator.SetInteger("AttackCount", 0);
                attackTimer = 0;
                isAttacking = false;
                canChainCombo = true;
            }

        }
    }

    private void Jump()
    {
        bool inGround = isGrounded();
        if(inGround)
        {
            Debug.Log("in ground");
            myAnimator.SetBool("jumping", false);
            inAir = false;

            if( Input.GetButtonDown("Jump") && !isLanding)
            {
                Debug.Log("Begin jump");
                //PlaySound(sfx_jumping);
                myBody.velocity += new Vector2(0, jumpSpeed);
                myAnimator.SetTrigger("takeof");
                isTakingOf = true;
                int r = Random.Range(0, sfx_jumps.Length);
                PlaySound(sfx_jumps[r]);
            }
            else
            {
                if(myBody.velocity.y < -0.01f && !isLanding)
                {
                    Debug.Log("Begin fall animation");
                    myAnimator.SetTrigger("landing");
                    isLanding = true;
                    PlaySound(sfx_landing);
                }
                
            }
            
        }
        else
        {
            inAir = true;
        }

        //start falling
        if(inAir && myBody.velocity.y <= 0 && !isTakingOf)
        {
            Debug.Log("falling from the air!");
            isTakingOf = false;
            myAnimator.SetBool("jumping", true);
        }
        

    }

    private bool isGrounded()
    {
        //return myFeet.IsTouchingLayers(LayerMask.GetMask("Ground"));
        RaycastHit2D ray = Physics2D.Raycast(myCollider.bounds.center, Vector2.down, myCollider.bounds.extents.y + 0.2f, LayerMask.GetMask("Ground"));
        Debug.DrawRay(myCollider.bounds.center, Vector2.down * (myCollider.bounds.extents.y + 0.2f), Color.green);
        return (ray.collider != null);

    }


    private void Dash()
    {
        if(Input.GetKeyDown(KeyCode.X) && isGrounded())
        {
            myAudio.Stop();
            myAudio.Play();
            Debug.Log("Play dash audio");
            //PlaySound(sfx_dashBegins);
            startTime = Time.time;
            dasher.SetTrigger("dashing");
        }
        if(Input.GetKey(KeyCode.X) && isGrounded())
        {
            if(startTime + dashTime >= Time.time)
            {
                myAnimator.SetBool("dashing", true);
                myBody.velocity += new Vector2(speed * 1.5f * direction, 0);
            }
            else
            {
                myAnimator.SetBool("dashing", false);
            }
        }
        if(Input.GetKeyUp(KeyCode.X) && isGrounded() && myAnimator.GetBool("dashing") )
        {
            Debug.Log("Stop dash audio");
            myAudio.Stop();
            myAnimator.SetBool("dashing", false);
            myBody.velocity = Vector2.zero;

        }
    }

    public void StopDash()
    {
        myAudio.PlayOneShot(sfx_dashStops);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        if(tag == "Item")
        {
            myManager.ReduceHearts();
            Destroy(collision.gameObject);
            //PlaySound(sfx_item);
        }
        else if(tag == "Enemy Bullet")
        {
            StartCoroutine(DIE());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(introduction && collision.gameObject.tag == "Ground")
        {
            //moe to intro animation part 2
            myAnimator.Play("Intro 1");
        }
        else
        {
            string tag = collision.gameObject.tag;
            if(tag == "Spikes" || tag == "Enemy"  || tag == "Flying Enemy")
                StartCoroutine(DIE());

        }
    }

    IEnumerator DIE()
    {
        myAnimator.SetBool("dead", true);
        pause = true;
        myBody.isKinematic = true;
        myBody.velocity = Vector3.zero;
        yield return new WaitForSeconds(1);
        AudioSource.PlayClipAtPoint(sfx_death, Camera.main.transform.position);
        Instantiate(death_vfx, transform.position + new Vector3(0,1,0), transform.rotation);
        myManager.GameOver();
        Destroy(gameObject);
    }

    public void AnimateFromTakeofToJump()
    {
        if(! isGrounded() && isTakingOf)
        {
            myAnimator.SetBool("jumping", true);
            isTakingOf = false;

        }
    }

    public void PlayerIsReady()
    {
        myAnimator.SetTrigger("Ready");
        introduction = false;
    }

    public void EndLanding()
    {
        isLanding = false;
        myAnimator.ResetTrigger("landing");
    }

    public void PlayAttackSound()
    {
        PlaySound(sfx_defaultAttack);
        if(attackCount == 1)
        {
            PlaySound(sfx_attacks[0]);
        }
        else if(attackCount == 2 )
        {
            PlaySound(sfx_attacks[1]);
        }
        else if (attackCount == 3)
        {
            PlaySound(sfx_attacks[2]);
        }
    }

    public void ResetTakeOf()
    {
        myAnimator.ResetTrigger("takeof");
    }

    public void FinishAttacking()
    {
        Debug.Log("Finishing attack num " + attackCount);
        myAnimator.SetBool("Attack", false);
        attackCount = 0;
        myAnimator.SetInteger("AttackCount", 0);
        attackTimer = 0;
        isAttacking = false;
        canChainCombo = true;
    }


}
