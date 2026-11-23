using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Publiski pieejami mainīgie, kurus var pielāgot Unity inspektorā
    public float shakeAmount = 0.5f; // Kameras trīcēšanas intensitāte
    public float shakeDuration = 0f; // Kameras trīcēšanas ilgums
    public float decreaseFactor = 1f; // Trīcēšanas samazināšanas ātrums

    private Vector3 originalPosition; // Kameras sākotnējā pozīcija

    // Izsaucama metode, kad objekts tiek aktivizēts (piemēram, sākot spēli)
    private void OnEnable()
    {
        // Saglabā sākotnējo kameras pozīciju
        originalPosition = transform.localPosition;
    }


// Update metode tiek izsaukta katrā kadru atjauninājumā
    void Update()
    {
        if (shakeDuration > 0)
        {
            // Ja shakeDuration ir lielāks par 0, veic trīcēšanu
            Shake();
            shakeDuration -= Time.deltaTime * decreaseFactor; // Samazini trīcēšanas ilgumu katru kadru
        }
        else
        {
            // Kad trīcēšanas ilgums beidzas, atgriez kameru sākotnējā pozīcijā
            shakeDuration = 0f;
            transform.localPosition = originalPosition;
        }
    }

    // Kameras trīcēšanas metode
    public void Shake()
    {
        // Pārvieto kameru trīcēšanas efekta robežās, izmantojot nejaušu pozīciju (insideUnitSphere)
        transform.localPosition = originalPosition + Random.insideUnitSphere * shakeAmount;
    }
}