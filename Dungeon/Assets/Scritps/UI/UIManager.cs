using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private GameObject currentOpen;
    [SerializeField] private TextMeshProUGUI promptText;
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
    public void OpenPrompt(string message)
    {
        promptText.gameObject.SetActive(true);
        promptText.text = message;
    }

    public void ClosePrompt()
    {
        promptText.gameObject.SetActive(false);
        promptText.text = string.Empty;
    }
}
