using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float time;
    public float fullDayLength;
    public float startTime = 0.4f;
    private float timeRate;
    public Vector3 noon; // Vector 90 0 0

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntenstiy;

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntenstiy;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMulitplier;
    public AnimationCurve reflectionIntensityMultitplier;

    private void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    private void Update()
    {
        time = (time + timeRate * Time.deltaTime) % 1.0f;

        UpdateLighting(sun, sunColor, sunIntenstiy);
        UpdateLighting(moon, moonColor, moonIntenstiy);

        RenderSettings.ambientIntensity = lightingIntensityMulitplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultitplier.Evaluate(time);
    }

    void UpdateLighting(Light lightSource, Gradient gradient, AnimationCurve intenstiryCurve)
    {
        float intensity = intenstiryCurve.Evaluate(time);

        // 정오시간 계산
        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.5f : 0.75f)) * noon;
        lightSource.color = gradient.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if(lightSource.intensity == 0 && go.activeInHierarchy)
        {
            go.SetActive(false);
        }
        else if(lightSource.intensity > 0 && !go.activeInHierarchy)
        {
            go.SetActive(true);
        }
    }
}
