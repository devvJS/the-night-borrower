using UnityEngine;
using System;
using System.Collections.Generic;

public struct InventorySlot
{
    public ItemType itemType;
    public int count;
    public bool IsEmpty => count <= 0;
}

public class PlayerInventory : MonoBehaviour
{
    [Header("Starting Items")]
    [SerializeField] private int startingBulbs = GameConstants.StartingBulbCount;

    private InventorySlot[] slots;

    public IReadOnlyList<InventorySlot> Slots => slots;
    public event Action OnInventoryChanged;

    private void Awake()
    {
        slots = new InventorySlot[GameConstants.InventorySlotCount];

        if (startingBulbs > 0)
        {
            AddItem(ItemType.SpareBulb, startingBulbs);
        }
    }

    public bool TryAddItem(ItemType type, int amount = 1)
    {
        // First try to stack into an existing slot of the same type
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsEmpty && slots[i].itemType == type
                && slots[i].count + amount <= GameConstants.MaxStackSize)
            {
                slots[i].count += amount;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // Then try an empty slot
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty)
            {
                slots[i].itemType = type;
                slots[i].count = amount;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        return false;
    }

    public bool TryUseItem(ItemType type, int amount = 1)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsEmpty && slots[i].itemType == type && slots[i].count >= amount)
            {
                slots[i].count -= amount;
                if (slots[i].count <= 0)
                {
                    slots[i].count = 0;
                }
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        return false;
    }

    public int GetItemCount(ItemType type)
    {
        int total = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsEmpty && slots[i].itemType == type)
            {
                total += slots[i].count;
            }
        }
        return total;
    }

    private void AddItem(ItemType type, int amount)
    {
        while (amount > 0)
        {
            int toAdd = Mathf.Min(amount, GameConstants.MaxStackSize);
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsEmpty && slots[i].itemType == type
                    && slots[i].count + toAdd <= GameConstants.MaxStackSize)
                {
                    slots[i].count += toAdd;
                    amount -= toAdd;
                    break;
                }

                if (slots[i].IsEmpty)
                {
                    slots[i].itemType = type;
                    slots[i].count = toAdd;
                    amount -= toAdd;
                    break;
                }
            }

            // Safety: if no slot available, break to avoid infinite loop
            if (amount > 0 && amount == toAdd)
                break;
        }
    }
}
