using UnityEngine;

public class Spawner_TimeAcceleration : MonoBehaviour
{
    public static bool HasStart;
    private int a = 0, index;
    public GameObject[] GO;


    private void Start()
    {
        if (!HasStart)
        {
            GO[0].GetComponent<TimeAcceleration>().PowerOff();
            HasStart = true;
        }

        foreach (GameObject go in GO)
        {
            go.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        while (a < Random.Range(0, GO.Length))
        {
            index = Random.Range(0, GO.Length);

            if (GO[index].activeSelf == false)
            {
                GO[index].SetActive(true);
                a++;
            }
        }
    }
}
