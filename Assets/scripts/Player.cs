using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpSpeed;
    [Range(0.1f, 0.5f)]
    [SerializeField] float dashTime;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject death_vfx;

    [SerializeField] AudioClip sfx_bullet;
    [SerializeField] AudioClip sfx_jumping;
    [SerializeField] AudioClip sfx_dash;
    [SerializeField] AudioClip sfx_item;
    [SerializeField] AudioClip sfx_death;



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
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!pause)
        {
            Run();
            Jump();
            Fire();
            Dash();
        }
        
    }

    public void PlaySound(AudioClip sfx)
    {
        myAudio.PlayOneShot(sfx);
    }

    private void Fire()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PlaySound(sfx_bullet);
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
        float h = Input.GetAxis("Horizontal");
        bool isMoving = (Mathf.Abs(h) > 0);
        myBody.velocity = new Vector2(h * speed, myBody.velocity.y);

        myAnimator.SetBool("running", isMoving);
        
        if (isMoving)
        {
            direction = Mathf.Sign(h);
            transform.localScale = new Vector2(direction, 1);
        }
            
    }

    private void Jump()
    {
        bool inGround = isGrounded();

        if(inGround)
        {
            if (Input.GetButtonDown("Jump"))
            {
                PlaySound(sfx_jumping);
                myBody.velocity += new Vector2(0, jumpSpeed);
                myAnimator.SetTrigger("takeof");
            }
            myAnimator.SetBool("jumping", false);
        }
        else
            myAnimator.SetBool("jumping", true);

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
            PlaySound(sfx_dash);
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
                myAnimator.SetBool("dashing", false);
        }
        else
            myAnimator.SetBool("dashing", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        if(tag == "Item")
        {
            myManager.ReduceHearts();
            Destroy(collision.gameObject);
            PlaySound(sfx_item);
        }
        else if(tag == "Enemy Bullet")
        {
            StartCoroutine(DIE());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;
        if(tag == "Spikes" || tag == "Enemy"  || tag == "Flying Enemy")
            StartCoroutine(DIE());
    }

    IEnumerator DIE()
    {
        myAnimator.SetBool("dead", true);
        pause = true;
        myBody.isKinematic = true;
        myBody.velocity = Vector3.zero;
        yield return new WaitForSeconds(1);
        Instantiate(death_vfx, transform.position, transform.rotation);
        AudioSource.PlayClipAtPoint(sfx_death, Camera.main.transform.position);
        myManager.GameOver();
        Destroy(gameObject);
    }


}
