
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GasFillEffect : MonoBehaviour
{
    [Header("Gas Overlay")]
    public Image gasImage;

    [Header("Gas Settings")]
    [Range(0f, 1f)]
    public float gasPerFart = 0.25f;

    [Range(0f, 1f)]
    public float maximumGas = 0.9f;

    public float fadeSpeed = 1.5f;

    [Header("Timing")]
    public float visibleDuration = 2f;

    private float currentGas = 0f;
    private float targetGas = 0f;

    private Coroutine gasRoutine;
    private Coroutine hideRoutine;

    void Start()
    {
        if (gasImage != null)
        {
            Color c = gasImage.color;
            c.a = 0f;
            gasImage.color = c;
        }
    }

    public void AddGas()
    {
        targetGas = Mathf.Clamp(
            targetGas + gasPerFart,
            0f,
            maximumGas
        );

        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }

        if (gasRoutine == null)
            gasRoutine = StartCoroutine(UpdateGas());

        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(visibleDuration);

        targetGas = 0f;

        hideRoutine = null;

        if (gasRoutine == null)
            gasRoutine = StartCoroutine(UpdateGas());
    }

    public void ClearGas()
    {
        targetGas = 0f;

        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }

        if (gasRoutine == null)
            gasRoutine = StartCoroutine(UpdateGas());
    }

    IEnumerator UpdateGas()
    {
        while (Mathf.Abs(currentGas - targetGas) > 0.001f)
        {
            currentGas = Mathf.MoveTowards(
                currentGas,
                targetGas,
                fadeSpeed * Time.deltaTime
            );

            if (gasImage != null)
            {
                Color c = gasImage.color;
                c.a = currentGas;
                gasImage.color = c;
            }

            yield return null;
        }

        currentGas = targetGas;

        if (gasImage != null)
        {
            Color c = gasImage.color;
            c.a = currentGas;
            gasImage.color = c;
        }

        gasRoutine = null;
    }
}
