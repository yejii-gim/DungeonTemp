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

    // 피격시 호출되어 화면에 빨간색 보이게하는 함수
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

    // 알파값 조절해서 이미지가 사라져보이게끔 하는 코루틴
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
