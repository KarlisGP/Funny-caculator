using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector3 target;
    public CameraShake cameraShake;

    private Coroutine moveCoroutine; // Izseko pašreizējo korutīnu

    // Publiska īpašība 'Target', kas iedarbina korutīnu, kad mainās mērķa vērtība
    public Vector3 Target
    {
        get { return target; }
        set
        {
            target = value;


            //LeanTween variants:
            LeanTween.cancel(gameObject);
            float distance = Vector3.Distance(transform.position, target); //10 metri
            float speed = 3f;
            float moveDuration = distance / speed;
            LeanTween.move(gameObject, target, moveDuration).setEase(LeanTweenType.easeInOutCubic);


            return;

            // Pārbaude, vai korutīna jau darbojas, un apturēt, ja nepieciešams
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            // Sākt jaunu korutīnu ar jauno mērķi
            moveCoroutine = StartCoroutine(Movement(target));
        }
    }

    // Korutīna, kas pārvieto objektu uz mērķa pozīciju
    public IEnumerator Movement(Vector3 target)
    {
        // Kamēr attālums līdz mērķim ir lielāks par 0.05f
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            // Kustība no pašreizējās pozīcijas uz mērķa pozīciju, izmantojot Lerp funkciju
            transform.position = Vector3.Lerp(transform.position, target, 0.05f);
            yield return null; // Gaidīt nākamo kadru
        }

        cameraShake.shakeDuration = 0.2f;

        moveCoroutine = null; // Atbrīvot korutīnas mainīgo
    }
}