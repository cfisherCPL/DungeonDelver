using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dray : MonoBehaviour
{
    [Header("Inscribed")]
    public float speed = 5;

    [Header("Dynamic")]
    public int dirHeld = -1; // Direction of the held movement key

    private Rigidbody2D rigid;
    private Animator anim;

    private Vector2[] directions = new Vector2[] {
        Vector2.right, Vector2.up, Vector2.left, Vector2.down };

    private KeyCode[] keys = new KeyCode[] {
        KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow,
        KeyCode.D,          KeyCode.W,       KeyCode.A,         KeyCode.S };

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        dirHeld = -1;
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKey(keys[i])) dirHeld = i % 4;                     // b
        }

        Vector2 vel = Vector2.zero;
        if (dirHeld > -1) vel = directions[dirHeld];

        rigid.velocity = vel * speed;

        // Animation
        if (dirHeld == -1)
        {                                                // c
            anim.speed = 0;
        }
        else
        {
            anim.Play("Dray_Walk_" + dirHeld);                              // d
            anim.speed = 1;
        }
    }



}
