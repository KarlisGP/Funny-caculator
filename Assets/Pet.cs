using System.Collections;
using UnityEngine;

public class Pet : MonoBehaviour
{
    // Publiskie mainīgie, kuri ir pieejami Unity inspektorā
    public AnimationCurve curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f); // Paātrināšanas un palēnināšanas līkne

    public Transform player; // Mērķa pozīcija objektam
    //public float duration = 1f; // Laiks, cik ilgi objekts kustēsies

    // Ja izmanto ātrumu:
    public float speed = 1f; // Kustības ātrums (vienības sekundē)
    
    public TextMesh durationText; // Teksta UI elements, kas rāda kustības progresu
    private Coroutine currentGoRoutine; // Glabā atsauci uz darbojošos korutīnu


    private void Start()
    {
        // Inicializē teksta elementu ar vērtību "0"
        durationText.text = "0";
    }

    // Update tiek izsaukts katrā kadra atjauninājumā
    void Update()
    {
        // Ja tiek nospiesta "G" poga, pārstartē korutīnu
        if (Input.GetKeyDown(KeyCode.G))
        {
            RestartCoroutine(); // Pārstartē korutīnu
        }
    }

    // Metode, kas aptur pašreizējo korutīnu un sāk jaunu
    void RestartCoroutine()
    {
        // Ja korutīna jau darbojas, aptur to
        if (currentGoRoutine != null)
        {
            StopCoroutine(currentGoRoutine);
        }

        // Sāk jaunu korutīnu, lai veiktu kustību
        currentGoRoutine = StartCoroutine(GoAndReturn(player.position));
    }

// Korutīna, kas pārvieto objektu no pašreizējās pozīcijas uz mērķi
    IEnumerator GoAndReturn(Vector3 target)
    {
        Vector3 petOriginPosition = this.transform.position; // Saglabā objekta sākuma pozīciju
        float startTime = Time.time; // Saglabā sākuma laiku
        
        // Ja izmanto ātrumu:
        float totalDistance = Vector3.Distance(petOriginPosition, target); // Aprēķina kopējo attālumu līdz mērķim
        float duration = totalDistance / speed; // Aprēķina ilgumu, balstoties uz ātrumu
        
        
        float procenti = 0f; // Inicializē progresu

        // Veic kustību, kamēr t ir mazāks vai vienāds ar 1
        while (procenti < 1)
        {
            // Aprēķina progresu, balstoties uz pagājušo laiku
            float timeSpent = (Time.time - startTime);
            procenti = Mathf.Clamp01(timeSpent / duration);
            // Atjauno tekstu, lai rādītu pašreizējo kustības progresu
            durationText.text = procenti.ToString("F2")+" %";
            
            // Pārvieto objektu, izmantojot Lerp un definēto līkni
            transform.position = Vector3.LerpUnclamped(petOriginPosition, target, curve.Evaluate(procenti));
            
            // Pagaida līdz nākamajam kadram
            yield return null;
        }

        // Kad kustība ir pabeigta, iestata objektu tieši mērķa pozīcijā
        this.transform.localPosition = target;
    }
}