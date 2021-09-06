using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnemy : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] AudioClip sfx_death;

    Animator myAnimator;
    BoxCollider2D myCollider;

    float nextFire;
    float fireCooldown = 1f;
    bool canFire = true;

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(DetectPlayer() && Time.time > nextFire && canFire )
        {
            Fire();
        }
    }

    private bool DetectPlayer()
    {
        Vector3 origin = transform.position;
        Vector3 destination = Vector2.left;

        RaycastHit2D myRaycast = Physics2D.Raycast(origin, destination, 20f, LayerMask.GetMask("Player"));
        Debug.DrawRay(origin, new Vector3(-20f, 0), Color.red);

        return (myRaycast.collider != null);
    }

    private void Fire()
    {
        Instantiate(bullet, transform.position + new Vector3(0,0.2f), transform.rotation);
        nextFire = Time.time + fireCooldown;
    }

    public void DIE()
    {
        canFire = false;
        AudioSource.PlayClipAtPoint(sfx_death, Camera.main.transform.position);
        myAnimator.SetTrigger("Dead");
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}
