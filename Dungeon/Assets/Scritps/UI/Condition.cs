using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue;
    public float startValue;
    public float maxValue; // 레벨에 따라 달라질때 이거 조절
    public float passiveValue;
    [SerializeField] private Image _uiBar;
    [SerializeField] private TextMeshProUGUI _curText;
    [SerializeField] private TextMeshProUGUI _maxText;
    private void Start()
    {
        curValue = startValue;
        UIText();
    }

    private void Update()
    {
        // ui 업데이트
        _uiBar.fillAmount = GetPercentage();
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
        if (_uiBar != null)
            _uiBar.fillAmount = GetPercentage();

        if (_curText != null && _maxText != null)
        {
            _curText.text = $"{Mathf.CeilToInt(curValue)}";
            _maxText.text = $" / {Mathf.CeilToInt(maxValue)}";
        } 
    }
}
