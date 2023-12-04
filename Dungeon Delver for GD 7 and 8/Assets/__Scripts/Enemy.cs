using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected static Vector2[] directions = new Vector2[] {                  // a
        Vector2.right, Vector2.up, Vector2.left, Vector2.down, Vector2.zero };

    [Header("Inscribed: Enemy")]                                              // b
    public float maxHealth = 1;                                // c
    public float knockbackSpeed = 10;                         // a
    public float knockbackDuration = 0.25f;
    public float invincibleDuration = 0.5f;

    [Header("Dynamic: Enemy")]                                                // b
    public float health;                                       // c
    public bool invincible = false;                          // a
    public bool knockback = false;
    
    private float invincibleDone = 0;                          // a
    private float knockbackDone = 0;
    private Vector2 knockbackVel;

    protected Animator anim;                                         // c
    protected Rigidbody2D rigid;                                        // c
    protected SpriteRenderer sRend;

    protected virtual void Awake()
    {                                          // d
        health = maxHealth;
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sRend = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {                                        // b
        // Check knockback and invincibility
        if (invincible && Time.time > invincibleDone) invincible = false;
        sRend.color = invincible ? Color.red : Color.white;
        if (knockback)
        {
            rigid.velocity = knockbackVel;
            if (Time.time < knockbackDone) return;
        }
    
        anim.speed = 1;                                                      // c
        knockback = false;
    }

    void OnTriggerEnter2D(Collider2D colld)
    {
        if (invincible) return; // Return if this can’t be damaged 
        DamageEffect dEf = colld.gameObject.GetComponent<DamageEffect>();
        if (dEf == null) return; // If no DamageEffect, exit this method

        health -= dEf.damage; // Subtract the damage amount from health
        if (health <= 0) Die();

        invincible = true; // Make this invincible
        invincibleDone = Time.time + invincibleDuration;

        if (dEf.knockback)
        { // Knockback this Enemy
            Vector2 delta;
            // Is an IFacingMover attached to the Collider that triggered this?
            IFacingMover iFM = colld.GetComponentInParent<IFacingMover>();   // f
            if (iFM != null)
            {
                // Determine the direction of knockback from the iFM’s facing
                delta = directions[iFM.GetFacing()];
            }
            else
            {
                // Determine the direction of knockback from relative position
                delta = transform.position - colld.transform.position;
                if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
                {
                    // Knockback should be horizontal
                    delta.x = (delta.x > 0) ? 1 : -1;
                    delta.y = 0;
                }
                else
                {
                    // Knockback should be vertical
                    delta.x = 0;
                    delta.y = (delta.y > 0) ? 1 : -1;
                }
            }
    
            // Apply knockback speed to the Rigidbody
            knockbackVel = delta * knockbackSpeed;
            rigid.velocity = knockbackVel;
        
            // Set mode to knockback and set time to stop knockback
            knockback = true;
            knockbackDone = Time.time + knockbackDuration;
            anim.speed = 0;
        }

    }

    void Die()
    {                                                             // g
        Destroy(gameObject);
    }

}
