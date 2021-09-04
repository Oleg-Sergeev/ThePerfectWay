using UnityEngine;
using System.Collections;

public class Modifier_Score : AbstractPowerUp
{
    public static float modifier;


    override public void PowerOff()
    {
        modifier = 1;

        StopCoroutine(methodName);
    }

    override public void FixedUpdate()
    {
        gameObject.transform.Rotate(Vector3.up * 1.5f);
    }

    override public void OnTriggerEnter(Collider other)
    {
        StopCoroutine(methodName);
        StartCoroutine(methodName);
    }

    override public IEnumerator PowerUp()
    {
        GetComponent<MeshRenderer>().enabled = false;

        modifier = 2;
        BaseUI.Instance.textScore.color = new Color(1, 0.5f, 0);

        yield return new WaitForSeconds(5 + Modifier(1.2f, _UserInterface.sv.boost["ScoreModifier"].upgrade));

        modifier = 1;
        BaseUI.Instance.textScore.color = Color.white;

        GetComponent<MeshRenderer>().enabled = true;
    }
}
