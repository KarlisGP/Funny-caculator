using UnityEngine;

public class Worldmap : MonoBehaviour
{
    public Player player;

    private void OnMouseDown()
    {
        // Izveido staru no kameras līdz peles pozīcijai ekrānā
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Pārbaudi, vai stars saduras ar kādu objektu
        if (Physics.Raycast(ray, out hit))
        {
            // Ja sadursme notiek ar konkrēto spēles objektu
            if (hit.collider.gameObject == gameObject)
            {
                // Izveido jaunu mērķa pozīciju, pārvietojot to par 0.5f pa Y asi
                Vector3 newTarget = hit.point + new Vector3(0, transform.localScale.y / 2f, 0);
                //Vector3 newTarget = hit.point + new Vector3(0, 0.5f, 0);

                // Izvada mērķa pozīciju konsolē
                Debug.Log(newTarget);

                // TODO: Izsaukt korutīnu šeit, piemēram:
                //StartCoroutine(player.Movement(newTarget));

                player.Target = newTarget;
            }
        }
    }
}