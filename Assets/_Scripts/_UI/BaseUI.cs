using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseUI : MonoBehaviour
{
    #region Variables
    private const string MUSIC = "Music", SOUND = "Sound";
    [HideInInspector]
    public string[] inGameMusic;
    [HideInInspector]
    public int index;
    public static BaseUI Instance;
    public static int costOfContinues, costModifier;
    public static Text coins, emeralds;
    public Slider sl_Sound, sl_Music;
    public Sprite[] sprites = new Sprite[4];
    public Image fillSound, fillMusic, sound, music;
    public Button jump;
    public Text textCoin, textFps, textEmerald, textScore, coins_CollectedText, emeralds_CollectedText, totalDistanceText;
    public Text[] allTexts;
    public Animation gameNotify;
    public GameObject loadingScreen, loading_object, textContinue, armor;
    public GameObject[] panels;
    private Dictionary<string, GameObject> panelsDict = new Dictionary<string, GameObject>();
    private Dictionary<string, Text> texts = new Dictionary<string, Text>();
    #endregion

    private void Awake()
    {
        NotifyManager.ClearNotify();

        if (Instance == null) Instance = this;
        
        EventAggregator.coinEvents = new EventManager<Coins>();
        EventAggregator.emeraldEvents = new EventManager<Emeralds>();
        EventAggregator.loseEvents = new EventManager<_Player>();

        loadingScreen.SetActive(false);
        loading_object.SetActive(false);

        _Player.modifier = 4;

        inGameMusic = new string[2];

        inGameMusic[0] = "InGame";
        inGameMusic[1] = "InGame2";

        index = Random.Range(0, 2);
    }

    private void Start()
    {
        EventAggregator.coinEvents.Subscribe(OnCoin);
        EventAggregator.emeraldEvents.Subscribe(OnEmerald);

        costOfContinues = 15;
        costModifier = 2;

        textCoin.text = _UserInterface.sv.coin.ToString();
        textEmerald.text = _UserInterface.sv.emerald.ToString();
        
        loading_object.GetComponentInChildren<Renderer>().material.color = _UserInterface.sv.colorFigure;

        if (_UserInterface.sv.boost["Jump"].amount > 0) jump.interactable = false;
        else jump.transform.parent.gameObject.SetActive(false);

        #region Add dictionaries
        for (int i = 0; i < panels.Length; i++) panelsDict.Add(panels[i].name, panels[i]);
        for (int i = 1; i < panels.Length; i++) panels[i].SetActive(true);

        GameObject[] temp = GameObject.FindGameObjectsWithTag("OtherText");
        for (int i = 0; i < temp.Length; i++) texts.Add(temp[i].name, temp[i].GetComponent<Text>());

        for (int i = 1; i < panels.Length; i++) panels[i].SetActive(false);
        panelsDict["PanelStartGame"].SetActive(true);
        #endregion

        #region Audio
        if (_UserInterface.sv.soundVolume <= 0) sound.sprite = sprites[0];
        else sound.sprite = sprites[1];
        sound.color = _UserInterface.sv.colorbttnSound;

        if (_UserInterface.sv.musicVolume <= 0) music.sprite = sprites[2];
        else music.sprite = sprites[3];
        music.color = _UserInterface.sv.colorbttnMusic;

        fillSound.color = Color.HSVToRGB(sl_Sound.value / 3f, 1, 1);
        fillMusic.color = Color.HSVToRGB(sl_Music.value / 3f, 1, 1);

        sl_Music.value = _UserInterface.sv.musicVolume;
        sl_Sound.value = _UserInterface.sv.soundVolume;

        AudioManager.StopAllAudio();
        AudioManager.ChangeVolumeAll(MUSIC, _UserInterface.sv.musicVolume);
        AudioManager.ChangeVolumeAll(SOUND, _UserInterface.sv.soundVolume);
        #endregion
        
        LanguageManager.ChangeLanguage();
        for (int i = 0; i < allTexts.Length; i++) allTexts[i].text = LanguageManager.GetLocalizedValue(allTexts[i].name);

        StartCoroutine(Fps());
    }

    private void Update()
    {
        textScore.text = _Player.Score;
    }

    private System.Collections.IEnumerator Fps()
    {
        while (true)
        {
            textFps.text = ((int)(1 / Time.deltaTime)).ToString();

            yield return new WaitForSeconds(0.5f);
        }
    }
    
    private void OnApplicationQuit()
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
        _Player.emeralds_count++;

        AudioManager.PlayAudio("EmeraldPickUp");

        textEmerald.text = _UserInterface.sv.emerald.ToString();
    }

    public void OnArmor(bool enabled)
    {
        armor.SetActive(enabled);
    }
    public void OnUseArmor()
    {
        _Player.CanTurn = true;
        TouchController.Instance.OnPointerDown(null);

        panelsDict["PanelStartGame"].SetActive(true);
        textContinue.SetActive(true);

        OnArmor(false);
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

    public void Click()
    {
        AudioManager.PlayAudio("Click");
    }

    public GameObject GetPanel(string name)
    {
        if (panelsDict.ContainsKey(name)) return panelsDict[name];
        else throw new KeyNotFoundException("Key not found. Key: " + name);
    }
    public void SetPanels(bool enabled)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(enabled);
        }
    }

    #region PauseMenu
    
    public void PauseMenu()
    {
        AudioManager.ChangeAudioVolume(inGameMusic[index], _UserInterface.sv.musicVolume / 3);
        AudioManager.PauseAudio(inGameMusic[index]);
        AudioManager.PlayAudio(inGameMusic[index]);

        Time.timeScale = 0;

        panelsDict["PanelInGame"].SetActive(false);
        panelsDict["PanelPauseMenu"].SetActive(true);
    }

    public void ContinueGame()
    {
        Time.timeScale = 1;

        panelsDict["PanelInGame"].SetActive(true);
        panelsDict["PanelPauseMenu"].SetActive(false);

        AudioManager.ChangeAudioVolume(inGameMusic[index], _UserInterface.sv.musicVolume);
    }

    public void SliderSounds()
    {
        _UserInterface.sv.soundVolume = sl_Sound.value;

        AudioManager.ChangeVolumeAll(SOUND, sl_Sound.value / 3);

        fillSound.color = Color.HSVToRGB(sl_Sound.value / 3f, 1, 1);

        if (sl_Sound.value != 0)
        {
            sound.sprite = sprites[1];
            _UserInterface.sv.colorbttnSound = new Color(0, 0.66f, 1);
            sound.color = _UserInterface.sv.colorbttnSound;
        }
        else
        {
            sound.sprite = sprites[0];
            _UserInterface.sv.colorbttnSound = Color.red;
            sound.color = _UserInterface.sv.colorbttnSound;
        }
    }
    public void SliderMusic()
    {
        _UserInterface.sv.musicVolume = sl_Music.value;

        AudioManager.ChangeVolumeAll(MUSIC, sl_Music.value / 3);

        fillMusic.color = Color.HSVToRGB(sl_Music.value / 3f, 1, 1);

        if (sl_Music.value != 0)
        {
            music.sprite = sprites[3];
            _UserInterface.sv.colorbttnMusic = new Color(0, 0.66f, 1);
            music.color = _UserInterface.sv.colorbttnMusic;
        }
        else
        {
            music.sprite = sprites[2];
            _UserInterface.sv.colorbttnMusic = Color.red;
            music.color = _UserInterface.sv.colorbttnMusic;
        }
    }

    #endregion

    #region LostMenu

    private void PanelOfLost()
    {
        _UserInterface.sv.totalDistance += _Player.totalDistance;

        panelsDict["PanelLost"].SetActive(true);
        coins_CollectedText.text = LanguageManager.GetLocalizedValue("Collected") + _Player.coins_count.ToString();
        emeralds_CollectedText.text = LanguageManager.GetLocalizedValue("Collected") + _Player.emeralds_count.ToString();
        totalDistanceText.text = LanguageManager.GetLocalizedValue("TotalDistance") + (int)_Player.totalDistance + LanguageManager.GetLocalizedValue("m");
        
        AchievementsManager.IncrementAchievement("YoungCollector", _Player.coins_count);
        GPGamesManager.IncrementAchievement(GPGSIds.achievement_young_collector, _Player.coins_count);

        AchievementsManager.IncrementAchievement("StartingSmall", _Player.ScoreInt);
        GPGamesManager.IncrementAchievement(GPGSIds.achievement_starting_small, _Player.ScoreInt);
    }
    
    #endregion

    public void StartGame()
    {
        textContinue.SetActive(false);

        panelsDict["PanelStartGame"].SetActive(false);
        panelsDict["PanelInGame"].SetActive(true);

        Time.timeScale = 1f;

        _Player.Instance.player_Trail.gameObject.SetActive(true);

        _Player.CanTurn = true;

        _Player.HasLose = false;

        AudioManager.PlayAudio(inGameMusic[index]);
    }

    public void OnUnlockAchievement(string text, NotifyType type, float showingTime = 3) => NotifyManager.ShowNotify(gameNotify, text, type, showingTime);
}
