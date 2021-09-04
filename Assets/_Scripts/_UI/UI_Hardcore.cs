using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Hardcore : MonoBehaviour
{
    #region Variables
    public static UI_Hardcore Instance;
    public Text d2, d3, bestScore_HardcoreText, methodText, levelText;
    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;

        _Player.modifier = 4;
    }

    private void Start()
    {
        EventAggregator.loseEvents.Subscribe(Lose);
        EventAggregator.coinEvents.Subscribe(OnCoin);
        EventAggregator.emeraldEvents.Subscribe(OnEmerald);

        bestScore_HardcoreText.text = _UserInterface.sv.BestScore_Hardcore.ToString();

        _Player.Speed = 45;
        _Player.Speed = 0;

        d2.text = "45";
        d3.text = "4";

        if (_UserInterface.sv.language == "Ru") levelText.text = "Хардкор";
        else levelText.text = "Hardcore";

        InvokeRepeating("Time_", 1, 1);
    }

    private void Time_()
    {
        _UserInterface.sv.time["lastHardcore"] = (int)Time.timeSinceLevelLoad;
    }

    private void Lose(_Player player)
    {
        _Player.Speed = 0;

        if (_Player.ScoreInt > _UserInterface.sv.BestScore_Hardcore)
        {
            _UserInterface.sv.BestScore_Hardcore = _Player.ScoreInt;
            GPGamesManager.ReportScore(GPGSIds.leaderboard_high_scores_hardcore, _Player.ScoreInt);
        }
        
        _Player.CanTurn = false;

        BaseUI.Instance.GetPanel("PanelInGame").SetActive(false);

        AudioManager.StopAudio("InGame");
        AudioManager.PlayAudio("Lose1");
        AudioManager.PlayAudio("Lose2");

        BaseUI.Instance.Invoke("PanelOfLost", 2);
    }

    private void OnCoin(Coins coin)
    {
        coin.Invoke("_SetTrue", 50 / _Player.Speed * 10);

        _Player.coins_count++;
    }
    private void OnEmerald(Emeralds emerald)
    {
        emerald.Invoke("_SetTrue", 50 / _Player.Speed * 10);

        _Player.emeralds_count++;
    }

    #region PauseMenu

    #region Debug

    public void OpenPanel(GameObject pan)
    {
        pan.SetActive(true);
    }

    public void FigureSpeed(Slider sl)
    {
        _Player.Speed = sl.value;
        d2.text = sl.value.ToString();
    }

    public void Koef(Slider sl)
    {
        _Player.modifier = (int)sl.value;
        d3.text = sl.value.ToString();
    }

    public void ClosePanel(GameObject pan)
    {
        pan.SetActive(false);
    }

    #endregion

    public void ConfirmAction(string method)
    {
        BaseUI.Instance.GetPanel("PanelExit").SetActive(true);

        switch (method)
        {
            case "Home":
                methodText.text = "RETURN TO HOME?";
                if (_UserInterface.sv.language == "Ru") methodText.text = "ВЕРНУТЬСЯ ДОМОЙ?";
                break;

            case "Restart":
                methodText.text = "RESTART GAME?";
                if (_UserInterface.sv.language == "Ru") methodText.text = "НАЧАТЬ ИГРУ ЗАНОВО?";
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
        switch (methodText.text)
        {
            case "ВЕРНУТЬСЯ ДОМОЙ?":
            case "RETURN TO HOME?":

                Time.timeScale = 1;

                if (_Player.ScoreInt > _UserInterface.sv.BestScore_Hardcore) _UserInterface.sv.BestScore_Hardcore = _Player.ScoreInt;

                LoadingScene.SceneName = "MainMenu";

                Time.timeScale = 0;

                BaseUI.Instance.GetPanel("PanelExit").SetActive(false);
                BaseUI.Instance.GetPanel("PanelPauseMenu").SetActive(false);
                BaseUI.Instance.GetPanel("PanelLost").SetActive(false);

                BaseUI.Instance.loading_object.SetActive(true);
                BaseUI.Instance.loadingScreen.SetActive(true);

                _UserInterface.sv.Save();

                break;

            case "НАЧАТЬ ИГРУ ЗАНОВО?":
            case "RESTART GAME?":

                _UserInterface.sv.time["hardcore"] += _UserInterface.sv.time["lastHardcore"];

                NotifyManager.ClearNotify();

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;

            default:
                throw new System.ArgumentException("Invalid argument ", methodText.text);
        }
    }

    #endregion

    public void StartGame()
    {
        _Player.Speed = -1;
    }
}
