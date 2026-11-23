
using UnityEngine;
using System.Collections;

public class AstronautManager : MonoBehaviour
{
    [Header("Explosion Sound")]
    public AudioClip explosionSound;
    [Range(0f, 1f)]
    public float explosionVolume = 1f;

    [Header("Astronaut Death Effect")]
    public GameObject astronautDeathEffectPrefab;
    public float deathEffectDuration = 3f;

    [Header("Astronaut Prefabs")]
    public GameObject idlePrefab;
    public GameObject hammerPrefab;
    public GameObject fartPrefab;
    public GameObject runPrefab;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    [Header("Action Durations")]
    public float hammerDuration = 2f;
    public float fartDuration = 2f;
    public float runDuration = 2f;

    [Header("Bomb Settings")]
    public GameObject bombPrefab;
    public Transform bombStartPoint;
    public Transform bombTargetPoint;

    [Header("Bomb Timing")]
    public float bombFallDuration = 3f;
    public float bombExplosionDuration = 1f;

    [Header("Explosion Camera Shake")]
    public Transform cameraTransform;
    public float shakeDuration = 0.4f;
    public float shakeMagnitude = 0.2f;

    [Header("Gas Effect")]
    public GasFillEffect gasFillEffect;

    private GameObject currentAstronaut;
    private GameObject currentBomb;
    private GameObject currentDeathEffect;

    private Coroutine currentAction;
    private Coroutine cameraShakeRoutine;

    private Vector3 cameraRestPosition;

    private bool isDead = false;

    void Start()
    {
        if (cameraTransform != null)
            cameraRestPosition = cameraTransform.localPosition;

        SpawnAstronaut(idlePrefab);
    }

    // SPAWN ASTRONAUT
    void SpawnAstronaut(GameObject prefab)
    {
        if (prefab == null || spawnPoint == null)
        {
            Debug.LogError("Astronaut prefab or Spawn Point is missing!");
            return;
        }

        if (currentAstronaut != null)
            Destroy(currentAstronaut);

        currentAstronaut = Instantiate(
            prefab,
            spawnPoint.position,
            spawnPoint.rotation
        );
    }

    // NORMAL ACTIONS
    void StartAction(GameObject prefab, float duration)
    {
        if (isDead) return;

        if (currentAction != null)
            StopCoroutine(currentAction);

        currentAction = StartCoroutine(
            ActionSequence(prefab, duration)
        );
    }

    IEnumerator ActionSequence(GameObject prefab, float duration)
    {
        SpawnAstronaut(prefab);

        yield return new WaitForSeconds(duration);

        if (!isDead)
            SpawnAstronaut(idlePrefab);

        currentAction = null;
    }

    public void PlayHammer()
    {
        if (isDead) return;

        if (gasFillEffect != null)
            gasFillEffect.ClearGas();

        StartAction(hammerPrefab, hammerDuration);
    }

    public void PlayFart()
    {
        if (isDead) return;

        StartAction(fartPrefab, fartDuration);
    }

    public void PlayRun()
    {
        if (isDead) return;

        if (gasFillEffect != null)
            gasFillEffect.ClearGas();

        StartAction(runPrefab, runDuration);
    }

    // BOMB BUTTON
    public void PlayBomb()
    {
        if (isDead) return;

        if (bombPrefab == null ||
            bombStartPoint == null ||
            bombTargetPoint == null)
        {
            Debug.LogError(
                "Assign Bomb Prefab, Bomb Start Point and Bomb Target Point!"
            );
            return;
        }

        if (currentAction != null)
            StopCoroutine(currentAction);

        if (gasFillEffect != null)
            gasFillEffect.ClearGas();

        isDead = true;

        currentAction = StartCoroutine(BombSequence());
    }

    IEnumerator BombSequence()
    {
        Debug.Log("Bomb sequence started!");

        Vector3 startPosition = bombStartPoint.position;
        Vector3 targetPosition = bombTargetPoint.position;

        // Create bomb
        currentBomb = Instantiate(
            bombPrefab,
            startPosition,
            bombStartPoint.rotation
        );

        float elapsed = 0f;

        // Move bomb towards astronaut
        while (elapsed < bombFallDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsed / bombFallDuration
            );

            if (currentBomb != null)
            {
                currentBomb.transform.position = Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    progress
                );
            }

            yield return null;
        }

        if (currentBomb == null)
        {
            currentAction = null;
            isDead = false;
            yield break;
        }

        currentBomb.transform.position = targetPosition;

        // PLAY EXPLOSION SOUND
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(
                explosionSound,
                targetPosition,
                explosionVolume
            );
        }

        // SHAKE CAMERA ON EXPLOSION
        if (cameraTransform != null)
        {
            if (cameraShakeRoutine != null)
                StopCoroutine(cameraShakeRoutine);

            cameraTransform.localPosition = cameraRestPosition;

            cameraShakeRoutine = StartCoroutine(ShakeCamera());
        }

        // Wait for bomb explosion animation
        yield return new WaitForSeconds(bombExplosionDuration);

        // PLAY ASTRONAUT DEATH ANIMATION
        if (currentAstronaut != null)
        {
            Vector3 deathPosition = currentAstronaut.transform.position;
            Quaternion deathRotation = currentAstronaut.transform.rotation;

            Destroy(currentAstronaut);
            currentAstronaut = null;

            if (astronautDeathEffectPrefab != null)
            {
                currentDeathEffect = Instantiate(
                    astronautDeathEffectPrefab,
                    deathPosition,
                    deathRotation
                );

                // Wait for death animation to finish
                yield return new WaitForSeconds(deathEffectDuration);

                if (currentDeathEffect != null)
                {
                    Destroy(currentDeathEffect);
                    currentDeathEffect = null;
                }
            }
        }

        // REMOVE BOMB
        if (currentBomb != null)
        {
            Destroy(currentBomb);
            currentBomb = null;
        }

        // RETURN TO IDLE
        SpawnAstronaut(idlePrefab);

        isDead = false;
        currentAction = null;

        Debug.Log("Astronaut returned to idle!");
    }

    // CAMERA SHAKE
    IEnumerator ShakeCamera()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            float strength = 1f - Mathf.Clamp01(
                elapsed / shakeDuration
            );

            Vector2 randomOffset = Random.insideUnitCircle
                * shakeMagnitude
                * strength;

            cameraTransform.localPosition = cameraRestPosition
                + new Vector3(
                    randomOffset.x,
                    randomOffset.y,
                    0f
                );

            yield return null;
        }

        cameraTransform.localPosition = cameraRestPosition;
        cameraShakeRoutine = null;
    }

    // RESPAWN BUTTON
    public void Respawn()
    {
        // Stop current action
        if (currentAction != null)
        {
            StopCoroutine(currentAction);
            currentAction = null;
        }

        // Stop camera shake and restore camera
        if (cameraShakeRoutine != null)
        {
            StopCoroutine(cameraShakeRoutine);
            cameraShakeRoutine = null;
        }

        if (cameraTransform != null)
            cameraTransform.localPosition = cameraRestPosition;

        // Remove bomb
        if (currentBomb != null)
        {
            Destroy(currentBomb);
            currentBomb = null;
        }

        // Remove death animation
        if (currentDeathEffect != null)
        {
            Destroy(currentDeathEffect);
            currentDeathEffect = null;
        }

        // Clear gas
        if (gasFillEffect != null)
            gasFillEffect.ClearGas();

        isDead = false;

        // Return astronaut to idle immediately
        SpawnAstronaut(idlePrefab);

        Debug.Log("Astronaut respawned!");
    }
}
