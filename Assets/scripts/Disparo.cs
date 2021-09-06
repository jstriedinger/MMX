using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disparo : MonoBehaviour
{
    float speed;
    bool moving = false;
    float dir = 1f;

    Animator myAnimator;

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(moving)
        {
            transform.Translate(new Vector3(speed * Time.deltaTime * dir, 0, 0));
        }
    }

    public void Shoot(float direction, float _speed)
    {
        moving = true;
        speed = _speed;
        dir = direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string layer = LayerMask.LayerToName(collision.gameObject.layer);
        string tag = collision.gameObject.tag;

        if(layer == "Ground" )
        {
            moving = false;
            myAnimator.SetTrigger("collision");
        }
        else if( tag == "Enemy")
        {
            moving = false;
            myAnimator.SetTrigger("collision");
            collision.gameObject.GetComponent<StaticEnemy>().DIE();
        }
        else if(tag == "Flying Enemy")
        {
            moving = false;
            myAnimator.SetTrigger("collision");
            collision.gameObject.GetComponent<FlyingEnemy>().DIE();
        }
        

    }

   

    public void DIE()
    {
        Destroy(gameObject);
    }
}
