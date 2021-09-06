using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FlyingEnemy : MonoBehaviour
{
    [SerializeField] AudioClip sfx_death;

    Animator myAnimator;
    AIPath myAiPath;
    CircleCollider2D myCollider;
    bool canMove = true;


    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myCollider = GetComponent<CircleCollider2D>();
        myAiPath = GetComponent<AIPath>();
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove)
            myAiPath.enabled = Physics2D.OverlapCircle(transform.position, 10f, LayerMask.GetMask("Player"));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255,0,0,0.3f);
        Gizmos.DrawSphere(transform.position, 10f);
    }


    public void DIE()
    {
        AudioSource.PlayClipAtPoint(sfx_death, Camera.main.transform.position);
        myAiPath.enabled = false;
        canMove = false;
        myCollider.enabled = false;
        myAnimator.SetTrigger("Dead");
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}
