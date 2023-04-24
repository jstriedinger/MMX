using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroFeet : MonoBehaviour
{
    private Zero zero;
    private BoxCollider2D col;
    // Start is called before the first frame update
    void Start()
    {
        zero = GetComponentInParent<Zero>();
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            Debug.Log("begin landing");
            //zero.triggerLanding();
        }
    }
    public bool isGrounded()
    {
        return col.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }
}
