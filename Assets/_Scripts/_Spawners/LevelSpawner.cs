using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    private int indexOfLevel, countLevel;
    private float PosX, PosZ, LevelLengthX, LevelLengthZ;
    private GameObject currentLevel, lastLevel;
    private GameObject[] low, high;
    public GameObject[] lowSpeedLevel, highSpeedLevel;


    private void Start()
    {
        EventAggregator.loadEvents = new EventManager<LoadLevel>();
        EventAggregator.hideEvents = new EventManager<HideLevel>();
        EventAggregator.loadEvents.Subscribe(SpawnLevel);
        EventAggregator.hideEvents.Subscribe(HideLevel);

        low = new GameObject[lowSpeedLevel.Length];
        high = new GameObject[highSpeedLevel.Length];

        for (int i = 0; i < lowSpeedLevel.Length; i++)
        {
            low[i] = Instantiate(lowSpeedLevel[i], transform);
            low[i].SetActive(false);
        }
        for (int i = 0; i < highSpeedLevel.Length; i++)
        {
            high[i] = Instantiate(highSpeedLevel[i], transform);
            high[i].SetActive(false);
        }

        SpawnLevel();        
    }

    private void SpawnLevel()
    {
        lastLevel = currentLevel;

        if (_Player.Speed <= 60)
        {
            int lastIndex = indexOfLevel;

            indexOfLevel = (indexOfLevel == 2) ? indexOfLevel = 3 : indexOfLevel = Random.Range(0, low.Length);

            while (indexOfLevel == lastIndex)
            {
                indexOfLevel = (indexOfLevel == 2) ? indexOfLevel = 3 : indexOfLevel = Random.Range(0, low.Length);
            }

            currentLevel = low[indexOfLevel];
        }
        else
        {
            int lastIndex = indexOfLevel;

            indexOfLevel = Random.Range(0, high.Length);

            while (indexOfLevel == lastIndex)
            {
                indexOfLevel = Random.Range(0, high.Length);
            }

            currentLevel = high[indexOfLevel];
        }

        LevelLengthX = currentLevel.GetComponent<BoxCollider>().bounds.size.x;
        LevelLengthZ = currentLevel.GetComponent<BoxCollider>().bounds.size.z;

        PosX += LevelLengthX;
        PosZ += LevelLengthZ;

        currentLevel.transform.position = new Vector3(PosX + 490 * countLevel, 0, PosZ + 500 * countLevel);

        countLevel++;
        
        currentLevel.SetActive(true);
    }

    private void HideLevel()
    {
        lastLevel.SetActive(false);
    }
}
