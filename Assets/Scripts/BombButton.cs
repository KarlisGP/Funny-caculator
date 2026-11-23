
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BombButtonEffect : MonoBehaviour
{
    [Header("Button References")]
    public Button button;
    public Image background;
    public Image glow;

    [Header("Button Colors")]
    public Color normalColor = new Color(0.8f, 0.25f, 0.05f);
    public Color pressedColor = new Color(1f, 0.75f, 0.1f);

    [Header("Glow Settings")]
    public Color glowColor = new Color(1f, 0.65f, 0.05f);
    [Range(0f, 1f)]
    public float glowIntensity = 0.8f;

    [Header("Animation Settings")]
    public float pressScale = 0.9f;
    public float effectDuration = 0.2f;

    [Header("Button Sound")]
    public AudioSource audioSource;
    public AudioClip buttonSound;

    [Header("Astronaut Manager")]
    public AstronautManager astronautManager;

    [Header("Button Text")]
    public TMP_Text buttonText;
    public string bombMessage = "BOOM!";

    private Vector3 originalScale;
    private Coroutine effectRoutine;

    void Start()
    {
        originalScale = transform.localScale;

        if (background != null)
            background.color = normalColor;

        // Start with glow invisible
        if (glow != null)
        {
            glow.color = new Color(
                glowColor.r,
                glowColor.g,
                glowColor.b,
                0f
            );
        }

        // Connect button click
        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(PressButton);
    }

    void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(PressButton);
    }

    void PressButton()
    {
        // Play button click sound
        if (audioSource != null && buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }

        // Display button message
        if (buttonText != null)
        {
            buttonText.text = bombMessage;
        }

        // Play button visual effect
        if (effectRoutine != null)
        {
            StopCoroutine(effectRoutine);
        }

        effectRoutine = StartCoroutine(ButtonEffect());

        // Trigger bomb
        if (astronautManager != null)
        {
            astronautManager.PlayBomb();
        }
        else
        {
            Debug.LogError("Astronaut Manager is not assigned!");
        }
    }

    IEnumerator ButtonEffect()
    {
        float timer = 0f;

        // SHRINK AND LIGHT UP
        while (timer < effectDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / effectDuration);

            // Shrink button
            transform.localScale = Vector3.Lerp(
                originalScale,
                originalScale * pressScale,
                t
            );

            // Change button color
            if (background != null)
            {
                background.color = Color.Lerp(
                    normalColor,
                    pressedColor,
                    t
                );
            }

            // Fade glow in
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

        // RELEASE BUTTON
        timer = 0f;

        while (timer < effectDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / effectDuration);

            // Return button to original size
            transform.localScale = Vector3.Lerp(
                originalScale * pressScale,
                originalScale,
                t
            );

            // Return original color
            if (background != null)
            {
                background.color = Color.Lerp(
                    pressedColor,
                    normalColor,
                    t
                );
            }

            // Fade glow out
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

        // Reset everything
        transform.localScale = originalScale;

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

        effectRoutine = null;
    }
}