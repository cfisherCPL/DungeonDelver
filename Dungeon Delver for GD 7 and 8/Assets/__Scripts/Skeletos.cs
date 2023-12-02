using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeletos : Enemy
{
    [Header("Inscribed: Skeletos")]                                              // b
    public int speed = 2;
    public float timeThinkMin = 1f;
    public float timeThinkMax = 4f;
      
    [Header("Dynamic: Skeletos")]                                                // b
    [Range(0, 4)]
    public int facing = 0;
    public float timeNextDecision = 0;

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= timeNextDecision)
        {                                     // c
            DecideDirection();
        }
        // rigid is inherited from Enemy, which defines it in Enemy.Awake()
        rigid.velocity = directions[facing] * speed;
    }

    void DecideDirection()
    {                                                     // d
        facing = Random.Range(0, 5);
        timeNextDecision = Time.time + Random.Range(timeThinkMin, timeThinkMax);
    }

}
