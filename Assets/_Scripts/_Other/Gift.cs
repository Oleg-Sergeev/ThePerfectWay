using UnityEngine;

public enum Name
{
    Coins,
    Emeralds,
    Jump,
    CommonTicket,
    GoldTicket,
    PerfectTicket
}

public class Gift : MonoBehaviour
{
    public Name names;
    public int amount;
}
