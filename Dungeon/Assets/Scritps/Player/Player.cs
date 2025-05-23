using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
   public PlayerController controller;
   public PlayerCondition condition;
    public Equipment equipment;

    public ItemData itemData;
    public Action addItem;

    public Transform dropPosition;
    private void Awake()
    {
        CharcterManager.Instance.player = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
        equipment = GetComponent<Equipment>();
    }
}
