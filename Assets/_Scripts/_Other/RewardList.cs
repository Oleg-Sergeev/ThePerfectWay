using System.Collections.Generic;
using UnityEngine;


public class RewardList : MonoBehaviour
{
    public List<Reward> rewards;
}

[System.Serializable]
public class Reward
{
    public string name;
    public int amountMin, amountMax, amount;
}
