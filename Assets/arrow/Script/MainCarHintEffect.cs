using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MainCarHintEffect : MonoBehaviour
{
    public GameObject arrow;
    public Light2D glowLight;

    public float delay = 3f;
    public float showTime = 1f;

    public float arrowMoveY = 0.15f;
    public float maxLightIntensity = 1.2f;

    private Vector3 arrowStartPos;

    private void Start()
    {
        if (arrow != null)
        {
            arrowStartPos = arrow.transform.localPosition;
            arrow.SetActive(false);
        }

        if (glowLight != null)
            glowLight.intensity = 0f;

        StartCoroutine(HintLoop());
    }

    private IEnumerator HintLoop()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(delay);
            yield return StartCoroutine(PlayHint());
        }
    }

    private IEnumerator PlayHint()
    {
        if (arrow != null)
            arrow.SetActive(true);

        float timer = 0f;

        while (timer < showTime)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / showTime;
            float pulse = Mathf.Sin(t * Mathf.PI);

            if (glowLight != null)
                glowLight.intensity = pulse * maxLightIntensity;

            if (arrow != null)
            {
                arrow.transform.localPosition =
                    arrowStartPos + Vector3.up * Mathf.Sin(t * Mathf.PI * 2f) * arrowMoveY;
            }

            yield return null;
        }

        if (glowLight != null)
            glowLight.intensity = 0f;

        if (arrow != null)
        {
            arrow.transform.localPosition = arrowStartPos;
            arrow.SetActive(false);
        }
    }
}