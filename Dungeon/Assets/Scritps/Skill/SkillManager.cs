using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SkillType
{
    Invincible,
    DoubleJump,
    Dash,
}
public class SkillManager : Singleton<SkillManager>
{
    public SkillSlot[] slots;
    public event Action OnSkillUpdated; // UI������Ʈ�� ���� �̺�Ʈ

    // ���� �ʱ�ȭ
    public void InitializeSlots(Transform slotPanel)
    {
        slots = new SkillSlot[slotPanel.childCount];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<SkillSlot>();
        }
    }

  

    // �κ��丮 UI ������Ʈ
    public void UpdateUI()
    {
        foreach (var slot in slots)
        {
            if (slot.skill != null) slot.Set();
            else slot.Clear();
        }

        // ������ UI�� �˸�������
        OnSkillUpdated?.Invoke();
    }

    public void TriggerCooldown(SkillType type)
    {
        foreach (var slot in slots)
        {
            if (slot.skill != null && slot.skill.type == type)
            {
                slot.StartCooldown();
                break;
            }
        }
    }

    public void UnLockSkill(SkillType type)
    {
        foreach (var slot in slots)
        {
            if (slot.skill != null && slot.skill.type == type)
            {
                slot.UnLock();
                slot.Set();
                break;
            }
        }
    }
}
