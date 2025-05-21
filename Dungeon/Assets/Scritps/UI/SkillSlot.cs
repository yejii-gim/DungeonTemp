using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    public SkillData skill;

    public Image icon;
    public Image cooldownImage; // 쿨타임 표시용
    public GameObject LockImage;

    public bool isOpen;

    private Coroutine cooldownRoutine;

    private void Awake()
    {
        cooldownImage.fillAmount = 1f;
    }
    public void Set()
    {
        icon.gameObject.SetActive(true);
        icon.sprite = skill.icon;
    }

    public void StartCooldown()
    {
        if (cooldownRoutine != null || !isOpen) return;
        cooldownRoutine = StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator CooldownCoroutine()
    {
        cooldownImage.fillAmount = 1f;

        float elapsed = 0f;

        // 쿨타임 진행
        while (elapsed < skill.coolTime)
        {
            elapsed += Time.deltaTime;
            cooldownImage.fillAmount = Mathf.Clamp01(1f - (elapsed / skill.coolTime));
            yield return null;
        }

        // 쿨타임 종료
        cooldownImage.fillAmount = 0f;
        cooldownRoutine = null;
    }

    // 초기화
    public void Clear()
    {
        skill = null;
        icon.gameObject.SetActive(false);
        cooldownImage.fillAmount = 0f;

        if (cooldownRoutine != null)
        {
            StopCoroutine(cooldownRoutine);
            cooldownRoutine = null;
        }
    }

    public void UnLock()
    {
        Debug.Log("열렸다");
        isOpen = true;
        LockImage.SetActive(false);
        cooldownImage.fillAmount = 0f;
    }
}
