using UnityEngine;

public class Player_Bot : MonoBehaviour
{
    private int IsTriggered, numberLap, afk_limit;
    private float movingX, movingZ, speed, last_ui;
    private bool IsMovingX;
    private static Rigidbody rb;
    private Vector3 Velocity, movHor, movVer, startPos;
    private Quaternion startQtrn;
    public GameObject playerBot;
    public TrailRenderer trail;
    

    private void Start()
    {
        Time.timeScale = 1;

        startPos = playerBot.transform.position;
        startQtrn = playerBot.transform.rotation;

        rb = GetComponent<Rigidbody>();

        numberLap = 1;
        speed = 10;
        movingZ = 5f;
        movingX = 0f;       
    }


    private void FixedUpdate()
    {
        movHor = transform.right * movingX;
        movVer = transform.forward * movingZ;

        Velocity = (movHor + movVer).normalized * speed;

        rb.MovePosition(rb.position + Velocity * Time.fixedDeltaTime);       
    }

    private void OnTriggerEnter(Collider trigger)
    {
        switch (trigger.tag)
        {

            case "TurningRightTrigger_1_2":
                if (numberLap == 1 || numberLap == 2)
                {
                    movingZ = 0f;
                    movingX = 5f;
                }
                break;

            case "TurningRightTrigger_3_4_5":
                if (numberLap == 3 || numberLap == 4 || numberLap == 5)
                {
                    movingZ = 0f;
                    movingX = 5f;
                }
                break;

            case "TurningRightTrigger_5":
                if (numberLap == 5)
                {
                    movingZ = 0f;
                    movingX = 5f;
                }
                break;

            case "TurningForwardTrigger_3":
                if (numberLap == 3 || numberLap <= 2)
                {
                    movingZ = 5f;
                    movingX = 0f;
                }
                break;

            case "TurningForwardTrigger_4":
                if (numberLap >= 4 || numberLap != 1)
                {
                    movingZ = 5f;
                    movingX = 0f;
                }
                break;

            
            case "Booster":
                rb.AddForce(Velocity * 3f, ForceMode.Impulse);
                break;

            case "Teleport":
                trail.gameObject.SetActive(false);
                playerBot.transform.position = startPos;
                playerBot.transform.rotation = startQtrn;
                trail.gameObject.SetActive(true);
                trail.Clear();

                if (numberLap < 5)
                {
                    numberLap++;
                }
                else if (numberLap >= 5)
                {
                    numberLap = 1;
                }
                break;
        }
    }
}
