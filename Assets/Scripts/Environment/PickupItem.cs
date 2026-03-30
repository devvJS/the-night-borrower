using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [Header("Pickup")]
    [SerializeField] private ItemType itemType;
    [SerializeField] private int amount = 1;

    public ItemType Type => itemType;
    public int Amount => amount;

    public void Collect()
    {
        gameObject.SetActive(false);
    }
}
