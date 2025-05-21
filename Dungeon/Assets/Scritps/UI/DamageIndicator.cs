using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public Image image;
    public float flashSpeed;

    private Coroutine coroutine;
    private void Start()
    {
        CharcterManager.Instance.player.condition.onTakeDamage += Flash;
    }

    // �ǰݽ� ȣ��Ǿ� ȭ�鿡 ������ ���̰��ϴ� �Լ�
    public void Flash()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        image.enabled = true;
        image.color = new Color(1f, 100f / 255f, 100f / 255f);
        coroutine = StartCoroutine(FadeAway());
    }

    // ���İ� �����ؼ� �̹����� ��������̰Բ� �ϴ� �ڷ�ƾ
    private IEnumerator FadeAway()
    {
        float startAlpha = 0.3f;
        float a = startAlpha;

        while(a>0)
        {
            a -= (startAlpha / flashSpeed) * Time.deltaTime;
            image.color = new Color(1f, 100f/255f, 100f/255f,a);
            yield return null;
        }
        image.enabled = false;
    }
}
