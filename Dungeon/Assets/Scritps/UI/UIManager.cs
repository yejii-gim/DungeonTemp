using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private GameObject currentOpen;

    // �ٸ�â�� �����־����� �ݰ� �� â ������ �ϴ� �Լ�
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
