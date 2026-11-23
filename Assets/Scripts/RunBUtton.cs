
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RunButtonEffect : MonoBehaviour
{
    [Header("Button References")]
    public Button button;
    public Image background;
    public Image glow;

    [Header("Button Colors")]
    public Color normalColor = new Color(0.05f, 0.25f, 0.65f);
    public Color pressedColor = new Color(0.1f, 0.85f, 1f);

    [Header("Glow Settings")]
    public Color glowColor = new Color(0f, 0.8f, 1f);
    [Range(0f, 1f)]
    public float glowIntensity = 0.8f;

    [Header("Animation Settings")]
    public float pressScale = 0.9f;
    public float effectDuration = 0.2f;

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip buttonSound;

    [Header("Astronaut")]
    public AstronautManager astronautManager;

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
        // Play button sound
        if (audioSource != null && buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }

        // Play button visual effect
        if (effectRoutine != null)
        {
            StopCoroutine(effectRoutine);
        }

        effectRoutine = StartCoroutine(ButtonEffect());

        // Start astronaut running animation
        if (astronautManager != null)
        {
            astronautManager.PlayRun();
        }
    }

    IEnumerator ButtonEffect()
    {
        float timer = 0f;

        // PRESS EFFECT
        while (timer < effectDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / effectDuration);

            transform.localScale = Vector3.Lerp(
                originalScale,
                originalScale * pressScale,
                t
            );

            background.color = Color.Lerp(
                normalColor,
                pressedColor,
                t
            );

            if (glow != null)
            {
                glow.color = new Color(
                    glowColor.r,
                    glowColor.g,
                    glowColor.b,
                    Mathf.Lerp(0f, glowIntensity, t)
                );
            }

            yield return null;
        }

        // RELEASE EFFECT
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

            background.color = Color.Lerp(
                pressedColor,
                normalColor,
                t
            );

            if (glow != null)
            {
                glow.color = new Color(
                    glowColor.r,
                    glowColor.g,
                    glowColor.b,
                    Mathf.Lerp(glowIntensity, 0f, t)
                );
            }

            yield return null;
        }

        // RESET
        transform.localScale = originalScale;
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

        effectRoutine = null;
    }
}
