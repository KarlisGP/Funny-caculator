
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FartButtonEffect : MonoBehaviour
{
    [Header("Button References")]
    public Button button;
    public Image background;
    public Image glow;

    [Header("Button Colors")]
    public Color normalColor = new Color(0.05f, 0.35f, 0.05f);
    public Color pressedColor = new Color(0.3f, 1f, 0.05f);

    [Header("Glow Settings")]
    public Color glowColor = new Color(0.2f, 1f, 0f);
    [Range(0f, 1f)]
    public float glowIntensity = 0.9f;

    [Header("Animation Settings")]
    public float pressScale = 0.9f;
    public float effectDuration = 0.2f;

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip buttonSound;

    [Header("Astronaut")]
    public AstronautManager astronautManager;
    [Header("Gas Effect")]
    public GasFillEffect gasFillEffect;

    private Vector3 originalScale;
    private Coroutine effectRoutine;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    void OnEnable()
    {
        if (button != null)
            button.onClick.AddListener(PressButton);
    }

    void Start()
    {
        if (background != null)
            background.color = normalColor;

        if (glow != null)
        {
            glow.color = new Color(
                glowColor.r,
                glowColor.g,
                glowColor.b,
                0f
            );
        }
    }

    void OnDisable()
    {
        if (button != null)
            button.onClick.RemoveListener(PressButton);

        if (effectRoutine != null)
        {
            StopCoroutine(effectRoutine);
            effectRoutine = null;
        }

        transform.localScale = originalScale;
    }

    void PressButton()
    {
        // Play button sound once
        if (audioSource != null && buttonSound != null)
            audioSource.PlayOneShot(buttonSound);

        // Play visual effect
        if (effectRoutine != null)
            StopCoroutine(effectRoutine);

        effectRoutine = StartCoroutine(ButtonEffect());

        if (astronautManager != null)
            astronautManager.PlayFart();

        if (gasFillEffect != null)
            gasFillEffect.AddGas();
    }

    IEnumerator ButtonEffect()
    {
        float timer = 0f;

        // PRESS
        while (timer < effectDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / effectDuration);

            transform.localScale = Vector3.Lerp(
                originalScale,
                originalScale * pressScale,
                t
            );

            if (background != null)
                background.color = Color.Lerp(
                    normalColor, pressedColor, t
                );

            if (glow != null)
                glow.color = new Color(
                    glowColor.r,
                    glowColor.g,
                    glowColor.b,
                    Mathf.Lerp(0f, glowIntensity, t)
                );

            yield return null;
        }

        // RELEASE
        timer = 0f;

        while (timer < effectDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / effectDuration);

            transform.localScale = Vector3.Lerp(
                originalScale * pressScale,
                originalScale,
                t
            );

            if (background != null)
                background.color = Color.Lerp(
                    pressedColor, normalColor, t
                );

            if (glow != null)
                glow.color = new Color(
                    glowColor.r,
                    glowColor.g,
                    glowColor.b,
                    Mathf.Lerp(glowIntensity, 0f, t)
                );

            yield return null;
        }

        // RESET
        transform.localScale = originalScale;

        if (background != null)
            background.color = normalColor;

        if (glow != null)
            glow.color = new Color(
                glowColor.r,
                glowColor.g,
                glowColor.b,
                0f
            );

        effectRoutine = null;
    }
}
