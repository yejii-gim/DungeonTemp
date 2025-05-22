using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventorySelected : MonoBehaviour
{ 
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedStatName;
    public TextMeshProUGUI selectedStatValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject dropButton;

    private ItemData selectedItem; // ���� ���õ� ������
    private int selectedItemIndex;

    private PlayerCondition condition; // �÷��̾� ����
    public ItemSlot[] slots; // ��ü ���� �迭
    int curEquipIndex; // ���� ���� ���� ���� �ε���
    private void Start()
    {
        slots = InventoryManager.Instance.slots;
        condition = CharcterManager.Instance.GetComponent<PlayerCondition>() ?? CharcterManager.Instance.player.GetComponent<PlayerCondition>();
    }

    // ������ ���ý� ������ UI�� ǥ���ϰ� ���ִ� �Լ�
    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        selectedItem = slots[index].item;
        selectedItemIndex = index;

        // �ؽ�Ʈ ���� ������Ʈ
        selectedItemName.text = selectedItem.displayName;
        selectedItemDescription.text = selectedItem.description;

        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        // �Һ� ������ ȿ�� ��� ǥ��
        for (int i = 0; i < selectedItem.consumables.Length; i++)
        {
            selectedStatName.text += selectedItem.consumables[i].type.ToString() + "\n";
            selectedStatValue.text += selectedItem.consumables[i].value.ToString() + "\n";
        }

        // ��ư ���� ����
        useButton.SetActive(selectedItem.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.type == ItemType.Equipable && !slots[index].equipped);
        unequipButton.SetActive(selectedItem.type == ItemType.Equipable && slots[index].equipped);
        dropButton.SetActive(true);
    }

    // UI �ʱ�ȭ
    public void ClearSelectedItemWindow()
    {
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;
        useButton.SetActive(false);
        equipButton.SetActive(false);
        unequipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public void OnUseButton()
    {
        if (selectedItem.type != ItemType.Consumable || selectedItem.type != ItemType.Box) return;

        foreach (var effect in selectedItem.consumables)
        {
            switch (effect.type)
            {
                case ConsumableType.Health: condition.Heal(effect.value); break;
                case ConsumableType.Hunger: condition.Eat(effect.value); break;
                case ConsumableType.Invincible:
                    SkillManager.Instance.UnLockSkill(SkillType.Invincible);
                    break;
                case ConsumableType.Dash:
                    SkillManager.Instance.UnLockSkill(SkillType.Dash);
                    break;
                case ConsumableType.DoubleJump:
                    SkillManager.Instance.UnLockSkill(SkillType.DoubleJump);
                    break;
            }
        }
        RemoveSelectedItem();
    }

    public void OnDropButton()
    {
        InventoryManager.Instance.ThrowItem(selectedItem);
        RemoveSelectedItem();
    }

    private void RemoveSelectedItem()
    {
        var slot = InventoryManager.Instance.slots[selectedItemIndex];
        slot.quantity--;
        if (slot.quantity <= 0)
        {
            slot.item = null;
            selectedItem = null;
            selectedItemIndex = -1;
            ClearSelectedItemWindow();
        }

        InventoryManager.Instance.UpdateUI();
    }

    public void OnEquipButton()
    {
        if (slots[curEquipIndex].equipped)
        {
            UnEquip(curEquipIndex);
        }
        slots[selectedItemIndex].equipped = true;
        curEquipIndex = selectedItemIndex;
        CharcterManager.Instance.player.equipment.EquipNew(selectedItem);
        InventoryManager.Instance.UpdateUI();

        SelectItem(selectedItemIndex);
    }

    public void OnUnEquipButton(int index)
    {
        UnEquip(index);
    }

    void UnEquip(int index)
    {
        slots[index].equipped = false;
        CharcterManager.Instance.player.equipment.UnEquip();
        InventoryManager.Instance.UpdateUI();

        if (selectedItemIndex == index)
        {
            SelectItem(selectedItemIndex);
        }
    }

}
