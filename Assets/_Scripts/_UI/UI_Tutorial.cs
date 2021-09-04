using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Tutorial : MonoBehaviour
{
    #region Variables
    private const string MUSIC = "Music", SOUND = "Sound";
    public static UI_Tutorial Instance;
    public static bool IsPlayTutorial, IsTutorialStarted, IsMovingX;
    private bool IsRestart;
    private Vector3 pos;
    public TrailRenderer player_tr;
    public Button jump;
    public Image fillSound, fillMusic;
    public Slider sl_Sound, sl_Music;
    public Sprite[] sprites = new Sprite[4];
    public GameObject buttonSound, buttonMusic, loading_object, loadingScreen, money;
    public GameObject[] panels, tips;
    public Text winText, methodText, startText, textCoin, textEmerald;
    public Text[] alltexts;
    public Dictionary<string, GameObject> panelsDict = new Dictionary<string, GameObject>();
    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;

        EventAggregator.coinEvents = new EventManager<Coins>();
        EventAggregator.emeraldEvents = new EventManager<Emeralds>();
        EventAggregator.loseEvents = new EventManager<_Player>();
        EventAggregator.coinEvents.Subscribe(OnCoin);
        EventAggregator.emeraldEvents.Subscribe(OnEmerald);
        EventAggregator.loseEvents.Subscribe(Lose);

        loadingScreen.SetActive(false);
        loading_object.SetActive(false);
    }

    void Start ()
    {
        IsTutorialStarted = false;

        _UserInterface.IsArcade = true;

        _UserInterface.sv.boost["Jump"].amount++;

        if (_UserInterface.sv.IsTutorialPassed) Destroy(money);

        loading_object.GetComponentInChildren<Renderer>().material.color = _UserInterface.sv.colorFigure;

        Time.timeScale = 0;

        winText.gameObject.SetActive(false);

        for (int i = 0; i < panels.Length; i++)
        {
            panelsDict.Add(panels[i].name, panels[i]);
            panels[i].SetActive(false);
        }
        panelsDict["PanelStartGame"].SetActive(true);

        for (int i = 0; i < tips.Length; i++)
        {
            tips[i].SetActive(false);
        }

        #region Audio
        if (_UserInterface.sv.soundVolume <= 0) buttonSound.GetComponent<Image>().sprite = sprites[0];
        else buttonSound.GetComponent<Image>().sprite = sprites[1];
        buttonSound.GetComponent<Image>().color = _UserInterface.sv.colorbttnSound;

        if (_UserInterface.sv.musicVolume <= 0) buttonMusic.GetComponent<Image>().sprite = sprites[2];
        else buttonMusic.GetComponent<Image>().sprite = sprites[3];
        buttonMusic.GetComponent<Image>().color = _UserInterface.sv.colorbttnMusic;

        fillSound.color = Color.HSVToRGB(sl_Sound.value / 3f, 1, 1);
        fillMusic.color = Color.HSVToRGB(sl_Music.value / 3f, 1, 1);

        sl_Music.value = _UserInterface.sv.musicVolume;
        sl_Sound.value = _UserInterface.sv.soundVolume;

        AudioManager.StopAllAudio();
        AudioManager.ChangeVolumeAll(MUSIC, _UserInterface.sv.musicVolume);
        AudioManager.ChangeVolumeAll(SOUND, _UserInterface.sv.soundVolume);
        #endregion

        LanguageManager.ChangeLanguage();
        for (int i = 0; i < alltexts.Length; i++) alltexts[i].text = LanguageManager.GetLocalizedValue(alltexts[i].name);
    }

    public void OnApplicationQuit()
    {
        _UserInterface.sv.gameMode = "ARCADE";

        _UserInterface.sv.Save();
    }

    private void OnApplicationPause(bool pause)
    {
        _UserInterface.sv.Save();
    }

    private void OnApplicationFocus(bool focus)
    {
        _UserInterface.sv.Save();
    }

    private void OnCoin(Coins coin)
    {
        _UserInterface.sv.coin += coin.coin;
        _UserInterface.sv.coinsCollected++;

        AudioManager.PlayAudio("CoinPickUp");

        textCoin.text = _UserInterface.sv.coin.ToString();
    }
    private void OnEmerald(Emeralds emerald)
    {
        _UserInterface.sv.emerald++;
        _UserInterface.sv.emeraldsCollected++;

        AudioManager.PlayAudio("EmeraldPickUp");

        textEmerald.text = _UserInterface.sv.emerald.ToString();
    }

    private void Lose(_Player player)
    {
        _Player.CanTurn = false;

        panelsDict["PanelLost"].SetActive(true);

        AudioManager.StopAudio("InTutorial");
        AudioManager.PlayAudio("Lose1");
    }

    public void ShowBoost(bool enabled)
    {
        jump.interactable = enabled;

        if (_UserInterface.sv.boost["Jump"].amount <= 0) jump.GetComponent<Image>().color = Color.red;
    }

    public void OnButtonDown(JumpBoost call)
    {
        EventAggregator.jumpEvents.Publish(call);
    }


    public void OnTrigger(int number)
    {
        EndTutorial.HasGetTip = true;

        Time.timeScale = 0;

        panelsDict["PanelOfTutorial"].SetActive(true);

        AudioManager.ChangeAudioVolume("InTutorial", _UserInterface.sv.musicVolume / 3);

        for (int i = 0; i < tips.Length; i++) tips[i].SetActive(false);
        tips[number].SetActive(true);
    }

    public void StartTutorial()
    {
        if (!IsTutorialStarted)
        {
            panelsDict["PanelStartGame"].SetActive(false);
            panelsDict["PanelOfTutorial"].SetActive(true);

            tips[0].SetActive(true);

            _Player.CanTurn = true;
            _Player.IsMovingX = true;
            _Player.Speed = 25;

            Time.timeScale = 0;

            AudioManager.PlayAudio("InTutorial");

            IsTutorialStarted = true;
        }
    }

    public void Click()
    {
        AudioManager.PlayAudio("Click");
    }

    #region Pause Menu

    public void Pause()
    {
        AudioManager.ChangeAudioVolume("InTutorial", _UserInterface.sv.musicVolume / 3);
        AudioManager.PauseAudio("InTutorial");
        AudioManager.PlayAudio("InTutorial");

        Time.timeScale = 0;
        panelsDict["PanelPauseMenu"].SetActive(true);
    }

    public void Continue()
    {
        Time.timeScale = 1;
        panelsDict["PanelPauseMenu"].SetActive(false);

         AudioManager.ChangeAudioVolume("InTutorial", _UserInterface.sv.musicVolume);
    }

    public void ConfirmAction(string method)
    {
        panelsDict["PanelExit"].SetActive(true);

        switch (method)
        {
            case "Home":
                methodText.text = "RETURN TO HOME?";
                if (_UserInterface.sv.language == "Ru") methodText.text = "ВЕРНУТЬСЯ ДОМОЙ?";
                break;

            case "Restart":
                methodText.text = "RESTART TUTORIAL?";
                if (_UserInterface.sv.language == "Ru") methodText.text = "НАЧАТЬ ОБУЧЕНИЕ ЗАНОВО?";
                break;

            case "Skip":
                methodText.text = "SKIP TUTORIAL? \n <color=red>You will not get a reward</color>";
                if (_UserInterface.sv.language == "Ru") methodText.text = "ПРОПУСТИТЬ ОБУЧЕНИЕ? \n <color=red>Вы не получите награду</color>";
                break;

            default:
                throw new System.ArgumentException("Invalid argument ", method);
        }
    }
    
    public void NO()
    {
        panelsDict["PanelExit"].SetActive(false);
    }
    public void YES()
    {
        switch (methodText.text)
        {
            case "ВЕРНУТЬСЯ ДОМОЙ?":
            case "RETURN TO HOME?":

                Time.timeScale = 1;

                _UserInterface.sv.Save();

                LoadingScene.SceneName = "MainMenu";

                loading_object.SetActive(true);
                loadingScreen.SetActive(true);

                break;

            case "НАЧАТЬ ОБУЧЕНИЕ ЗАНОВО?":
            case "RESTART TUTORIAL?":

                Time.timeScale = 1;
                
                player_tr.gameObject.SetActive(false);

                _Player.Speed = 0;
                _Player.rb.position = new Vector3(-76f, 36.4f, -73.5f);
                _Player.rb.rotation = Quaternion.Euler(Vector3.zero);

                player_tr.Clear();

                AudioManager.ChangeAudioVolume("InTutorial", _UserInterface.sv.musicVolume);
                AudioManager.PlayAudio("InTutorial");

                panelsDict["PanelExit"].SetActive(false);
                panelsDict["PanelPauseMenu"].SetActive(false);
                panelsDict["PanelStartGame"].SetActive(true);
                
                for (int i = 0; i < tips.Length; i++)
                {
                    tips[i].SetActive(false);
                    startText.text = "Start tutorial";
                }

                IsTutorialStarted = false;

                break;

            case "ПРОПУСТИТЬ ОБУЧЕНИЕ? \n <color=red>Вы не получите награду</color>":
            case "SKIP LEVEL? \n <color=red>You will not get a reward</color>":

                _UserInterface.sv.IsTutorialPassed = true;

                _UserInterface.sv.Save();

                Time.timeScale = 1;

                LoadingScene.SceneName = "MainMenu";

                loading_object.SetActive(true);
                loadingScreen.SetActive(true);

                break;
        }
    }

    public void SliderSounds()
    {
        _UserInterface.sv.soundVolume = sl_Sound.value;

        AudioManager.ChangeVolumeAll(SOUND, sl_Sound.value / 3);

        fillSound.color = Color.HSVToRGB(sl_Sound.value / 3f, 1, 1);

        if (sl_Sound.value != 0)
        {
            buttonSound.GetComponent<Image>().sprite = sprites[1];
            _UserInterface.sv.colorbttnSound = new Color(0, 0.66f, 1);
            buttonSound.GetComponent<Image>().color = _UserInterface.sv.colorbttnSound;
        }
        else
        {
            buttonSound.GetComponent<Image>().sprite = sprites[0];
            _UserInterface.sv.colorbttnSound = Color.red;
            buttonSound.GetComponent<Image>().color = _UserInterface.sv.colorbttnSound;
        }
    }
    public void SliderMusic()
    {
        _UserInterface.sv.musicVolume = sl_Music.value;

        AudioManager.ChangeVolumeAll(MUSIC, sl_Music.value / 3);

        fillMusic.color = Color.HSVToRGB(sl_Music.value / 3f, 1, 1);

        if (sl_Music.value != 0)
        {
            buttonMusic.GetComponent<Image>().sprite = sprites[3];
            _UserInterface.sv.colorbttnMusic = new Color(0, 0.66f, 1);
            buttonMusic.GetComponent<Image>().color = _UserInterface.sv.colorbttnMusic;
        }
        else
        {
            buttonMusic.GetComponent<Image>().sprite = sprites[2];
            _UserInterface.sv.colorbttnMusic = Color.red;
            buttonMusic.GetComponent<Image>().color = _UserInterface.sv.colorbttnMusic;
        }
    }
    #endregion

    public void OnWarning()
    {
        panelsDict["PanelWarning"].SetActive(false);
        
        _Player.CanTurn = true;
        TouchController.Instance.OnPointerDown(null);
        _Player.Speed = -1;
    }

    public void Restart()
    {
        panelsDict["PanelLost"].SetActive(false);
        panelsDict["PanelStartGame"].SetActive(true);

        _Player.Speed = 0;
        _Player.CanTurn = true;
        _Player.IsMovingX = IsMovingX;

        player_tr.Clear();

        player_tr.gameObject.SetActive(false);
        
        _Player.rb.position = TutorialCheckPoints.checkPos;
        _Player.rb.rotation = Quaternion.Euler(Vector3.zero);
        
        AudioManager.ChangeAudioVolume("InTutorial", _UserInterface.sv.musicVolume);
        AudioManager.PlayAudio("InTutorial");
    }

    public void Transition(bool enabled)
    {
        panelsDict["PanelTransition"].SetActive(enabled);

        panelsDict["PanelOfTutorial"].SetActive(enabled);

        tips[0].transform.parent.gameObject.SetActive(false);
    }

    public void StartGameAgain()
    {
        player_tr.gameObject.SetActive(true);
        panelsDict["PanelStartGame"].SetActive(false);
        
        _Player.Speed = 25;
    }

    public void StartGame()
    {
        startText.text = "Continue";
        if (_UserInterface.sv.language == "Ru") startText.text = "Продолжить";
        
        panelsDict["PanelOfTutorial"].SetActive(false);
        
        Time.timeScale = 1;

        _Player.CanTurn = true;

        player_tr.gameObject.SetActive(true);

        AudioManager.ChangeAudioVolume("InTutorial", _UserInterface.sv.musicVolume);
    }
    public void StartGame(bool enabled)
    {
        panelsDict["PanelTransition"].SetActive(enabled);

        panelsDict["PanelOfTutorial"].SetActive(enabled);

        tips[0].transform.parent.gameObject.SetActive(!enabled);

        _Player.Speed = 25;
    }
}
