
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyController : MonoBehaviour
{

    public float speed;
    public float jumpForce;
    public float gravityModifier = 0.5f;
    private float moveInput;
    private Vector2 velocity;
    public float chaseDistance;

    private Rigidbody2D rb;
    private Rigidbody2D playerRb;

    public bool isGrounded;
    public Transform groundCheck1, groundCheck2, groundCheck3;
    public float checkRadius;
    public LayerMask whatIsGround;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        InvokeRepeating("FlapWings", 1f,Random.Range(0.4f, 0.7f));
    }

    // Update is called once per frame
    void Update() {
    }

    void FlapWings() {
        if (!animator.GetBool("isDead")) {
            if (rb.position.y - playerRb.position.y < 5) {
                velocity.y = Mathf.Lerp(rb.velocity.y, rb.velocity.y + jumpForce, 0.5f);
            }
        }
    }


    void FixedUpdate() {
        Vector2 targetPos = playerRb.position;

        if (Mathf.Abs(targetPos.y - rb.position.y) >  chaseDistance || Mathf.Abs(targetPos.x - rb.position.x) > chaseDistance) {

        } else {
            rb.position = new Vector2(Mathf.Lerp(rb.position.x, targetPos.x, Time.deltaTime * speed), rb.position.y);
        }
        velocity.y += Physics2D.gravity.y * gravityModifier * Time.deltaTime;
        rb.velocity = new Vector2(rb.velocity.x, velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // checking to see if player landed on enemy's head
        if (other.gameObject.tag == "Player") {
            PlayerPlatformerController pl = other.gameObject.GetComponent(typeof(PlayerPlatformerController)) as PlayerPlatformerController;
            pl.velocity.y = 15f;
            pl.UpdateScore(50);

            gravityModifier = 2;
            animator.SetBool("isDead", true);
            rb.AddForce(transform.up * 600f);

            AudioSource source = GetComponent<AudioSource>();
            source.Play();
            gameObject.GetComponent<BoxCollider2D>().enabled =false;
            gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
            Destroy(gameObject, 3.0f);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        // player loses health
        if(other.gameObject.tag == "Player") {

            PlayerPlatformerController pl = other.gameObject.GetComponent(typeof(PlayerPlatformerController)) as PlayerPlatformerController;

            if (other.gameObject.transform.position.y > gameObject.transform.position.y) {
                return;
            }

            pl.GetHit(1);
        }
    }
}
