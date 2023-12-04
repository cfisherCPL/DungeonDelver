using UnityEngine;

public class PickUp : MonoBehaviour
{
    public enum eType { none, key, health, grappler }
    
    [Header("Inscribed")]
    public eType itemType;

    private Collider2D colld;
    private const float colliderEnableDelay = 0.5f;                               // a
       
    void Awake()
    {
        colld = GetComponent<Collider2D>();                                       // b
        colld.enabled = false;
        Invoke(nameof(EnableCollider), colliderEnableDelay);                    // c
    }
     
    void EnableCollider()
    {                                                       // d
        colld.enabled = true;
    }
}
