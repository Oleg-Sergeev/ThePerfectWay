using UnityEngine;
using System.Collections;

public abstract class AbstractPowerUp : MonoBehaviour {

    protected string methodName = "PowerUp";

    protected float Modifier(float modifier, int quantity)
    {
        float number = 0;

        for (int i = 1; i <= quantity; i++) number += modifier * i;
        Debug.Log(number);
        return number;
    }

    public abstract void PowerOff();

    public abstract void FixedUpdate();

    public abstract void OnTriggerEnter(Collider other);

    public abstract IEnumerator PowerUp();
}
