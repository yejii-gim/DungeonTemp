using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public ItemSlot[] slots;
    public Transform dropPosition;
    public event Action OnInventoryUpdated; // UI업데이트를 위한 이벤트

    // 슬롯 초기화
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

    // 아이템 추가
    public void AddItem()
    {
        ItemData data = CharcterManager.Instance.player.itemData;

        if (data.canStack) // 스택 가능한지 확인
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

        // 비어 있는 슬롯 있으면 새로 추가
        var emptySlot = GetEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharcterManager.Instance.player.itemData = null;
            return;
        }

        // 슬롯이 없으면 땅에 드롭
        ThrowItem(data);
        CharcterManager.Instance.player.itemData = null;
    }

    // 인벤토리 UI 업데이트
    public void UpdateUI()
    {
        foreach (var slot in slots)
        {
            if (slot.item != null) slot.Set();
            else slot.Clear();
        }

        // 구독된 UI에 알림가도록
        OnInventoryUpdated?.Invoke();
    }

    public ItemSlot GetItemStack(ItemData data) => slots.FirstOrDefault(s => s.item == data && s.quantity < data.maxStackAmount);
    public ItemSlot GetEmptySlot() => slots.FirstOrDefault(s => s.item == null);

    public void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * UnityEngine.Random.value * 360));
    }
}
