using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected static Vector2[] directions = new Vector2[] {                  // a
        Vector2.right, Vector2.up, Vector2.left, Vector2.down, Vector2.zero };

    [Header("Inscribed: Enemy")]                                              // b
    public float maxHealth = 1;                                // c
    
    [Header("Dynamic: Enemy")]                                                // b
    public float health;                                       // c
    
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


}
