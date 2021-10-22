using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FB_EnergyScreen : MonoBehaviour
{
    [Range(0,1)] public float vignetteGoal = 0.5f;
    public float duration = 0.5f;

    public Volume postPro;

    public void EnergyCall(bool hasEnergy)
    {
        StopAllCoroutines();
        StartCoroutine(hasEnergy ? GetEnergy() : LostEnergy());
    }

    IEnumerator GetEnergy()
    {
        float time = 0;
        float startValue = 0;

        if (postPro.profile.TryGet<Vignette>(out var vignette))
        {
            while (time < duration)
            {
                vignette.intensity.value = Mathf.Lerp(startValue, vignetteGoal, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
        }
    }

    IEnumerator LostEnergy()
    {
        float time = 0;
        float startValue = 0;

        if (postPro.profile.TryGet<Vignette>(out var vignette))
        {
            while (time < duration)
            {
                vignette.intensity.value = Mathf.Lerp(vignetteGoal, startValue, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}
