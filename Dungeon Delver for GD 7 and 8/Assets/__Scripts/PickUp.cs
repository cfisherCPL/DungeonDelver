using UnityEngine;

public class PickUp : MonoBehaviour
{
    public enum eType { none, key, health, grappler }
    
    [Header("Inscribed")]
    public eType itemType;
}
