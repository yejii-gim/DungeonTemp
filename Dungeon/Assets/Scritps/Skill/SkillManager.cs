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
    public event Action OnSkillUpdated; // UI업데이트를 위한 이벤트

    // 슬롯 초기화
    public void InitializeSlots(Transform slotPanel)
    {
        slots = new SkillSlot[slotPanel.childCount];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<SkillSlot>();
        }
    }

  

    // 인벤토리 UI 업데이트
    public void UpdateUI()
    {
        foreach (var slot in slots)
        {
            if (slot.skill != null) slot.Set();
            else slot.Clear();
        }

        // 구독된 UI에 알림가도록
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
