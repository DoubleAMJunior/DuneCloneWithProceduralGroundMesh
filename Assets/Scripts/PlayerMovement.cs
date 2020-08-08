using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static Transform PlayerTransform;
    public float forwardForce;
    public float downwardForce;
    public float loseImpact;
    public float slowImpact;
    public float slowMultiplier;
    
    private const float forwardMultiplier=10;
    private const float downMultiplier = 100;
    private Rigidbody2D rb;
    private Collider2D col;
    private ContactFilter2D filter;
    private float previousVelocity;
    private bool aired;

    private GameObject trail;
    // Start is called before the first frame update
    private void Awake()
    {
        PlayerTransform = transform;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        filter=new ContactFilter2D().NoFilter();
        aired = false;
        trail = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
        bool grounded = col.IsTouching(filter);
        if (Input.anyKey || Input.touchCount>0)
        {
            trail.SetActive(true);
            if (grounded)
            {
                rb.AddTorque(forwardForce * forwardMultiplier * -1, ForceMode2D.Force);
            }
            else
            {
                rb.AddForce(downMultiplier * downwardForce*Vector2.down, ForceMode2D.Force);
            }
        }
        else
        {
            trail.SetActive(false);
        }

        if (grounded)
        {
            aired = true;
            previousVelocity = rb.velocity.magnitude;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (aired)
        {
            aired = false;
            float impact = rb.velocity.magnitude - previousVelocity;
            if (impact<-1 * loseImpact)
            {
                LoseManager.lose = true;
                rb.angularVelocity = 0;
                rb.velocity=Vector2.zero;
                rb.gravityScale = 0;
                gameObject.GetComponent<PlayerMovement>().enabled=false;
            }
            else if(impact<-1 * slowImpact)
            {
                rb.angularVelocity /= slowMultiplier * impact;
            }
        }
    }
}
