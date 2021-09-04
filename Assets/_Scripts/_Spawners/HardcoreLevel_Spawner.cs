using UnityEngine;

public class HardcoreLevel_Spawner : MonoBehaviour
{
    private GameObject lastLevel, currentLevel;
    private GameObject[] levels;
    public GameObject[] LevelPrefabs;
    private int indexOfLevel, countLevel;
    private float PosZ, LengthZ;


    private void Start()
    {
        EventAggregator.loadEvents = new EventManager<LoadLevel>();
        EventAggregator.hideEvents = new EventManager<HideLevel>();
        EventAggregator.loadEvents.Subscribe(SpawnLevel);
        EventAggregator.hideEvents.Subscribe(HideLevel);

        levels = new GameObject[LevelPrefabs.Length];

        for (int i = 0; i < LevelPrefabs.Length; i++)
        {
            levels[i] = Instantiate(LevelPrefabs[i], transform);
            levels[i].SetActive(false);
        }

        indexOfLevel = Random.Range(0, levels.Length);

        SpawnLevel();
    }

    private void SpawnLevel()
    {
        lastLevel = currentLevel;

        currentLevel = levels[indexOfLevel];

        LengthZ = currentLevel.GetComponent<BoxCollider>().bounds.size.z;

        PosZ += LengthZ;

        currentLevel.transform.position = new Vector3(0, 0, PosZ + 499 * countLevel);

        countLevel++;

        currentLevel.SetActive(true);

        int lastIndex = indexOfLevel;

        indexOfLevel = Random.Range(0, levels.Length);

        while (indexOfLevel == lastIndex)
        {
            indexOfLevel = Random.Range(0, levels.Length);
        }
    }

    public UnityEngine.UI.Text t;
    public void DebugSetNextLevel(UnityEngine.UI.Slider sl)
    {
        indexOfLevel = (int)sl.value - 1;
        t.text = ((int)sl.value).ToString();
    }

    private void HideLevel()
    {
        lastLevel.SetActive(false);
    }
}
