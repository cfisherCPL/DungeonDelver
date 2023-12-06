using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InRoom))]
public class Dray : MonoBehaviour, IFacingMover, IKeyMaster
{
    static private Dray S;

    static public IFacingMover IFM;
    public enum eMode { idle, move, attack, roomTrans, knockback, gadget }

    [Header("Inscribed")]
    public float speed = 5;
    public float attackDuration = 0.25f;// Number of seconds to attack
    public float attackDelay = 0.5f;    // Delay between attacks
    public float roomTransDelay = 0.5f; // Room transition delay   // b
    public int maxHealth = 10;
    public float knockbackSpeed = 10;                              // b
    public float knockbackDuration = 0.25f;
    public float invincibleDuration = 0.5f;
    public int healthPickupAmount = 2;
    public KeyCode keyAttack = KeyCode.Z;                           // b
    public KeyCode keyGadget = KeyCode.X;
    [SerializeField]                                 
    private bool startWithGrappler = true;


    [Header("Dynamic")]
    public int dirHeld = -1; // Direction of the held movement key
    public int facing = 1;   // Direction Dray is facing 
    public eMode mode = eMode.idle;                                 // a
    public bool invincible = false;
    [SerializeField] [Range(0, 20)]
    private int _numKeys = 0;

    [SerializeField] [Range(0, 10)]
    private int _health;
    public int health
    {                                                        // c
        get { return _health; }
        set { _health = value; }
    }

    private float timeAtkDone = 0;                                   // b
    private float timeAtkNext = 0;
    private float roomTransDone = 0;                                // b
    private Vector2 roomTransPos;
    private float knockbackDone = 0;                                // d
    private float invincibleDone = 0;
    private Vector2 knockbackVel;

    private Grappler grappler;
    private SpriteRenderer sRend;
    private Rigidbody2D rigid;
    private Animator anim;
    private InRoom inRm;

    private Vector2[] directions = new Vector2[] {
        Vector2.right, Vector2.up, Vector2.left, Vector2.down };

    private KeyCode[] keys = new KeyCode[] {
        KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow,
        KeyCode.D,          KeyCode.W,       KeyCode.A,         KeyCode.S };

    void Awake()
    {
        S = this;
        IFM = this;
        sRend = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        inRm = GetComponent<InRoom>();
        health = maxHealth;

        grappler = GetComponentInChildren<Grappler>();                           // b
        if (startWithGrappler) currentGadget = grappler;
    }

    // Update is called once per frame
    void Update()
    {
        // Check knockback and invincibility
        if (invincible && Time.time > invincibleDone) invincible = false;     // g
        sRend.color = invincible ? Color.red : Color.white;
        if (mode == eMode.knockback)
        {
            rigid.velocity = knockbackVel;
            if (Time.time < knockbackDone) return;
            // The following line is only reached if Time.time >= knockbackDone
            mode = eMode.idle;
        }

        if (mode == eMode.roomTrans)
        {                                      // c
            rigid.velocity = Vector3.zero;
            anim.speed = 0;
            posInRoom = roomTransPos;  // Keeps Dray in place
            if (Time.time < roomTransDone) return;
            // The following line is only reached if Time.time >= transitionDone
            mode = eMode.idle;
        }


        // Finishing the attack when it’s over
        if (mode == eMode.attack && Time.time >= timeAtkDone)
        {                // a
            mode = eMode.idle;
        }

        //———————————— Handle Keyboard Input in idle or move Modes ————————————
        if (mode == eMode.idle || mode == eMode.move)
        {
            dirHeld = -1;
            for (int i = 0; i < keys.Length; i++)
            {
                if (Input.GetKey(keys[i])) dirHeld = i % 4;
            }

            // Choosing the proper movement or idle mode based on dirHeld
            if (dirHeld == -1)
            {                                               // c
                mode = eMode.idle;
            }
            else
            {
                facing = dirHeld; // d
                mode = eMode.move;
            }

            // Pressing the gadget button
            if (Input.GetKeyDown(keyGadget))
            {                           // d
                if (currentGadget != null)
                {
                    if (currentGadget.GadgetUse(this, GadgetIsDone))
                    {   // e
                        mode = eMode.gadget;
                        rigid.velocity = Vector2.zero;
                    }
                }
            }


            // Pressing the attack button
            if (Input.GetKeyDown(keyAttack) && Time.time >= timeAtkNext)
            {     // e
                mode = eMode.attack;
                timeAtkDone = Time.time + attackDuration;
                timeAtkNext = Time.time + attackDelay;
            }
        }

        //———————————————————— Act on the current mode ————————————————————
        Vector2 vel = Vector2.zero;
        switch (mode)
        {
            case eMode.attack: // Show the Attack pose in the correct direction
                anim.Play("Dray_Attack_" + facing);
                anim.speed = 0;
                break;

            case eMode.idle:   // Show frame 1 in the correct direction
                anim.Play("Dray_Walk_" + facing);
                anim.speed = 0;
                break;

            case eMode.move:   // Play walking animation in the correct direction
                vel = directions[dirHeld];
                anim.Play("Dray_Walk_" + facing);
                anim.speed = 1;
                break;

            case eMode.gadget: // Show Attack pose & wait for IGadget to be done  // g
                anim.Play("Dray_Attack_" + facing);
                anim.speed = 0;
                break;

        }

        rigid.velocity = vel * speed;

    }

    void LateUpdate()
    {
        // Get the nearest quarter-grid position to Dray
        Vector2 gridPosIR = GetGridPosInRoom(0.25f);                        // d
        
        // Check to see whether we’re in a Door tile
        int doorNum;
        for (doorNum = 0; doorNum < 4; doorNum++)
        {                              // e
            if (gridPosIR == InRoom.DOORS[doorNum])
            {
                break;
            }
        }
    
        if (doorNum > 3 || doorNum != facing) return;                       // f
    
        // Move to the next room
        Vector2 rm = roomNum;
        switch (doorNum)
        {                                                    // g
            case 0:
                rm.x += 1;
                break;
            case 1:
                rm.y += 1;
                break;
            case 2:
                rm.x -= 1;
                break;
            case 3:
                rm.y -= 1;
                break;
        }
    
        // Make sure that the rm we want to jump to is valid
        if (0 <= rm.x && rm.x <= InRoom.MAX_RM_X)
        {                           // h
            if (0 <= rm.y && rm.y <= InRoom.MAX_RM_Y)
            {
                roomNum = rm;
                roomTransPos = InRoom.DOORS[(doorNum + 2) % 4];               // i
                posInRoom = roomTransPos;
                mode = eMode.roomTrans; // j
                roomTransDone = Time.time + roomTransDelay;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (invincible) return; // Return if Dray can’t be damaged            // h
        DamageEffect dEf = coll.gameObject.GetComponent<DamageEffect>();
        if (dEf == null) return; // If no DamageEffect, exit this method
    
        health -= dEf.damage;  // Subtract the damage amount from health      // i
        invincible = true;  // Make Dray invincible
        invincibleDone = Time.time + invincibleDuration;

        if (dEf.knockback)
        {  // Knockback Dray                               // j
            // Determine the direction of knockback from relative position
            Vector2 delta = transform.position - coll.transform.position;     // k
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
    
            // Apply knockback speed to the Rigidbody
            knockbackVel = delta * knockbackSpeed;
            rigid.velocity = knockbackVel;

            // If not in gadget mode OR if GadgetCancel is successful
            if (mode != eMode.gadget || currentGadget.GadgetCancel())
            {     // i
                // Set mode to knockback and set time to stop knockback
                mode = eMode.knockback;
                knockbackDone = Time.time + knockbackDuration;
            }
        }


    }

    void OnTriggerEnter2D(Collider2D colld)
    {
        PickUp pup = colld.GetComponent<PickUp>();                            // b
        if (pup == null) return;

        switch (pup.itemType)
        {
            case PickUp.eType.health:
                health = Mathf.Min(health + healthPickupAmount, maxHealth);
                break;
            case PickUp.eType.key:
                _numKeys++;
                break;
            default:
                Debug.LogError("No case for PickUp type " + pup.itemType);      // c
                break;
        }

        Destroy(pup.gameObject);

    }




    static public int HEALTH { get { return S._health; } }                  // f
    static public int NUM_KEYS { get { return S._numKeys; } }


    //———————————————————— Implementation of IFacingMover ————————————————————
    public int GetFacing() { return facing; }                                // e

    public float GetSpeed() { return speed; }                                 // f
      
    public bool moving { get { return (mode == eMode.move); } }            // g
     
    public float gridMult { get { return inRm.gridMult; } }                   // h
      
    public bool isInRoom { get { return inRm.isInRoom; } }
    
    public Vector2 roomNum
    {               
        get { return inRm.roomNum; }
        set { inRm.roomNum = value; }
    }
    
    public Vector2 posInRoom
    {             
        get { return inRm.posInRoom; }
        set { inRm.posInRoom = value; }
    }
    
    public Vector2 GetGridPosInRoom(float mult = -1)
    {                      // i
        return inRm.GetGridPosInRoom(mult);
    }


    //———————————————————— Implementation of IKeyMaster ————————————————————
    public int keyCount
    {                                                         // d
        get { return _numKeys; }
        set { _numKeys = value; }
    }

    public Vector2 pos
    {                                                          // e
        get { return (Vector2)transform.position; }
    }

#region IGadget_Affordances
    //———————————————————— IGadget Affordances  ————————————————————
    public IGadget currentGadget { get; private set; }

    /// <summary>
    /// Called by an IGadget when it is done. Sets mode to eMode.idle.
    /// Matches the System.Func<IGadget, bool> delegate type required by the 
    ///  tDoneCallback parameter of IGadget.GadgetUse().
    /// </summary>
    /// <param name="gadget">The IGadget calling this method</param>
    /// <returns>true if successful, false if not</returns>
    public bool GadgetIsDone(IGadget gadget)
    {
        if (gadget != currentGadget)
        {
            Debug.LogError("A non-current Gadget called GadgetDone"
                +"\ncurrentGadget: " + currentGadget.name
                +"\tcalled by: " + gadget.name);
        }
    
        mode = eMode.idle;
        return true;
    }
#endregion



}
