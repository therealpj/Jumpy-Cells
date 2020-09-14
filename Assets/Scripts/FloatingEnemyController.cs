using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingEnemyController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            PlayerPlatformerController pl = other.gameObject.GetComponent(typeof(PlayerPlatformerController)) as PlayerPlatformerController;
            pl.GetHit(1);
            Vector3 knockbackDirection = (pl.GetPosition() - transform.position).normalized;
            pl.DamageKnockback(knockbackDirection,  0.5f);

        }
    }
}
