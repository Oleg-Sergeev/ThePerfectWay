using UnityEngine;

public class _Player : MonoBehaviour
{
    #region Variables
    private const string MUSIC = "Music", SOUND = "Sound";
    private static float speed, lastSpeed;
    private static BoxCollider bx;
    public static _Player Instance;
    public static Renderer player_renderer;
    public static Rigidbody rb;
    public static float ScoreFloat, _scoreFloat, totalDistance;
    public static float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            if (value == 0)
            {
                if (speed == 0)
                {
                    Debug.LogWarning("Speed was not saved: speed = " + speed + ". Last speed = " + lastSpeed);
                    return;
                }

                lastSpeed = speed;
                speed = value;
            }
            else
            {
                if (value == -1) speed = lastSpeed;
                else speed = value;
            }
        }
    }
    public static string Score;
    public static int IsTutorialPassed, coins_count, emeralds_count, ScoreInt, _scoreInt, modifier;
    public static bool CanTurn, IsMovingX, HasLose;
    private float MoneyMagnifer = 1;
    private Vector3 velocity, movHor, movVer;
    public TrailRenderer player_Trail;
    #endregion


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        coins_count = 0;
        emeralds_count = 0;
        ScoreFloat = 0;
        totalDistance = 0;
        modifier = 4;
        IsMovingX = true;
        CanTurn = false;

        player_renderer = GetComponent<Renderer>();
        player_renderer.material.color = _UserInterface.sv.colorFigure;
        player_Trail.material.color = _UserInterface.sv.colorTrail;

        rb = GetComponent<Rigidbody>();

        bx = GetComponent<BoxCollider>();
    }

    public void FixedUpdate()
    {
        velocity = transform.forward * Speed;

        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

        totalDistance += (Time.fixedDeltaTime * Speed / 2) / modifier;

        ScoreFloat += Modifier_Score.modifier * Time.fixedDeltaTime * Speed / 2;
        ScoreInt = Mathf.RoundToInt(ScoreFloat);
        Score = ScoreInt.ToString();
    }

    public static void ChangeBoxCollider()
    {
        if (Armor.IsArmor) bx.size = new Vector3(1.02f, 1, 1.02f);
        else bx.size = new Vector3(1, 1, 1);
    }

    private void OnCollisionEnter(Collision Walls)
    {
        switch (Walls.gameObject.tag)
        {
            case "Lose":
                if (!Armor.IsArmor)
                {
                    if (!HasLose)
                    {
                        if (EventAggregator.loseEvents != null) EventAggregator.loseEvents.Publish(this);
                        HasLose = true;
                    }
                }
                else
                {
                    Speed = 0;
                    CanTurn = false;

                    Invoke("ContinueGame", 2f);
                }
                break;

            case "TutorialLose":
                EventAggregator.loseEvents.Publish(this);
                break;

            case "GreenWall": // столкновение с "дверью"
                if (!Keys.IsCollected)
                {
                    if (!HasLose)
                    {
                        if (EventAggregator.loseEvents != null) EventAggregator.loseEvents.Publish(this);
                        HasLose = true;
                    }
                }
                else
                {
                    if (Walls.gameObject.name == "WallDoor_Sound") AudioManager.PlayAudio("WallDoorSound");
                    Invoke("RedColor", 3f);
                }
                break;

            case "SpringBoard":
                CanTurn = false;
                Invoke("_CanMove", 1.5f);
                break;
        }
    }

    private void OnTriggerEnter(Collider _trigger)
    {
        switch (_trigger.tag)
        {
            case "TutorialWall":
                AudioManager.PlayAudio("Deny");
                UI_Tutorial.Instance.panelsDict["PanelWarning"].SetActive(true);
                Speed = 0;
                CanTurn = false;
                break;

            case "Booster":
                rb.AddForce(velocity * 2f, ForceMode.Impulse);
                CanTurn = false;
                Invoke("_CanMove", 1.5f);
                Freeze();

                AudioManager.PlayAudio("BoosterSound");
                break;           

            case "Jump":
                rb.AddForce(Vector3.up * Speed, ForceMode.Impulse);
                CanTurn = false;
                Invoke("_CanMove", 4f);
                break;

            case "SpeedMagnifer":
                Speed = _UserInterface.IsArcade ? Speed += 7 : Speed += 10;
                if (Speed > 110) Speed = 110;

                if (MoneyMagnifer % 2 == 0)
                {
                    _UserInterface.sv.coin += 10 * (int)MoneyMagnifer;
                    _UserInterface.sv.emerald += 2 * (int)MoneyMagnifer;
                }

                MoneyMagnifer++;

                if (Keys.IsCollected) Keys.IsCollected = false;

                break;
        }
    }
    
    private void ContinueGame()
    {
        BaseUI.Instance.OnUseArmor();

        Armor.IsArmor = false;

        ChangeBoxCollider();
    }

    private void _CanMove()
    {
        CanTurn = true;        
    }
    
    private void Freeze()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void RedColor()
    {
        Keys.IsCollected = false;
    }
}
