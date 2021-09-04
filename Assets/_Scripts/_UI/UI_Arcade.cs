using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Arcade : MonoBehaviour
{
    #region Variables
    public static UI_Arcade Instance;
    public static int costOfContinues, costModifier;
    public TrailRenderer trail;
    public Slider timerView;
    public GameObject buttonContinue;
    public Text emeraldText, bestScoreText, costText, methodText, levelText;
    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        EventAggregator.loseEvents.Subscribe(Lose);
        EventAggregator.coinEvents.Subscribe(OnCoin);
        EventAggregator.emeraldEvents.Subscribe(OnEmerald);

        _Player.Speed = 45;
        _Player.Speed = 0;

        costOfContinues = 15;
        costModifier = 2;

        bestScoreText.text = _UserInterface.sv.BestScore.ToString();

        if (_UserInterface.sv.language == "Ru") levelText.text = "Аркада";
        else levelText.text = "Arcade";

        InvokeRepeating("Time_", 1, 1);
    }

    private IEnumerator GameContinue()
    {
        _Player.Speed = 0;

        _Player.CanTurn = false;

        BaseUI.Instance.GetPanel("PanelInGame").SetActive(false);

        AudioManager.StopAudio(BaseUI.Instance.inGameMusic[BaseUI.Instance.index]);
        AudioManager.PlayAudio("Lose1");
        AudioManager.PlayAudio("Lose2");

        if (SceneManager.GetActiveScene().name != "Game_Hardcore")
        {

            yield return new WaitForSeconds(1.85f);

            float timer = 1f;
            timerView.value = timer;
            Image fill = timerView.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
            
            BaseUI.Instance.GetPanel("PanelGameContinue").SetActive(true);

            costText.text = costOfContinues.ToString();
            emeraldText.text = _UserInterface.sv.emerald.ToString();

            while (timer > 0)
            {
                timerView.value = timer;
                fill.color = Color.HSVToRGB(timer / 3, 1, 1);
                timer -= Time.deltaTime / 4;
                yield return null;
            }
            
            BaseUI.Instance.GetPanel("PanelGameContinue").SetActive(false);
        }
        BaseUI.Instance.Invoke("PanelOfLost", 2);
    }

    private void Time_()
    {
        _UserInterface.sv.time["lastArcade"] = (int)Time.timeSinceLevelLoad;
    }

    private void Lose(_Player player)
    {
        if(_Player.ScoreInt > _UserInterface.sv.BestScore)
        {
            _UserInterface.sv.BestScore = _Player.ScoreInt;
            GPGamesManager.ReportScore(GPGSIds.leaderboard_high_scores_arcade, _Player.ScoreInt);
        }

        StartCoroutine("GameContinue");
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

                if (_Player.ScoreInt > _UserInterface.sv.BestScore) _UserInterface.sv.BestScore = _Player.ScoreInt;

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

                Spawner_Armor.HasStart = false;
                Spawner_ModifierScore.HasStart = false;
                Spawner_TimeAcceleration.HasStart = false;
                Spawner_TimeSlowing.HasStart = false;

                _UserInterface.sv.time["arcade"] += _UserInterface.sv.time["lastArcade"];

                NotifyManager.ClearNotify();

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;

            default:
                throw new System.ArgumentException("Invalid argument ", methodText.text);
        }
    }

    #endregion

    #region LostMenu

    public void Continue()
    {
        if (_UserInterface.sv.emerald >= costOfContinues)
        {
            StopCoroutine("GameContinue");

            BaseUI.Instance.GetPanel("PanelGameContinue").SetActive(false);
            BaseUI.Instance.GetPanel("PanelStartGame").SetActive(true);

            _UserInterface.sv.emerald -= costOfContinues;
            costOfContinues *= costModifier;

            _Player.Instance.player_Trail.Clear();
            _Player.Instance.player_Trail.gameObject.SetActive(false);

            _Player.CanTurn = true;
            _Player.IsMovingX = Checkpoint.IsMovingX;
            _Player.rb.position = Checkpoint.checkPos;
            _Player.rb.rotation = Checkpoint.checkRot;
            _Player.ScoreFloat = Checkpoint.checkScore;
            if (_Player.Speed > 60) _Player.Speed -= 5;           

            AudioManager.StopAudio("Lose1");
            AudioManager.StopAudio("Lose2");
        }
        else
        {
            buttonContinue.GetComponent<Image>().color = Color.red;
            Invoke("ReturnColor", 0.5f);

            AudioManager.PlayAudio("Deny");
        }
    }

    public void SkipGameContinue()
    {
        StopCoroutine("GameContinue");
        
        BaseUI.Instance.GetPanel("PanelGameContinue").SetActive(false);
        BaseUI.Instance.Invoke("PanelOfLost", 2);
    }

    #endregion

    public void StartGame()
    {
        _Player.Speed = -1;
    }
}
