using UnityEngine;
using System.Collections;

public class Armor : AbstractPowerUp
{
    public static bool IsArmor;
    public _Player player;


    override public void PowerOff()
    {
        IsArmor = false;

        StopCoroutine(methodName);
    }

    override public void FixedUpdate()
    {
        transform.Rotate(Vector3.up * 1.5f);
    }

    override public void OnTriggerEnter(Collider other)
    {
        StopCoroutine(methodName);
        StartCoroutine(methodName);
    }
    
    override public IEnumerator PowerUp()
    {
        GetComponent<MeshRenderer>().enabled = false;

        IsArmor = true;

        BaseUI.Instance.OnArmor(IsArmor);
        _Player.ChangeBoxCollider();

        yield return new WaitForSeconds(15 + Modifier(4, _UserInterface.sv.boost["Armor"].upgrade));

        IsArmor = false;

        BaseUI.Instance.OnArmor(IsArmor);
        _Player.ChangeBoxCollider();

        GetComponent<MeshRenderer>().enabled = true;
    }
}
