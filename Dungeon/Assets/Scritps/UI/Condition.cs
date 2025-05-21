using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue;
    public float startValue;
    public float maxValue; // ������ ���� �޶����� �̰� ����
    public float passiveValue;
    public Image uiBar;
    public TextMeshProUGUI curText;
    public TextMeshProUGUI maxText;
    private void Start()
    {
        curValue = startValue;
        UIText();
    }

    private void Update()
    {
        // ui ������Ʈ
        uiBar.fillAmount = GetPercentage();
        UIText();
    }

    float GetPercentage()
    {
        return curValue / maxValue;
    }

    public void Add(float value)
    {
        curValue = Mathf.Min(curValue + value,maxValue);
    }

    public void Substact(float value)
    {
        curValue = Mathf.Max(curValue - value, 0);
    }

    private void UIText()
    {
        if (uiBar != null)
            uiBar.fillAmount = GetPercentage();

        if (curText != null && maxText != null)
        {
            curText.text = $"{Mathf.CeilToInt(curValue)}";
            maxText.text = $" / {Mathf.CeilToInt(maxValue)}";
        } 
    }
}
