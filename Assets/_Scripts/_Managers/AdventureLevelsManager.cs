using UnityEngine;

public class AdventureLevelsManager : MonoBehaviour
{
    public static AdventureLevelsManager Instance;
    private Transform[] startPositions;
    [HideInInspector]
    public GameObject[] levels, allCoins, allEmeralds;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        allCoins = new GameObject[levels.Length];
        allEmeralds = new GameObject[levels.Length];
        startPositions = new Transform[levels.Length];
        for (int i = 0; i < levels.Length; i++)
        {
            allCoins[i] = levels[i].transform.GetChild(0).GetChild(0).gameObject;
            allEmeralds[i] = levels[i].transform.GetChild(0).GetChild(1).gameObject;
            startPositions[i] = levels[i].transform.GetChild(1);
        }
    }

    private void Start()
    {
        for (int i = 0; i < levels.Length; i++) levels[i].SetActive(false);

        if (_UserInterface.sv.completedLevel < 0 || _UserInterface.sv.completedLevel > levels.Length - 1) _UserInterface.sv.completedLevel = 0;
    }

    public void Restart()
    {
        _Player.rb.rotation = Quaternion.identity;
        _Player.Speed = 0;
        _Player.IsMovingX = true;

        _Player.Instance.player_Trail.gameObject.SetActive(false);
        _Player.Instance.transform.position = startPositions[_UserInterface.sv.currentLevel].position;
        _Player.Instance.player_Trail.gameObject.SetActive(true);
        _Player.Instance.player_Trail.Clear();
        levels[_UserInterface.sv.currentLevel].SetActive(true);

        for (int i = 0; i < 3; i++)
        {
            UI_Adventure.Instance.stars[i].sprite = UI_Adventure.Instance.sprites[_UserInterface.sv.levels[_UserInterface.sv.currentLevel - 1].stars[i]];
        }

        UI_Adventure.Instance.allMoney[0].text = allCoins[_UserInterface.sv.currentLevel].transform.childCount.ToString();
        UI_Adventure.Instance.allMoney[1].text = allEmeralds[_UserInterface.sv.currentLevel].transform.childCount.ToString();

        BaseUI.Instance.SetPanels(false);
        BaseUI.Instance.GetPanel("PanelStartGame").SetActive(true);
    }

    public void SetLevel(UnityEngine.UI.Text textLevel)  
    {
        int selectedLevel = int.Parse(textLevel.text);

        if (selectedLevel <= _UserInterface.sv.completedLevel + 1)
        {
            levels[_UserInterface.sv.currentLevel].SetActive(false);
            levels[selectedLevel].SetActive(true);

            _UserInterface.sv.currentLevel = selectedLevel;

            if (_UserInterface.sv.levels[_UserInterface.sv.currentLevel - 1].stars[1] == 0)
            {
                for (int i = 0; i < allCoins[_UserInterface.sv.currentLevel].transform.childCount; i++)
                {
                    allCoins[_UserInterface.sv.currentLevel].transform.GetChild(i).gameObject.SetActive(true);
                }
            }

            UI_Adventure.Instance.textLevel.text = LanguageManager.GetLocalizedValue("Level") + selectedLevel;

            for (int i = 0; i < 3; i++)
            {
                UI_Adventure.Instance.stars[i].sprite = UI_Adventure.Instance.sprites[_UserInterface.sv.levels[_UserInterface.sv.currentLevel - 1].stars[i]];
            }

            for (int i = 0; i < 2; i++) UI_Adventure.Instance.collectedMoney[i].text = "0";
            UI_Adventure.Instance.allMoney[0].text = allCoins[_UserInterface.sv.currentLevel].transform.childCount.ToString();
            UI_Adventure.Instance.allMoney[1].text = allEmeralds[_UserInterface.sv.currentLevel].transform.childCount.ToString();

            _Player.Speed = 0;

            _Player.Instance.player_Trail.gameObject.SetActive(false);
            _Player.Instance.transform.position = startPositions[selectedLevel].position;
            _Player.Instance.transform.rotation = Quaternion.identity;
            _Player.Instance.player_Trail.Clear();
            _Player.Instance.player_Trail.gameObject.SetActive(true);

            Finish.coinCount = 0;
            Finish.emeraldCount = 0;

            BaseUI.Instance.GetPanel("PanelFinish").SetActive(false);
            BaseUI.Instance.GetPanel("PanelLevelSelection").SetActive(false);
            BaseUI.Instance.GetPanel("PanelStartGame").SetActive(true);
        }
    }

    public void NextLevel()
    {
        if (_UserInterface.sv.currentLevel == _UserInterface.sv.levelsCount)
        {
            return;
        }

        _UserInterface.sv.currentLevel++;

        _UserInterface.sv.Save();

        UI_Adventure.Instance.textLevel.text = LanguageManager.GetLocalizedValue("Level") + _UserInterface.sv.currentLevel;

        for (int i = 0; i < 3; i++)
        {
            UI_Adventure.Instance.stars[i].sprite = UI_Adventure.Instance.sprites[_UserInterface.sv.levels[_UserInterface.sv.currentLevel - 1].stars[i]];
        }

        for (int i = 0; i < 2; i++) UI_Adventure.Instance.collectedMoney[i].text = "0";
        UI_Adventure.Instance.allMoney[0].text = allCoins[_UserInterface.sv.currentLevel].transform.childCount.ToString();
        UI_Adventure.Instance.allMoney[1].text = allEmeralds[_UserInterface.sv.currentLevel].transform.childCount.ToString();

        Finish.coinCount = 0;
        Finish.emeraldCount = 0;

        if (_UserInterface.sv.currentLevel < levels.Length) levels[_UserInterface.sv.currentLevel].SetActive(true);
        else Debug.Log("Else work");
        levels[_UserInterface.sv.currentLevel - 1].SetActive(false);

        _Player.Instance.player_Trail.gameObject.SetActive(false);
        _Player.Instance.transform.position = startPositions[_UserInterface.sv.currentLevel].position;
        _Player.Instance.transform.rotation = Quaternion.identity;
        _Player.Instance.player_Trail.gameObject.SetActive(true);
        _Player.Instance.player_Trail.Clear();

        _Player.IsMovingX = true;
        
        BaseUI.Instance.GetPanel("PanelFinish").SetActive(false);
        BaseUI.Instance.GetPanel("PanelStartGame").SetActive(true);
    }
}
