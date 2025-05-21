using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    public GameObject inventoryWindow;
    public Transform slotPanel;

    private InventorySelected itemSelector;

    private void Start()
    {
        itemSelector = GetComponent<InventorySelected>();

        inventoryWindow.SetActive(false);
        InventoryManager.Instance.InitializeSlots(slotPanel);
        itemSelector.slots = InventoryManager.Instance.slots;
        // updateUI가 실행될때마다 ClearSelectedItemWindow 호출
        InventoryManager.Instance.OnInventoryUpdated += itemSelector.ClearSelectedItemWindow;

        var controller = CharcterManager.Instance.player.controller ?? CharcterManager.Instance.player.GetComponent<PlayerController>();
        controller.OnInventory += Toggle;


        CharcterManager.Instance.player.addItem += InventoryManager.Instance.AddItem;
    }

    public void Toggle()
    {
        inventoryWindow.SetActive(!inventoryWindow.activeInHierarchy);
    }
   
}
