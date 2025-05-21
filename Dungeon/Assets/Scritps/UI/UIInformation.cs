using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInformation : MonoBehaviour
{
    public GameObject informationWindow;
    private void Start()
    {
        var controller = CharcterManager.Instance.player.controller ?? CharcterManager.Instance.player.GetComponent<PlayerController>();
        controller.OnInformation += Toggle;
        informationWindow.SetActive(false);
    }

    public void Toggle()
    {
        UIManager.Instance.Toggle(informationWindow);
    }
}
