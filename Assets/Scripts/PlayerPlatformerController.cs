using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerPlatformerController : PhysicsObject
{
    public float jumpTakeOffSpeed = 7f;
    public float maxSpeed = 7f;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D collider;

    private Animator animator;
    public bool isInvincible = false;
    public float invincibilityDuration = 1.5f;
    public float invincibilityDeltaTime = 0.15f;
    public Score score;
    public Score scoreScript;
    private AudioSource source;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
        scoreScript = score.GetComponent<Score>();
        source = GetComponent<AudioSource>();
    }

    protected override void ComputeVelocity() {
        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");

        if(Input.GetButtonDown("Jump") && grounded) {
            source.Play();
            velocity.y = jumpTakeOffSpeed;

        } else if (Input.GetButtonUp("Jump")) {
            if (velocity.y > 0) {
                velocity.y = velocity.y * 0.5f;
            }
        }


        bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < -0.01f));
        if (flipSprite) {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
        targetVelocity = move * maxSpeed;

    }

    public void GetHit(int damageAmount) {
        if(isInvincible) {
            return;
        }

        HeartsHealthVisual.heartHealthSystemStatic.Damage(damageAmount);
        isInvincible = true;
        StartCoroutine(BecomeInvincible());
    }

    public void DamageKnockback(Vector3 direction, float distance) {
        transform.position += direction * distance;
    }

    public Vector3 GetPosition(){
        return transform.position;
    }

    public void UpdateScore(int amount) {
        scoreScript.UpdateScore(amount);
    }

    IEnumerator BecomeInvincible() {

        for(int i = 0; i < 10; i++) {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.05f);
        }

        isInvincible = false;
    }

}
