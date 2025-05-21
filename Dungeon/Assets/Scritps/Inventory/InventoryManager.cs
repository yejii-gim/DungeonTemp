using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public ItemSlot[] slots;
    public Transform dropPosition;
    public event Action OnInventoryUpdated; // UI������Ʈ�� ���� �̺�Ʈ

    // ���� �ʱ�ȭ
    public void InitializeSlots(Transform slotPanel)
    {
        slots = new ItemSlot[slotPanel.childCount];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
        }

        dropPosition = CharcterManager.Instance.player.dropPosition;
    }

    // ������ �߰�
    public void AddItem()
    {
        ItemData data = CharcterManager.Instance.player.itemData;

        if (data.canStack) // ���� �������� Ȯ��
        {
            var stackSlot = GetItemStack(data);
            if (stackSlot != null)
            {
                stackSlot.quantity++;
                UpdateUI();
                CharcterManager.Instance.player.itemData = null;
                return;
            }
        }

        // ��� �ִ� ���� ������ ���� �߰�
        var emptySlot = GetEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharcterManager.Instance.player.itemData = null;
            return;
        }

        // ������ ������ ���� ���
        ThrowItem(data);
        CharcterManager.Instance.player.itemData = null;
    }

    // �κ��丮 UI ������Ʈ
    public void UpdateUI()
    {
        foreach (var slot in slots)
        {
            if (slot.item != null) slot.Set();
            else slot.Clear();
        }

        // ������ UI�� �˸�������
        OnInventoryUpdated?.Invoke();
    }

    public ItemSlot GetItemStack(ItemData data) => slots.FirstOrDefault(s => s.item == data && s.quantity < data.maxStackAmount);
    public ItemSlot GetEmptySlot() => slots.FirstOrDefault(s => s.item == null);

    public void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * UnityEngine.Random.value * 360));
    }
}
