using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICondition : MonoBehaviour
{
    public Condition health;
    public Condition hunger;
    public Condition stamina;



    private void Start()
    {
        CharcterManager.Instance.player.condition.uiCondition = this;
    }
}
