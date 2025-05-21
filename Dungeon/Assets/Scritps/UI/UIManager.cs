using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private GameObject currentOpen;

    // 다른창이 열려있었으면 닫고 이 창 열도록 하는 함수
    public void Toggle(GameObject window)
    {
        if (currentOpen == window)
        {
            window.SetActive(false);
            currentOpen = null;
        }
        else
        {
            if (currentOpen != null)
                currentOpen.SetActive(false);

            window.SetActive(true);
            currentOpen = window;
        }
    }

}
