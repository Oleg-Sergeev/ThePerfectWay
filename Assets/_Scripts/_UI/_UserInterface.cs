using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class _UserInterface : MonoBehaviour
{
    #region Variables
    public const string MUSIC = "Music", SOUND = "Sound";
    private const int CHANGEMODIFIER = 15, AFK_LIMIT = 45;
    private static int[] ticketsCost = new int[] { 100, 500, 1500 };
    private static string index = "Arcade";
    public static string[] bonusCodes = new string[] { "SUPERTESTER", "BETATESTER", "EPICJUMP", "РУССКИЙЯЗЫК" };
    public static readonly int levelCount = 5;
    public static bool IsArcade, singleton;
    public static _UserInterface Instance;
    public static SaveManager sv = new SaveManager();
    private bool IsAFK;
    private int IF_Emeralds_int, IF_Coins_int, perfectLevel, numberBonusCode;
    private int[] chances = { 47, 15, 10, 8, 20 };
    private float last_touch = 0;
    private string favoriteMode;
    private string[] names;
    private TimeSpan time, allTime;
    private Color lastColor_cube, lastColor_trail;
    private Range[][] ranges;
    public Animation gameNotify;
    public ColorPicker picker;
    public TrailRenderer bot_Trail, custom_Trail;
    public RewardList[] rewards;
    public Slider sl_Sound, sl_Music;
    public Image trailIcon, figureIcon, lastButton_Profile, lastButton_Shop, fillSound, fillMusic, dailyGift;
    public Image[] curtains, pageMarkers;
    public Sprite[] sprites;
    public Renderer[] figures_Colors;
    public Button[] dailyGifts;
    public InputField Emerald_IF, Coin_IF, BonusCode_IF;
    public Text qualityText, notifyText, bestScoreText, bestScore_HardcoreText, debugText, buyText, gameModeText, versionText, rewardText, takeGiftText, giftText;
    public Text[] coinsText, emeralds, allTexts;
    public GameObject flag, sound, music, cube_camera, trail_camera, giftInfo, go,
        loading_object, loadingScreen, offers, bundles, bttnOffers, bttnBundles, lastPanel_Profile, lastPanel_Shop, lockImage, ticketParent, curtain;
    public GameObject[] notify, panels, Pages, powerUps, tickets;
    public Transform underline_Profile, underline_Shop, allRewards;
    public readonly Dictionary<string, GameObject> panelsDict = new Dictionary<string, GameObject>();
    public readonly Dictionary<string, Text> texts = new Dictionary<string, Text>();
    private Dictionary<string, GameObject> powerUp = new Dictionary<string, GameObject>();
    private Dictionary<string, Range[]> rewardRange = new Dictionary<string, Range[]>();
    private Dictionary<string, Image> dailyImage = new Dictionary<string, Image>();
    #endregion

    private void Awake()
    {
        NotifyManager.ClearNotify();

        if (Instance == null) Instance = this;

        #region Add dictionaries
        for (int i = 0; i < panels.Length; i++) panelsDict.Add(panels[i].name, panels[i]);
        for (int i = 1; i < panels.Length; i++) panels[i].SetActive(true);
        for (int i = 0; i < powerUps.Length; i++) powerUp.Add(powerUps[i].transform.name, powerUps[i]);
        for (int i = 0; i < dailyGifts.Length; i++)
        {
            if (!dailyImage.ContainsKey(dailyGifts[i].GetComponent<Gift>().names.ToString()))
            {
                dailyImage.Add(dailyGifts[i].GetComponent<Gift>().names.ToString(), dailyGifts[i].transform.GetChild(0).GetComponent<Image>());
            }
        }

        GameObject[] tempGO = GameObject.FindGameObjectsWithTag("TextStats");
        for (int i = 0; i < tempGO.Length; i++) texts.Add(tempGO[i].name, tempGO[i].GetComponent<Text>());
        tempGO = GameObject.FindGameObjectsWithTag("OtherText");
        for (int i = 0; i < tempGO.Length; i++) texts.Add(tempGO[i].name, tempGO[i].GetComponent<Text>());

        for (int i = 1; i < panels.Length; i++) panels[i].SetActive(false);
        #endregion

        loadingScreen.SetActive(false);
        loading_object.SetActive(false);

        sv.Load(sv);

        sv.levelsCount = levelCount;

        if (sv.levels.Length < levelCount)
        {
            Debug.Log(levelCount - sv.levels.Length + " levels have been added");
            var temp = sv.levels;
            sv.levels = new Levels[levelCount];
            for (int i = 0; i < temp.Length; i++) sv.levels[i] = temp[i];
            for (int i = temp.Length; i < sv.levels.Length; i++) sv.levels[i].stars = new int[3];
        }
        else if (sv.levels.Length > levelCount)
        {
            Debug.Log(sv.levels.Length - levelCount + " levels have been removed");
            var temp = sv.levels;
            sv.levels = new Levels[levelCount];
            for (int i = 0; i < levelCount; i++) sv.levels[i] = temp[i];
        }
    }

    private void Start()
    {
        GPGamesManager.Initialize(true);
        GPGamesManager.Auth((success, message) =>
        {
            if (success)
            {
                Debug.Log("Good " + message);
                NotifyManager.ShowNotify(gameNotify, "Authorized", NotifyType.Success, 2);
            }
            else
            {
                Debug.LogError("Bad " + message);
                NotifyManager.ShowNotify(gameNotify, $"Not authorized: {message}", NotifyType.Error, 4);
            }
        });

        EventAggregator.coinEvents = null;
        EventAggregator.emeraldEvents = null;

        Application.targetFrameRate = 60;

        Time.timeScale = 1;

        IsAFK = false;

        cube_camera.SetActive(false);
        trail_camera.SetActive(false);

        offers.SetActive(true);
        bundles.SetActive(false);

        allRewards.gameObject.SetActive(false);

        rewardText.text = "";

        gameModeText.text = sv.gameMode;

        bestScoreText.text = sv.BestScore.ToString();

        bestScore_HardcoreText.text = sv.BestScore_Hardcore.ToString();

        versionText.text = Application.version;

        InvokeRepeating("CheckAFK", AFK_LIMIT - 2, 5);

        InvokeRepeating("Time_", 1, 1);

        InvokeRepeating("CloudSave", 10, 60);

        for (int i = 1; i < Pages.Length; i++) Pages[i].SetActive(false);
        Pages[0].SetActive(true);

        for (int i = 0; i < coinsText.Length; i++)
        {
            coinsText[i].text = sv.coin.ToString();
            emeralds[i].text = sv.emerald.ToString();
        }

        for (int i = 0; i < notify.Length; i++)
        {
            notify[i].SetActive(false);
        }

        tickets = new GameObject[ticketParent.transform.childCount - 1];
        for (int i = 0; i < ticketParent.transform.childCount - 1; i++)
        {
            tickets[i] = ticketParent.transform.GetChild(i).GetChild(0).gameObject;
        }

        #region Audio
        if (sv.soundVolume <= 0) sound.GetComponent<Image>().sprite = sprites[0];
        else sound.GetComponent<Image>().sprite = sprites[1];

        if (sv.musicVolume <= 0) music.GetComponent<Image>().sprite = sprites[2];
        else music.GetComponent<Image>().sprite = sprites[3];

        music.GetComponent<Image>().color = sv.colorbttnMusic;
        sound.GetComponent<Image>().color = sv.colorbttnSound;

        fillSound.color = Color.HSVToRGB(sl_Sound.value / 3f, 1, 1);
        fillMusic.color = Color.HSVToRGB(sl_Music.value / 3f, 1, 1);

        sl_Music.value = sv.musicVolume;
        sl_Sound.value = sv.soundVolume;


        AudioManager.GetAudio("InGame").Stop();
        AudioManager.GetAudio("InTutorial").Stop();
        AudioManager.PlayAudio("InMainMenu");

        AudioManager.ChangeVolumeAll(MUSIC, sv.musicVolume);
        AudioManager.ChangeVolumeAll(SOUND, sv.soundVolume);
        #endregion

        #region Figure
        lastColor_cube = sv.colorFigure;
        lastColor_trail = sv.colorTrail;

        figureIcon.color = sv.colorFigure;
        trailIcon.color = sv.colorTrail;

        bot_Trail.material.color = sv.colorTrail;
        custom_Trail.material.color = sv.colorTrail;

        foreach (Renderer color in figures_Colors)
        {
            color.material.color = sv.colorFigure;
        }
        #endregion

        #region Language
        switch (sv.language)
        {
            case "Ru":
                flag.GetComponent<Image>().sprite = sprites[4];
                break;

            case "En":
                flag.GetComponent<Image>().sprite = sprites[5];
                break;

            default:
                flag.GetComponent<Image>().sprite = sprites[4];
                sv.language = "Ru";
                break;
        }

        LanguageManager.ChangeLanguage();
        for (int i = 0; i < allTexts.Length; i++) allTexts[i].text = LanguageManager.GetLocalizedValue(allTexts[i].name);

        for (int i = 0; i < sv.achievements.Length; i++)
        {
            panelsDict["PanelAchievements"].transform.GetChild(i + 1).GetChild(1).GetComponent<Text>().text = LanguageManager.GetLocalizedValue(panelsDict["PanelAchievements"].transform.GetChild(i + 1).name);
        }

        for (int i = 0; i < powerUps.Length; i++)
        {
            powerUps[i].transform.GetChild(1).GetChild(0).GetComponent<Text>().text = LanguageManager.GetLocalizedValue(powerUps[i].transform.GetChild(1).GetChild(0).name);
            powerUps[i].transform.GetChild(4).GetChild(0).GetComponent<Text>().text = LanguageManager.GetLocalizedValue(powerUps[i].transform.GetChild(4).GetChild(0).name);
            powerUps[i].transform.GetChild(2).GetChild(0).GetComponent<Text>().text = LanguageManager.GetLocalizedValue(powerUps[i].transform.GetChild(2).GetChild(0).name);
            powerUps[i].transform.GetChild(3).GetChild(0).GetComponent<Text>().text = LanguageManager.GetLocalizedValue(powerUps[i].transform.GetChild(3).GetChild(0).name);
        }

        for (int i = 0; i < tickets.Length; i++)
        {
            tickets[i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = LanguageManager.GetLocalizedValue(tickets[i].transform.GetChild(0).GetChild(0).name);
            tickets[i].transform.GetChild(2).GetChild(1).GetComponent<Text>().text = LanguageManager.GetLocalizedValue(tickets[i].transform.GetChild(2).GetChild(1).name);
            tickets[i].transform.parent.GetChild(1).GetChild(0).GetComponent<Text>().text = LanguageManager.GetLocalizedValue(tickets[i].transform.parent.GetChild(1).GetChild(0).name);
        }

        gameModeText.text = LanguageManager.GetLocalizedValue(index);

        notifyText.text = LanguageManager.GetLocalizedValue(sv.notify);
        qualityText.text = LanguageManager.GetLocalizedValue(sv.quality);
        #endregion

        #region textStats

        favoriteMode = FavoriteMode();

        allTime = TimeSpan.FromSeconds(sv.time["menu"] + sv.time["adventure"] + sv.time["arcade"] + sv.time["hardcore"]);

        if (sv.levels != null)
        {
            for (int i = 0; i < sv.levels.Length; i++)
            {
                if (sv.levels[i].stars[0] == 1 && sv.levels[i].stars[1] == 1 && sv.levels[i].stars[2] == 1) perfectLevel++;
            }
        }
        TextStats();
        #endregion

        ChangePowerUp();

        ChangeTickets();

        for (int i = 0; i < sv.achievements.Length; i++)
        {
            panelsDict["PanelAchievements"].transform.GetChild(i + 1).GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = (sv.achievements[i].currentProgress - sv.achievements[i].startProgress) / (sv.achievements[i].endProgress - sv.achievements[i].startProgress);

            if (sv.achievements[i].currentProgress >= sv.achievements[i].endProgress)
            {
                Button button = panelsDict["PanelAchievements"].transform.GetChild(i + 1).GetChild(3).GetComponent<Button>();

                panelsDict["PanelAchievements"].transform.GetChild(i + 1).GetChild(1).GetComponent<Text>().color = Color.green;
                button.interactable = true;

                if (sv.achievements[i].IsUnlocked)
                {
                    GetReward(panelsDict["PanelAchievements"].transform.GetChild(i + 1));

                    panelsDict["PanelAchievements"].transform.GetChild(i + 1).GetChild(1).GetComponent<Text>().color = Color.green;

                    continue;
                }

                notify[0].SetActive(true);
                notify[1].SetActive(true);

                button.animator.enabled = true;
                button.animator.Play("CompletedAchievement");
            }
        }

        names = new string[] { "Coins", "Emeralds", sv.tickets[0].name, sv.tickets[1].name, sv.boosts[0].name };

        ranges = new Range[names.Length][];
        ranges[0] = new Range[] { new Range(400, 801, 40), new Range(801, 1401, 40), new Range(1401, 2001, 15), new Range(2001, 3001, 5) };
        ranges[1] = new Range[] { new Range(20, 101, 70), new Range(101, 151, 25), new Range(151, 201, 5) };
        ranges[2] = new Range[] { new Range(1, 1, 75), new Range(2, 4, 20) };
        ranges[3] = new Range[] { new Range(1, 1, 100) };
        ranges[4] = new Range[] { new Range(1, 1, 75), new Range(2, 5, 25) };

        for (int i = 0; i < names.Length; i++) rewardRange.Add(names[i], ranges[i]);

        for (int i = 0; i < rewards.Length; i++)
        {
            rewards[i].GetComponent<Button>().interactable = true;
            rewards[i].rewards = new List<Reward>(UnityEngine.Random.Range(1, 5));
            for (int j = 0; j < rewards[i].rewards.Capacity; j++)
            {
                rewards[i].rewards.Add(new Reward());
                rewards[i].rewards[j].name = GetRandomName(names);
                rewards[i].rewards[j].amount = GetRandomInt(rewardRange[rewards[i].rewards[j].name]);
            }
        }

        TimeSpan lastSession = DateTime.Now - sv.dateTime;

        if (lastSession.Days >= 1)
        {
            panelsDict["PanelDailyGift"].SetActive(true);

            if (lastSession.Days > 1) sv.giftDays = 1;

            for (int i = 0; i < sv.giftDays; i++) dailyGifts[i].interactable = true;

            dailyGifts[sv.giftDays - 1].animator.enabled = true;
            dailyGifts[sv.giftDays - 1].animator.Play(0);
        }
    }

    struct Range
    {
        public int min, max, chance;

        public Range(int min, int max, int chance)
        {
            this.min = min;
            this.max = max;
            this.chance = chance;
        }
    }

    private string GetRandomName(string[] names)
    {
        int i, chanceSum = 0;
        for (i = 0; i < chances.Length; i++) chanceSum += chances[i];
        int randomChance = UnityEngine.Random.Range(0, chanceSum);
        for (i = 0; i < chances.Length; i++)
        {
            randomChance -= chances[i];
            if (randomChance <= 0) break;
        }
        return names[i];
    }
    private int GetRandomInt(Range[] ranges)
    {
        int chanceSum = 0, i;

        for (i = 0; i < ranges.Length; i++) chanceSum += ranges[i].chance;

        int randomChance = UnityEngine.Random.Range(0, chanceSum);

        for (i = 0; i < ranges.Length; i++)
        {
            randomChance -= ranges[i].chance;
            if (randomChance <= 0) break;
        }

        return UnityEngine.Random.Range(ranges[i].min, ranges[i].max);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            last_touch = Time.timeSinceLevelLoad;

            if (IsAFK) CheckAFK();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        sv.Save();
    }

    private void OnApplicationFocus(bool focus)
    {
        sv.gameMode = "ARCADE";

        sv.Save();
    }

    private void OnApplicationQuit()
    {
        sv.gameMode = "ARCADE";

        sv.Save();
    }

    private void ChangeTickets()
    {
        for (int i = 0; i < tickets.Length; i++)
        {
            tickets[i].transform.GetChild(1).GetChild(0).GetComponent<Text>().text = sv.tickets[i].amount.ToString() + "x";
            tickets[i].transform.GetChild(2).GetChild(0).GetComponent<Text>().text = ticketsCost[i].ToString();

            sv.tickets[i].name = tickets[i].transform.GetChild(0).GetChild(0).name;
            sv.tickets[i].rarity = i;
            sv.tickets[i].cost = ticketsCost[i];

            sv.ticket.Add(sv.tickets[i].name, sv.tickets[i]);
        }
    }

    private void ChangePowerUp()
    {
        for (int i = 0; i < powerUps.Length; i++)
        {
            string name = powerUps[i].transform.GetChild(1).GetChild(0).name;

            if (sv.boost[name].upgrade == -1)
            {
                powerUps[i].transform.GetChild(3).GetChild(1).GetComponent<Text>().text = sv.boost[name].amount.ToString();

                sv.boost[name].cost = int.Parse(powerUps[i].transform.GetChild(4).GetChild(1).GetComponent<Text>().text);
            }
            else if (sv.boost[name].amount == -1)
            {
                powerUps[i].transform.GetChild(4).GetChild(1).GetComponent<Text>().text = sv.boost[name].cost.ToString();

                for (int x = 0; x < 6; x++) powerUps[i].transform.GetChild(3).GetChild(x + 1).GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);

                for (int x = 0; x < sv.boost[name].upgrade; x++) powerUps[i].transform.GetChild(3).GetChild(x + 1).GetComponent<Image>().color = new Color(1, 0.85f, 0);

                if (sv.boost[powerUps[i].name].upgrade == 6)
                {
                    powerUp[name].transform.GetChild(4).GetComponent<Button>().interactable = false;

                    powerUp[name].transform.GetChild(4).GetChild(0).GetComponent<Text>().text = sv.language == "Ru" ? "Максимум" : "Maximum";
                    powerUp[name].transform.GetChild(4).GetChild(1).GetComponent<Text>().text = "";
                    powerUp[name].transform.GetChild(4).GetChild(2).gameObject.SetActive(false);
                }
            }
            else Debug.LogError("Error");
        }
    }
    private void ChangePowerUpAmount(string name)
    {
        powerUp[name].transform.GetChild(3).GetChild(1).GetComponent<Text>().text = sv.boost[name].amount.ToString();
    }
    private void ChangePowerUpUpgrade(string name)
    {
        for (int i = 0; i < sv.boost[name].upgrade; i++) powerUp[name].transform.GetChild(3).GetChild(i + 1).GetComponent<Image>().color = new Color(1, 0.85f, 0);

        powerUp[name].transform.GetChild(4).GetChild(1).GetComponent<Text>().text = sv.boost[name].cost.ToString();

        if (sv.boost[name].upgrade == 6)
        {
            powerUp[name].transform.GetChild(4).GetComponent<Button>().interactable = false;

            powerUp[name].transform.GetChild(4).GetChild(0).GetComponent<Text>().text = sv.language == "Ru" ? "Максимум" : "Maximum";
            powerUp[name].transform.GetChild(4).GetChild(1).GetComponent<Text>().text = "";
            powerUp[name].transform.GetChild(4).GetChild(2).gameObject.SetActive(false);
        }
    }

    private void OnChangeMoney()
    {
        for (int i = 0; i < coinsText.Length; i++) coinsText[i].text = sv.coin.ToString();
        for (int i = 0; i < emeralds.Length; i++) emeralds[i].text = sv.emerald.ToString();
    }
    private void OnChangeTickets()
    {
        for (int i = 0; i < tickets.Length; i++)
        {
            tickets[i].transform.GetChild(1).GetChild(0).GetComponent<Text>().text = sv.tickets[i].amount.ToString() + "x";
        }
    }
    private void OnChangeBoosts()
    {
        for (int i = 0; i < powerUps.Length; i++)
        {
            if (sv.boost[powerUps[i].name].amount != -1) powerUp[powerUps[i].name].transform.GetChild(3).GetChild(1).GetComponent<Text>().text = sv.boost[powerUps[i].name].amount.ToString();
        }
    }

    private void CloudSave() => sv.Save(true);

    private void Time_()
    {
        sv.time["lastMenu"] = (int)Time.timeSinceLevelLoad;
    }

    private void CheckAFK()
    {
        if ((Time.timeSinceLevelLoad - last_touch) > AFK_LIMIT)
        {
            IsAFK = true;

            cube_camera.SetActive(false);
            trail_camera.SetActive(false);
            panelsDict["PanelMainMenu"].SetActive(false);
        }
        else if (IsAFK)
        {
            IsAFK = false;

            panelsDict["PanelMainMenu"].SetActive(true);

            if (panelsDict["PanelCustom"].activeSelf)
            {
                if (texts["Trail"].gameObject.activeSelf) trail_camera.SetActive(true);
                else cube_camera.SetActive(true);
            }
        }
    }

    private void TextStats()
    {
        texts["BestArcade"].text += sv.BestScore;
        texts["BestHardcore"].text += sv.BestScore_Hardcore;
        texts["LevelsCompleted"].text += sv.completedLevel + "/" + sv.levelsCount;
        texts["PerfectLevels"].text += perfectLevel + "/" + sv.levelsCount;
        texts["TimeArcade"].text += TimeSpan.FromSeconds(sv.time["arcade"]).Hours + LanguageManager.GetLocalizedValue("h") + " " + TimeSpan.FromSeconds(sv.time["arcade"]).Minutes + LanguageManager.GetLocalizedValue("m");
        texts["TimeHardcore"].text += TimeSpan.FromSeconds(sv.time["hardcore"]).Hours + LanguageManager.GetLocalizedValue("h") + " " + TimeSpan.FromSeconds(sv.time["hardcore"]).Minutes + LanguageManager.GetLocalizedValue("m");
        texts["TimeAdventure"].text += TimeSpan.FromSeconds(sv.time["adventure"]).Hours + LanguageManager.GetLocalizedValue("h") + " " + TimeSpan.FromSeconds(sv.time["adventure"]).Minutes + LanguageManager.GetLocalizedValue("m");
        texts["AllTime"].text += allTime.Hours + LanguageManager.GetLocalizedValue("h") + " " + allTime.Minutes + LanguageManager.GetLocalizedValue("m");
        texts["CoinsCollected"].text += sv.coinsCollected;
        texts["EmeraldsCollected"].text += sv.emeraldsCollected;
        texts["SkinsUnlocked"].text += "0/0";
        texts["BackgroundsUnlocked"].text += "0/0";
        texts["FavoriteMode"].text += favoriteMode;
        texts["TotalDistance"].text += (int)sv.totalDistance + LanguageManager.GetLocalizedValue("m");
    }

    private string FavoriteMode()
    {
        if (sv.time["adventure"] > sv.time["hardcore"] && sv.time["adventure"] > sv.time["arcade"]) return LanguageManager.GetLocalizedValue("Adventure");
        else if (sv.time["hardcore"] > sv.time["arcade"]) return LanguageManager.GetLocalizedValue("Hardcore");
        else return LanguageManager.GetLocalizedValue("Arcade");
    }

    public void DailyGift(GameObject dailyGift)
    {
        if (dailyGift.GetComponent<Animator>().enabled == false) return;

        Gift gift = dailyGift.GetComponent<Gift>();

        switch (gift.names)
        {
            case Name.Coins:
                sv.coin += gift.amount;
                break;

            case Name.Emeralds:
                sv.emerald += gift.amount;
                break;

            case Name.Jump:
                sv.boost[Name.Jump.ToString()].amount += gift.amount;
                break;

            case Name.CommonTicket:
                sv.ticket["CommonTicket"].amount += gift.amount;
                break;

            case Name.GoldTicket:
                sv.ticket["GoldTicket"].amount += gift.amount;
                break;

            case Name.PerfectTicket:
                sv.ticket["PerfectTicket"].amount += gift.amount;
                break;
        }

        takeGiftText.text = "Ваш подарок:";
        giftText.text = dailyGift.GetComponent<Gift>().amount.ToString();

        this.dailyGift.sprite = dailyImage[gift.names.ToString()].sprite;
        this.dailyGift.color = dailyImage[gift.names.ToString()].color;

        go.SetActive(true);
    }
    public void TakeDailyGift()
    {
        OnChangeMoney();
        OnChangeTickets();
        OnChangeBoosts();

        sv.giftDays %= 15;
        sv.giftDays++;

        sv.dateTime = DateTime.Now;

        panelsDict["PanelDailyGift"].SetActive(false);
    }

    public void Click() => AudioManager.PlayAudio("Click");

    public static T GetDictionaryValue<T>(Dictionary<string, T> arr, string key)
    {
        if (arr.ContainsKey(key))
        {
            return arr[key];
        }
        else
        {
            Debug.LogError("Key not found. Key: " + key);
            throw new KeyNotFoundException("Key not found. Key: " + key);
        }
    }

    #region Main Menu

    public void Back()
    {
        for (int i = 1; i < panels.Length; i++)
        {
            if (panels[i].activeSelf)
            {
                panels[i].SetActive(false);
                if (!panelsDict["PanelCustom"].activeSelf)
                {
                    cube_camera.SetActive(false);
                    trail_camera.SetActive(false);
                }
                break;
            }

        }
    }

    public void ChangeLanguage()
    {
        if (sv.language == "En")
        {
            flag.GetComponent<Image>().sprite = sprites[4];
            sv.language = "Ru";
        }
        else if (sv.language == "Ru")
        {
            flag.GetComponent<Image>().sprite = sprites[5];
            sv.language = "En";
        }

        LanguageManager.ChangeLanguage();

        favoriteMode = FavoriteMode();

        notifyText.text = LanguageManager.GetLocalizedValue(sv.notify);
        qualityText.text = LanguageManager.GetLocalizedValue(sv.quality);

        gameModeText.text = LanguageManager.GetLocalizedValue(index);

        for (int i = 0; i < allTexts.Length; i++)
        {
            allTexts[i].text = LanguageManager.GetLocalizedValue(allTexts[i].name);
        }

        for (int i = 0; i < sv.achievements.Length; i++)
        {
            panelsDict["PanelAchievements"].transform.GetChild(i + 1).GetChild(1).GetComponent<Text>().text = LanguageManager.GetLocalizedValue(panelsDict["PanelAchievements"].transform.GetChild(i + 1).name);
        }

        for (int i = 0; i < powerUps.Length; i++)
        {
            powerUps[i].transform.GetChild(1).GetChild(0).GetComponent<Text>().text = LanguageManager.GetLocalizedValue(powerUps[i].transform.GetChild(1).GetChild(0).name);
            powerUps[i].transform.GetChild(2).GetChild(0).GetComponent<Text>().text = LanguageManager.GetLocalizedValue(powerUps[i].transform.GetChild(2).GetChild(0).name);
            powerUps[i].transform.GetChild(3).GetChild(0).GetComponent<Text>().text = LanguageManager.GetLocalizedValue(powerUps[i].transform.GetChild(3).GetChild(0).name);
            powerUps[i].transform.GetChild(4).GetChild(0).GetComponent<Text>().text = LanguageManager.GetLocalizedValue(powerUps[i].transform.GetChild(4).GetChild(0).name);
        }

        for (int i = 0; i < tickets.Length; i++)
        {
            tickets[i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = LanguageManager.GetLocalizedValue(tickets[i].transform.GetChild(0).GetChild(0).name);
            tickets[i].transform.GetChild(2).GetChild(1).GetComponent<Text>().text = LanguageManager.GetLocalizedValue(tickets[i].transform.GetChild(2).GetChild(1).name);
            tickets[i].transform.parent.GetChild(1).GetChild(0).GetComponent<Text>().text = LanguageManager.GetLocalizedValue(tickets[i].transform.parent.GetChild(1).GetChild(0).name);
        }

        TextStats();
    }

    #region Help

    public void Help()
    {
        panelsDict["PanelHelp"].SetActive(true);
    }

    public void StartTutorial()
    {
        UI_Tutorial.IsPlayTutorial = true;

        LoadingScene.SceneName = "Tutorial";

        loading_object.SetActive(true);
        loadingScreen.SetActive(true);
    }

    Vector3 pos;
    public void OnBeginDrag()
    {
        pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }

    uint page = 0;
    public void OnDrag(Text pageNumber)
    {
        Vector3 pos1 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        if (pos1.normalized.x < pos.normalized.x)
        {
            if (page + 1 < Pages.Length)
            {
                pageMarkers[page].color = Color.black;

                Pages[page].SetActive(false);
                Pages[++page].SetActive(true);
                pageNumber.text = (page + 1).ToString() + "/" + Pages.Length.ToString();

                pageMarkers[page].color = Color.white;
            }
        }
        else
        {
            if (page > 0)
            {
                pageMarkers[page].color = Color.black;

                Pages[page].SetActive(false);
                Pages[--page].SetActive(true);
                pageNumber.text = (page + 1).ToString() + "/" + Pages.Length.ToString();

                pageMarkers[page].color = Color.white;
            }
        }
    }

    #endregion

    public void Play()
    {
        if (sv.IsTutorialPassed)
        {
            switch (sv.gameMode)
            {
                case "АРКАДА":
                case "ARCADE":
                    LoadingScene.SceneName = "Game_Arcade";
                    IsArcade = true;
                    break;

                case "ПРИКЛЮЧЕНИЕ":
                case "ADVENTURE":
                    LoadingScene.SceneName = "Game_Adventure";
                    IsArcade = true;
                    break;

                case "ХАРДКОР":
                case "HARDCORE":
                    LoadingScene.SceneName = "Game_Hardcore";
                    IsArcade = false;
                    break;

                default:
                    throw new ArgumentException("Invalid name of scene: ", sv.gameMode);
            }

            sv.time["lastMenu"] = (int)Time.timeSinceLevelLoad;

            sv.Save();

            loading_object.SetActive(true);
            loadingScreen.SetActive(true);
        }
        else
        {
            panelsDict["PanelTutorial"].SetActive(true);
        }
    }
    public void YESorNO(string answer)
    {
        if (answer == "YES")
        {
            panelsDict["PanelTutorial"].SetActive(false);
            LoadingScene.SceneName = "Tutorial";

            sv.time["lastMenu"] = (int)Time.timeSinceLevelLoad;

            sv.Save();

            loading_object.SetActive(true);
            loadingScreen.SetActive(true);
        }
        else
        {
            panelsDict["PanelTutorial"].SetActive(false);
            sv.IsTutorialPassed = true;
        }
    }

    public void PrevMode()
    {
        switch (sv.gameMode)
        {
            case "ХАРДКОР":
            case "HARDCORE":
                index = "Adventure";
                gameModeText.text = LanguageManager.GetLocalizedValue(index);
                sv.gameMode = gameModeText.text;

                bestScoreText.gameObject.SetActive(true);
                bestScore_HardcoreText.gameObject.SetActive(false);
                break;

            case "ПРИКЛЮЧЕНИЕ":
            case "ADVENTURE":
                index = "Arcade";
                gameModeText.text = LanguageManager.GetLocalizedValue(index);
                sv.gameMode = gameModeText.text;

                bestScoreText.gameObject.SetActive(true);
                bestScore_HardcoreText.gameObject.SetActive(false);
                break;

            case "АРКАДА":
            case "ARCADE":
                index = "Hardcore";
                gameModeText.text = LanguageManager.GetLocalizedValue(index);
                sv.gameMode = gameModeText.text;

                bestScoreText.gameObject.SetActive(false);
                bestScore_HardcoreText.gameObject.SetActive(true);
                break;

            default:
                index = "Arcade";
                gameModeText.text = LanguageManager.GetLocalizedValue(index);
                sv.gameMode = gameModeText.text;

                bestScoreText.gameObject.SetActive(true);
                bestScore_HardcoreText.gameObject.SetActive(false);
                break;
        }
    }

    public void NextMode()
    {
        switch (sv.gameMode)
        {
            case "АРКАДА":
            case "ARCADE":
                index = "Adventure";
                gameModeText.text = LanguageManager.GetLocalizedValue(index);
                sv.gameMode = gameModeText.text;

                bestScoreText.gameObject.SetActive(true);
                bestScore_HardcoreText.gameObject.SetActive(false);
                break;

            case "ПРИКЛЮЧЕНИЕ":
            case "ADVENTURE":
                index = "Hardcore";
                gameModeText.text = LanguageManager.GetLocalizedValue(index);
                sv.gameMode = gameModeText.text;

                bestScoreText.gameObject.SetActive(false);
                bestScore_HardcoreText.gameObject.SetActive(true);
                break;

            case "ХАРДКОР":
            case "HARDCORE":
                index = "Arcade";
                gameModeText.text = LanguageManager.GetLocalizedValue(index);
                sv.gameMode = gameModeText.text;

                bestScoreText.gameObject.SetActive(true);
                bestScore_HardcoreText.gameObject.SetActive(false);
                break;

            default:
                index = "Arcade";
                gameModeText.text = LanguageManager.GetLocalizedValue(index);
                sv.gameMode = gameModeText.text;

                bestScoreText.gameObject.SetActive(true);
                bestScore_HardcoreText.gameObject.SetActive(false);
                break;
        }
    }

    private void ReturnColor()
    {
        Coin_IF.GetComponentInChildren<Text>().color = Color.white;
        Emerald_IF.GetComponentInChildren<Text>().color = Color.white;
        buyText.color = Color.white;
        coinsText[1].color = Color.white;
        emeralds[1].color = Color.white;
        BonusCode_IF.textComponent.color = Color.black;
    }
    private IEnumerator ReturnColor(string name)
    {
        yield return new WaitForSeconds(0.5f);

        if (powerUp.ContainsKey(name)) powerUp[name].transform.GetChild(4).GetComponent<Image>().color = Color.green;
        if (sv.ticket.ContainsKey(name))
        {
            tickets[sv.ticket[name].rarity].transform.GetChild(2).GetComponent<Image>().color = new Color(0.9f, 0.6f, 1);
            tickets[sv.ticket[name].rarity].transform.parent.GetChild(1).GetComponent<Image>().color = new Color(0.9f, 0.6f, 1);
        }
    }

    public void Home()
    {
        panelsDict["PanelProfile"].SetActive(false);
        panelsDict["PanelShop"].SetActive(false);
        lastPanel_Profile.SetActive(false);
        lastPanel_Shop.SetActive(false);

        cube_camera.SetActive(false);
        trail_camera.SetActive(false);
    }

    #region Shop

    public void Shop()
    {
        ChangePowerUp();

        panelsDict["PanelShop"].SetActive(true);
        lastPanel_Shop.SetActive(true);

        if (lastPanel_Shop.name == "PanelCustom")
        {
            texts["Figure"].gameObject.SetActive(true);
            texts["Trail"].gameObject.SetActive(false);

            cube_camera.SetActive(true);
            trail_camera.SetActive(false);
        }
    }

    #region ShopMenu

    public void Store(Image button)
    {
        lastPanel_Shop.SetActive(false);
        panelsDict["PanelStore"].SetActive(true);
        lastPanel_Shop = panelsDict["PanelStore"];

        lastButton_Shop.color = Color.white;
        button.color = new Color(1, 0.5f, 0);
        lastButton_Shop = button;

        underline_Shop.localPosition = button.rectTransform.localPosition;

        cube_camera.SetActive(false);
        trail_camera.SetActive(false);
    }

    public void Custom(Image button)
    {
        lastPanel_Shop.SetActive(false);
        panelsDict["PanelCustom"].SetActive(true);
        lastPanel_Shop = panelsDict["PanelCustom"];

        lastButton_Shop.color = Color.white;
        button.color = new Color(1, 0.5f, 0);
        lastButton_Shop = button;

        underline_Shop.localPosition = button.rectTransform.localPosition;

        panelsDict["PanelCustom"].SetActive(true);

        texts["Figure"].gameObject.SetActive(true);
        texts["Trail"].gameObject.SetActive(false);

        cube_camera.SetActive(true);
        trail_camera.SetActive(false);

        if (!sv.IsPayed)
        {
            lockImage.SetActive(true);
            lockImage.transform.parent.GetComponent<Button>().interactable = false;
        }
    }

    public void Boosts(Image button)
    {
        lastPanel_Shop.SetActive(false);
        panelsDict["PanelBoosts"].SetActive(true);
        lastPanel_Shop = panelsDict["PanelBoosts"];

        lastButton_Shop.color = Color.white;
        button.color = new Color(1, 0.5f, 0);
        lastButton_Shop = button;

        underline_Shop.localPosition = button.rectTransform.localPosition;

        cube_camera.SetActive(false);
        trail_camera.SetActive(false);
    }

    public void Offers(Image button)
    {
        lastPanel_Shop.SetActive(false);
        panelsDict["PanelOffers"].SetActive(true);
        lastPanel_Shop = panelsDict["PanelOffers"];

        lastButton_Shop.color = Color.white;
        button.color = new Color(1, 0.5f, 0);
        lastButton_Shop = button;

        underline_Shop.localPosition = button.rectTransform.localPosition;

        cube_camera.SetActive(false);
        trail_camera.SetActive(false);
    }

    #endregion


    #region Store

    public void Buy()
    {
        Application.OpenURL("https://vk.com/app6302639_-172029485");
    }

    public void Change_Emerald()
    {
        int.TryParse(Emerald_IF.text, out IF_Emeralds_int);
        IF_Coins_int = CHANGEMODIFIER * IF_Emeralds_int;
        Coin_IF.text = IF_Coins_int.ToString();
    }
    public void EndChange()
    {
        if (Emerald_IF.text == "") Emerald_IF.text = "0";
    }
    public void Confirm()
    {
        if (sv.emerald >= IF_Emeralds_int && sv.emerald > 0 && IF_Emeralds_int >= 0)
        {
            sv.emerald -= IF_Emeralds_int;
            sv.coin += IF_Coins_int;
            for (int i = 0; i < coinsText.Length; i++)
            {
                emeralds[i].text = sv.emerald.ToString();
                coinsText[i].text = sv.coin.ToString();
            }

            AudioManager.PlayAudio("Buy");
        }
        else
        {
            Coin_IF.GetComponentInChildren<Text>().color = Color.red;
            Emerald_IF.GetComponentInChildren<Text>().color = Color.red;
            Invoke("ReturnColor", 0.5f);
            AudioManager.PlayAudio("Deny");
        }
    }

    #endregion

    #region Custom

    public void SetUpFigure()
    {
        cube_camera.SetActive(true);
        trail_camera.SetActive(false);

        texts["Figure"].gameObject.SetActive(true);
        texts["Figure"].text = LanguageManager.GetLocalizedValue("Figure");
        texts["Trail"].gameObject.SetActive(false);
    }

    public void SetUpTrail()
    {
        cube_camera.SetActive(false);
        trail_camera.SetActive(true);
        custom_Trail.Clear();

        texts["Trail"].gameObject.SetActive(true);
        texts["Trail"].text = LanguageManager.GetLocalizedValue("Trail");
        texts["Figure"].gameObject.SetActive(false);
    }

    public void ChangeColor(GameObject button)
    {
        if (texts["Figure"].gameObject.activeSelf)
        {
            figures_Colors[0].material.color = button.GetComponent<Image>().color;
            for (int i = 0; i < figures_Colors.Length; i++)
            {
                figures_Colors[i].material.color = figures_Colors[0].material.color;
            }
            figureIcon.color = figures_Colors[0].material.color;

            foreach (Renderer color in figures_Colors)
            {
                sv.colorFigure = color.material.color;
            }

            lastColor_cube = sv.colorFigure;
        }
        else if (texts["Trail"].gameObject.activeSelf)
        {
            bot_Trail.material.color = button.GetComponent<Image>().color;
            custom_Trail.material.color = button.GetComponent<Image>().color;
            trailIcon.color = button.GetComponent<Image>().color;

            sv.colorTrail = custom_Trail.material.color;

            lastColor_trail = sv.colorTrail;
        }
    }
    public void ChangeColor_ColorPalette()
    {
        if (texts["Figure"].gameObject.activeSelf)
        {
            foreach (Renderer color in figures_Colors)
            {
                color.material.color = picker.CurrentColor;
            }
            figureIcon.color = picker.CurrentColor;
        }
        else if (texts["Trail"].gameObject.activeSelf)
        {
            bot_Trail.material.color = picker.CurrentColor;
            custom_Trail.material.color = picker.CurrentColor;
            trailIcon.color = picker.CurrentColor;
        }
    }

    public void Cancel_ChangeColor()
    {
        if (texts["Figure"].gameObject.activeSelf)
        {
            foreach (Renderer color in figures_Colors)
            {
                color.material.color = lastColor_cube;
            }
            figureIcon.color = lastColor_cube;
        }
        else if (texts["Trail"].gameObject.activeSelf)
        {
            bot_Trail.material.color = lastColor_trail;
            custom_Trail.material.color = lastColor_trail;
            trailIcon.color = lastColor_trail;
        }

        panelsDict["PanelColorPalette"].SetActive(false);
    }

    public void Confirm_ChangeColor()
    {
        if (sv.coin >= 100)
        {
            sv.coin -= 100;

            for (int i = 0; i < coinsText.Length; i++) coinsText[i].text = sv.coin.ToString();

            if (texts["Figure"].gameObject.activeSelf)
            {
                sv.colorFigure = picker.CurrentColor;
                lastColor_cube = picker.CurrentColor;
                figureIcon.color = picker.CurrentColor;
            }
            else if (texts["Trail"].gameObject.activeSelf)
            {
                sv.colorTrail = picker.CurrentColor;
                lastColor_trail = picker.CurrentColor;

                trailIcon.color = picker.CurrentColor;
            }

            AudioManager.PlayAudio("Buy");

            panelsDict["PanelColorPalette"].SetActive(false);
        }
        else
        {
            AudioManager.PlayAudio("Deny");

            buyText.color = Color.red;
            Invoke("ReturnColor", 0.5f);
        }
    }

    public void ColorPalette()
    {
        if (sv.IsPayed)
        {
            panelsDict["PanelColorPalette"].SetActive(true);

            if (texts["Figure"].gameObject.activeSelf)
            {
                foreach (Renderer color in figures_Colors) color.material.color = sv.colorFigure;

                picker.CurrentColor = sv.colorFigure;
            }
            else if (texts["Trail"].gameObject.activeSelf)
            {
                bot_Trail.material.color = sv.colorTrail;
                custom_Trail.material.color = sv.colorTrail;

                picker.CurrentColor = sv.colorTrail;
            }
        }
        else panelsDict["PanelLocked"].SetActive(true);
    }

    public void NoPay()
    {
        panelsDict["PanelLocked"].SetActive(false);
    }
    public void YesPay(GameObject lockImage)
    {
        if (sv.coin >= 1000 && sv.emerald >= 500)
        {
            sv.coin -= 1000;
            sv.emerald -= 500;

            for (int i = 0; i < coinsText.Length; i++)
            {
                emeralds[i].text = sv.emerald.ToString();
                coinsText[i].text = sv.coin.ToString();
            }

            sv.IsPayed = true;

            lockImage.SetActive(false);
            lockImage.transform.parent.GetComponent<Button>().interactable = true;
            panelsDict["PanelLocked"].SetActive(false);

            AudioManager.PlayAudio("Buy");
        }
        else
        {
            if (sv.coin < 1000) coinsText[1].color = Color.red;
            if (sv.emerald < 500) emeralds[1].color = Color.red;

            Invoke("ReturnColor", 0.5f);

            AudioManager.PlayAudio("Deny");
        }
    }

    #endregion

    #region Boosts

    public void BuyPowerUp(Text name)
    {
        if (sv.boost.ContainsKey(name.name))
        {
            if (sv.coin >= sv.boost[name.name].cost)
            {
                sv.coin -= sv.boost[name.name].cost;
                sv.boost[name.name].amount++;

                OnChangeMoney();

                AudioManager.PlayAudio("Buy");

                ChangePowerUpAmount(name.name);
            }
            else
            {
                powerUp[name.name].transform.GetChild(4).GetComponent<Image>().color = Color.red;

                AudioManager.PlayAudio("Deny");

                StartCoroutine(ReturnColor(name.name));
            }
        }
        else Debug.LogError("boost not found. Name: " + name);
    }

    public void UpgradePowerUp(Text name)
    {
        if (sv.boost.ContainsKey(name.name))
        {
            if (sv.coin >= sv.boost[name.name].cost)
            {
                sv.coin -= sv.boost[name.name].cost;
                sv.boost[name.name].upgrade++;
                sv.boost[name.name].cost *= 2;

                OnChangeMoney();

                ChangePowerUpUpgrade(name.name);

                AudioManager.PlayAudio("Buy");
            }
        }
        else Debug.LogError("boost not found. Name: " + name);
    }

    #endregion

    #region Offers

    public void SetOffers()
    {
        offers.SetActive(true);
        bundles.SetActive(false);

        bttnOffers.GetComponent<Image>().color = Color.cyan;
        bttnBundles.GetComponent<Image>().color = Color.white;
    }

    public void SetBundles()
    {
        offers.SetActive(false);
        bundles.SetActive(true);

        bttnOffers.GetComponent<Image>().color = Color.white;
        bttnBundles.GetComponent<Image>().color = Color.cyan;
    }

    #endregion

    #endregion Shop

    #region Settings

    public void Settings()
    {
        panelsDict["PanelSettings"].SetActive(true);
    }


    public void Notify()
    {
        switch (sv.notify)
        {
            case "Disabled":
                sv.notify = "Important";
                notifyText.text = LanguageManager.GetLocalizedValue("Important");
                break;

            case "Important":
                sv.notify = "Enabled";
                notifyText.text = LanguageManager.GetLocalizedValue("Enabled");
                break;

            case "Enabled":
                sv.notify = "Disabled";
                notifyText.text = LanguageManager.GetLocalizedValue("Disabled");
                break;

            default:
                sv.notify = "Important";
                notifyText.text = LanguageManager.GetLocalizedValue("Important");
                break;
        }
    }

    public void Quality()
    {
        switch (sv.quality)
        {
            case "Low":
                qualityText.text = LanguageManager.GetLocalizedValue("Medium");
                sv.quality = "Medium";

                QualitySettings.SetQualityLevel(1);
                break;

            case "Medium":
                qualityText.text = LanguageManager.GetLocalizedValue("High");
                sv.quality = "High";

                QualitySettings.SetQualityLevel(2);
                break;

            case "High":
                qualityText.text = LanguageManager.GetLocalizedValue("Low");
                sv.quality = "Low";

                QualitySettings.SetQualityLevel(0);
                break;

            default:
                qualityText.text = LanguageManager.GetLocalizedValue("Medium");
                sv.quality = "Medium";

                QualitySettings.SetQualityLevel(1);
                break;
        }
    }

    public void SliderSounds()
    {
        sv.soundVolume = sl_Sound.value;

        AudioManager.ChangeVolumeAll(SOUND, sl_Sound.value);

        fillSound.color = Color.HSVToRGB(sl_Sound.value / 3f, 1, 1);

        if (sl_Sound.value != 0)
        {
            sound.GetComponent<Image>().sprite = sprites[1];
            sv.colorbttnSound = new Color(0, 0.66f, 1);
            sound.GetComponent<Image>().color = sv.colorbttnSound;
        }
        else
        {
            sound.GetComponent<Image>().sprite = sprites[0];
            sv.colorbttnSound = Color.red;
            sound.GetComponent<Image>().color = sv.colorbttnSound;
        }
    }

    public void SliderMusic()
    {
        sv.musicVolume = sl_Music.value;

        AudioManager.ChangeVolumeAll(MUSIC, sl_Music.value);

        fillMusic.color = Color.HSVToRGB(sl_Music.value / 3f, 1, 1);

        if (sl_Music.value != 0)
        {
            music.GetComponent<Image>().sprite = sprites[3];
            sv.colorbttnMusic = new Color(0, 0.66f, 1);
            music.GetComponent<Image>().color = sv.colorbttnMusic;
        }
        else
        {
            music.GetComponent<Image>().sprite = sprites[2];
            sv.colorbttnMusic = Color.red;
            music.GetComponent<Image>().color = sv.colorbttnMusic;
        }
    }

    #endregion

    public void LeaderBoard()
    {
        GPGamesManager.ShowLeaderboardUI();
    }

    #region Gifts

    public void Gift()
    {
        panelsDict["PanelGift"].SetActive(true);
    }
    public void Gift(GameObject panel)
    {
        panel.SetActive(true);

        panel.transform.GetChild(0).gameObject.SetActive(false);

        BonusCode_IF.text = "";
    }

    public void BuyTicket(Text name)
    {
        Debug.Log(name.name);
        Debug.Log(sv.tickets?[0]?.name ?? "NULL");
        Debug.Log(sv.ticket?.Count ?? 0);
        Debug.Log(sv.ticket["CommonTicket"].name);
        if (sv.emerald >= sv.ticket[name.name].cost)
        {
            sv.emerald -= sv.ticket[name.name].cost;
            sv.ticket[name.name].amount++;
            tickets[sv.ticket[name.name].rarity].transform.GetChild(1).GetChild(0).GetComponent<Text>().text = sv.ticket[name.name].amount.ToString() + "x";

            AudioManager.PlayAudio("Buy");

            OnChangeMoney();
        }
        else
        {
            tickets[sv.ticket[name.name].rarity].transform.GetChild(2).GetComponent<Image>().color = Color.red;

            AudioManager.PlayAudio("Deny");

            StartCoroutine(ReturnColor(name.name));
        }
    }
    public void UseTicket(Text name)
    {
        if (sv.ticket[name.name].amount > 0)
        {
            int temp;
            for (temp = 0; temp < rewards.Length; temp++) if (rewards[temp].rewards.Count > 0) break;
            if (temp == rewards.Length)
            {
                Debug.LogError("No rewards more");
                return;
            }

            sv.ticket[name.name].amount--;

            tickets[sv.ticket[name.name].rarity].transform.GetChild(1).GetChild(0).GetComponent<Text>().text = sv.ticket[name.name].amount.ToString() + "x";

            curtain.SetActive(true);

            allRewards.gameObject.SetActive(true);

            int[] sums = new int[5];
            for (int i = 0; i < rewards.Length; i++)
            {
                for (int j = 0; j < rewards[i].rewards.Count; j++)
                {
                    if (rewards[i].rewards[j].name == names[0]) sums[0] += rewards[i].rewards[j].amount;
                    if (rewards[i].rewards[j].name == names[1]) sums[1] += rewards[i].rewards[j].amount;
                    if (rewards[i].rewards[j].name == names[2]) sums[2] += rewards[i].rewards[j].amount;
                    if (rewards[i].rewards[j].name == names[3]) sums[3] += rewards[i].rewards[j].amount;
                    if (rewards[i].rewards[j].name == names[4]) sums[4] += rewards[i].rewards[j].amount;
                }
            }
            for (int i = 0; i < 5; i++)
            {
                allRewards.GetChild(i).GetComponent<Text>().text = sums[i].ToString() + "x";
            }

            for (int i = 0; i < curtains.Length; i++) curtains[i].color = new Color(curtains[i].color.r, curtains[i].color.g, curtains[i].color.b, 1);
            curtains[sv.ticket[name.name].rarity].gameObject.SetActive(false);

            rewardText.transform.position = curtains[sv.ticket[name.name].rarity].transform.position;
            rewardText.text = "Выберите ячейку";
        }
        else
        {
            tickets[sv.ticket[name.name].rarity].transform.parent.GetChild(1).GetComponent<Image>().color = Color.red;

            AudioManager.PlayAudio("Deny");

            StartCoroutine(ReturnColor(name.name));
        }
    }

    public void Reward(int index)
    {
        int temp = UnityEngine.Random.Range(0, rewards[index].rewards.Count);

        switch (rewards[index].rewards[temp].name)
        {
            case "Coins":
                sv.coin += rewards[index].rewards[temp].amount;
                break;

            case "Emeralds":
                sv.emerald += rewards[index].rewards[temp].amount;
                break;

            case "CommonTicket":
                sv.ticket["CommonTicket"].amount += rewards[index].rewards[temp].amount;
                tickets[sv.ticket["CommonTicket"].rarity].transform.GetChild(1).GetChild(0).GetComponent<Text>().text = sv.tickets[sv.ticket["CommonTicket"].rarity].amount.ToString() + "x";
                break;

            case "GoldTicket":
                sv.ticket["GoldTicket"].amount += rewards[index].rewards[temp].amount;
                tickets[sv.ticket["GoldTicket"].rarity].transform.GetChild(1).GetChild(0).GetComponent<Text>().text = sv.tickets[sv.ticket["GoldTicket"].rarity].amount.ToString() + "x";
                break;

            case "Jump":
                sv.boost["Jump"].amount += rewards[index].rewards[temp].amount;
                powerUp["Jump"].transform.GetChild(3).GetChild(1).GetComponent<Text>().text = sv.boost["Jump"].amount.ToString();
                break;

            default:
                Debug.LogError("error");
                break;
        }
        OnChangeMoney();
        rewardText.text = "Ваша награда: " + rewards[index].rewards[temp].amount + " " + rewards[index].rewards[temp].name;
        rewards[index].rewards.RemoveAt(temp);

        if (rewards[index].rewards.Count == 0)
        {
            rewards[index].GetComponent<Button>().interactable = false;
        }

        curtain.SetActive(false);

        allRewards.gameObject.SetActive(false);

        for (int i = 0; i < curtains.Length; i++)
        {
            curtains[i].gameObject.SetActive(true);
            curtains[i].color = new Color(curtains[i].color.r, curtains[i].color.g, curtains[i].color.b, 0);
        }
    }

    private bool CheckBonusCode(string bonusCode)
    {
        bool check = false;
        for (int i = 0; i < bonusCodes.Length; i++)
        {
            if (bonusCode == bonusCodes[i])
            {
                numberBonusCode = i;
                check = true;
                break;
            }
        }

        return check;
    }
    public void BonusCode(GameObject reward)
    {
        if (CheckBonusCode(BonusCode_IF.text))
        {
            if (!sv.IsBonusCodesEntered[numberBonusCode])
            {
                int coinsBonus = 0, emeraldsBonus = 0;

                sv.IsBonusCodesEntered[numberBonusCode] = true;

                BonusCode_IF.text = "";

                switch (numberBonusCode)
                {
                    case 0:
                        coinsBonus = 30000;
                        emeraldsBonus = 8000;

                        sv.coin += coinsBonus;
                        sv.emerald += emeraldsBonus;
                        break;

                    case 1:
                        coinsBonus = 15000;
                        emeraldsBonus = 4000;

                        sv.coin += coinsBonus;
                        sv.emerald += emeraldsBonus;
                        break;

                    case 2:
                    case 3:
                        coinsBonus = 5000;
                        emeraldsBonus = 1000;

                        sv.coin += coinsBonus;
                        sv.emerald += emeraldsBonus;
                        break;
                }

                for (int i = 0; i < coinsText.Length; i++)
                {
                    coinsText[i].text = sv.coin.ToString();
                    emeralds[i].text = sv.emerald.ToString();
                }

                reward.SetActive(true);
                allTexts[12].text = coinsBonus + " " + LanguageManager.GetLocalizedValue("Coins");
                allTexts[13].text = emeraldsBonus + " " + LanguageManager.GetLocalizedValue("Emeralds");
            }
            else
            {
                if (sv.language == "Ru") BonusCode_IF.text = "Вы уже вводили этот бонус код";
                else BonusCode_IF.text = "You already entered this bonus code";

                BonusCode_IF.textComponent.color = Color.red;
                Invoke("ReturnColor", 0.5f);

                AudioManager.PlayAudio("Deny");
            }
        }
        else
        {
            BonusCode_IF.textComponent.color = Color.red;
            Invoke("ReturnColor", 0.5f);

            AudioManager.PlayAudio("Deny");
        }
    }

    #endregion

    public void RateUs()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.ZixxanGames.PerfectWay");
    }

    #region Profile

    public void Profile()
    {
        panelsDict["PanelProfile"].SetActive(true);
        lastPanel_Profile.SetActive(true);
    }

    #region Profile menu

    public void Stats(Image button)
    {
        lastPanel_Profile.SetActive(false);
        panelsDict["PanelStats"].SetActive(true);
        lastPanel_Profile = panelsDict["PanelStats"];

        lastButton_Profile.color = Color.white;
        button.color = new Color(1, 0.5f, 0);
        lastButton_Profile = button;

        underline_Profile.localPosition = button.rectTransform.localPosition;
    }

    #region Achievements

    public void Achievements(Image button)
    {
        notify[0].SetActive(false);
        notify[1].SetActive(false);

        lastPanel_Profile.SetActive(false);
        panelsDict["PanelAchievements"].SetActive(true);
        lastPanel_Profile = panelsDict["PanelAchievements"];

        lastButton_Profile.color = Color.white;
        button.color = new Color(1, 0.5f, 0);
        lastButton_Profile = button;

        underline_Profile.localPosition = button.rectTransform.localPosition;
    }

    public void ShowDesciption(Transform achievement)
    {
        if (panelsDict["PanelAchievementInfo"].transform.position != achievement.position)
        {
            panelsDict["PanelAchievementInfo"].SetActive(true);
        }
        else
        {
            panelsDict["PanelAchievementInfo"].SetActive(!panelsDict["PanelAchievementInfo"].activeSelf);
        }

        panelsDict["PanelAchievementInfo"].transform.position = achievement.position;

        texts["TextInfo"].text = LanguageManager.GetLocalizedValue(achievement.name.Split()[0] + "Description").Replace(" x ", sv.achievement[achievement.name].endProgress.ToString());
    }

    public void GetReward(Transform achievement)
    {
        achievement.GetChild(3).GetComponent<Button>().animator.enabled = false;

        for (int i = 0; i < achievement.GetChild(3).childCount - 1; i++)
        {
            achievement.GetChild(3).GetChild(i).gameObject.SetActive(false);
        }
        achievement.GetChild(3).GetChild(achievement.GetChild(3).childCount - 1).gameObject.SetActive(true);

        if (!sv.achievement[achievement.name].IsUnlocked)
        {
            sv.coin += sv.achievement[achievement.name].coins;
            sv.emerald += sv.achievement[achievement.name].emeralds;

            OnChangeMoney();
        }

        sv.achievement[achievement.name].IsUnlocked = true;
    }

    #endregion

    #region Community

    public void Community(Image button)
    {
        lastPanel_Profile.SetActive(false);
        panelsDict["PanelCommunity"].SetActive(true);
        lastPanel_Profile = panelsDict["PanelCommunity"];

        lastButton_Profile.color = Color.white;
        button.color = new Color(1, 0.5f, 0);
        lastButton_Profile = button;

        underline_Profile.localPosition = button.rectTransform.localPosition;
    }

    public void VKontakte()
    {
        Application.OpenURL("https://vk.com/zg_perfectway");
    }

    public void YouTube()
    {
        Application.OpenURL("https://www.youtube.com/channel/UCMvOJleNpBvjXm7agVsnoFw?view_as=subscriber");
    }

    public void Support()
    {
        Application.OpenURL("https://vk.com/topic-172029485_39342841");
    }

    #endregion

    #region Info

    public void Info(Image button)
    {
        lastPanel_Profile.SetActive(false);
        panelsDict["PanelInfo"].SetActive(true);
        lastPanel_Profile = panelsDict["PanelInfo"];

        lastButton_Profile.color = Color.white;
        button.color = new Color(1, 0.5f, 0);
        lastButton_Profile = button;

        underline_Profile.localPosition = button.rectTransform.localPosition;
    }

    public void PrivacyPolicy()
    {
        Application.OpenURL("https://github.com/Zixxan/ZixxanGames.github.io/blob/master/PrivacyPolicy");
    }

    public void AboutGame()
    {
        Debug.Log("AboutGame");
    }

    public void ShowCloudSavesUI()
    {
        GPGamesManager.ShowSavesUI((status, data) =>
        {
            if (status == GooglePlayGames.BasicApi.SavedGame.SavedGameRequestStatus.Success && data.Length > 0)
            {
                Debug.Log("*** Loading data... ");
                sv.Load(sv, System.Text.Encoding.UTF8.GetString(data));
                Debug.Log("*** Loading data completed ");

                OnChangeMoney();
                ChangeTickets();

                NotifyManager.ShowNotify(gameNotify, LanguageManager.GetLocalizedValue("CloudLoadSuccess"), NotifyType.Success, 4);
            }
        },
        () =>
        {
            Debug.Log("*** Saving...");
            GPGamesManager.allowSaving = true;
            sv.Save();
        });
    }

    #endregion /Info

    #endregion /Profile menu

    #endregion /Profile

    #endregion /Main Menu
}
