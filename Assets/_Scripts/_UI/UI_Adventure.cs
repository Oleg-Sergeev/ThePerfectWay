using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_Adventure : MonoBehaviour
{
    #region Variables
    private const string MUSIC = "Music", SOUND = "Sound";
    public static UI_Adventure Instance;
    public Sprite[] sprites;
    public Image[] stars;
    public Text[] textsOnFinish, collectedMoney, allMoney;
    public Text textMethod, textBonusCode, textLevel;
    public GameObject[] levels;
    public GameObject starsOnFinish;
    private Color[] textColor = { Color.red, Color.green };
    private Dictionary<string, Color> colors = new Dictionary<string, Color>()
    {
        {"red", new Color(0.8f, 0.2f, 0.2f) },
        {"green", Color.green },
        {"grey", new Color(0.7f, 0.7f, 0.7f) },
        {"white", Color.white }
    };
    #endregion


    private void Awake()
    {
        if (Instance == null) Instance = this;

        EventAggregator.finishEvents = new EventManager<Finish>();
    }

    private void Start()
    {
        _Player.Speed = 35;
        _Player.Speed = 0;

        EventAggregator.coinEvents.Subscribe(OnCoin);
        EventAggregator.emeraldEvents.Subscribe(OnEmerald);
        EventAggregator.loseEvents.Subscribe(Lose);
        EventAggregator.finishEvents.Subscribe(OnFinish);

        levels = new GameObject[gameObject.transform.GetChild(0).childCount - 2];
        for (int i = 0; i < gameObject.transform.GetChild(0).childCount - 2; i++)
        {
            levels[i] = gameObject.transform.GetChild(0).GetChild(i+1).gameObject;
        }

        InvokeRepeating("Time_", 1, 1);

        LevelSelection();
    }

    private void OnApplicationQuit()
    {
        _UserInterface.sv.gameMode = "ARCADE";

        _UserInterface.sv.Save();
    }

    private void OnApplicationFocus(bool focus)
    {
        _UserInterface.sv.Save();
    }

    private void Time_()
    {
        _UserInterface.sv.time["lastAdventure"] = (int)Time.timeSinceLevelLoad;
    }

    private void OnFinish(Finish finish)
    {
        _Player.Speed = 0;

        BaseUI.Instance.GetPanel("PanelFinish").SetActive(true);
        BaseUI.Instance.GetPanel("PanelInGame").SetActive(false);

        if (finish.gameObject.tag == "SecretWin") textBonusCode.gameObject.SetActive(true);
        else textBonusCode.gameObject.SetActive(false);

        for (int i = 0; i < 3; i++)
        {
            starsOnFinish.transform.GetChild(i).GetComponent<Image>().sprite = sprites[_UserInterface.sv.levels[_UserInterface.sv.currentLevel - 1].stars[i]];
        }

        textsOnFinish[0].text = LanguageManager.GetLocalizedValue("Level") + _UserInterface.sv.currentLevel + LanguageManager.GetLocalizedValue("Completed");
        for (int i = 1; i < textsOnFinish.Length; i++)
        {
            textsOnFinish[i].color = textColor[_UserInterface.sv.levels[_UserInterface.sv.currentLevel - 1].stars[i - 1]];
        }
    }

    private void Lose(_Player player)
    {
        _Player.Speed = 0;
        _Player.CanTurn = false;
        
        BaseUI.Instance.GetPanel("PanelInGame").SetActive(false);
        BaseUI.Instance.Invoke("PanelOfLost", 1.85f);

        AudioManager.StopAudio("InGame");
        AudioManager.PlayAudio("Lose1");
        AudioManager.PlayAudio("Lose2");
    }

    private void OnCoin(Coins coin)
    {
        Finish.coinCount++;

        collectedMoney[0].text = Finish.coinCount.ToString();

        if (int.Parse(collectedMoney[0].text) >= AdventureLevelsManager.Instance.allCoins[_UserInterface.sv.currentLevel].transform.childCount)
        {
            _UserInterface.sv.levels[_UserInterface.sv.currentLevel - 1].stars[1] = 1;

            OnStar(1);
        }
    }
    private void OnEmerald(Emeralds emerald)
    {
        Finish.emeraldCount++;

        collectedMoney[1].text = Finish.emeraldCount.ToString();
    }

    public void OnStar(int star)
    {
        stars[star].sprite = sprites[1];
    }

    public void LevelSelection()
    {
        BaseUI.Instance.SetPanels(false);

        for (int i = 1; i < levels.Length; i++)
        {
            int level = int.Parse(levels[i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text);

            string colorImg, colorText;
            bool interactable = true;

            for (int j = 0; j < 3; j++)
            {
                levels[i].transform.GetChild(1).transform.GetChild(j).GetComponent<Image>().sprite = sprites[_UserInterface.sv.levels[i - 1].stars[j]];
            }

            if (level > _UserInterface.sv.completedLevel + 1)
            {
                    colorImg = "grey";
                    colorText = "red";
                    interactable = false;
            }
            else if (level == _UserInterface.sv.completedLevel + 1)
            {
                colorImg = "grey";
                colorText = "white";
            }
            else
            {
                colorImg = "white";
                colorText = "green";
            }

            levels[i].transform.GetChild(0).GetComponent<Image>().color = colors[colorImg];
            levels[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().color = colors[colorText];
            levels[i].transform.GetChild(0).GetComponent<Button>().interactable = interactable;
        }

        BaseUI.Instance.GetPanel("PanelLevelSelection").SetActive(true);
    }

    #region PauseMenu
    public void ConfirmAction(string method)
    {
        BaseUI.Instance.GetPanel("PanelExit").SetActive(true);

        switch (method)
        {
            case "Home":
                textMethod.text = "RETURN TO HOME?";
                if (_UserInterface.sv.language == "Ru") textMethod.text = "ВЕРНУТЬСЯ ДОМОЙ?";
                break;

            case "Restart":
                textMethod.text = "RESTART LEVEL?";
                if (_UserInterface.sv.language == "Ru") textMethod.text = "ПЕРЕЗАПУСТИТЬ УРОВЕНЬ?";
                break;

            case "Select":
                textMethod.text = "SELECT LEVEL?";
                if (_UserInterface.sv.language == "Ru") textMethod.text = "ВЫБРАТЬ УРОВЕНЬ?";
                break;

            default:
                throw new System.ArgumentException("Invalid argument ", method);
        }
    }

    public void NO()
    {
        BaseUI.Instance.GetPanel("PanelExit").SetActive(false);
    }
    public void YES()
    {
        switch (textMethod.text)
        {
            case "ВЕРНУТЬСЯ ДОМОЙ?":
            case "RETURN TO HOME?":

                Time.timeScale = 1;

                _UserInterface.sv.Save();

                LoadingScene.SceneName = "MainMenu";

                BaseUI.Instance.loading_object.SetActive(true);
                BaseUI.Instance.loadingScreen.SetActive(true);

                break;

            case "ПЕРЕЗАПУСТИТЬ УРОВЕНЬ?":
            case "RESTART LEVEL?":
                _UserInterface.sv.time["adventure"] += _UserInterface.sv.time["lastAdventure"];

                AdventureLevelsManager.Instance.Restart();
                break;

            case "ВЫБРАТЬ УРОВЕНЬ?":
            case "SELECT LEVEL?":
                LevelSelection();
                break;
        }
    }
    #endregion

    public void StartGame()
    {
        _Player.Speed = -1;
        _Player.IsMovingX = true;
    }
}
