using UnityEngine;
using System.Collections;

public class TimeSlowing : AbstractPowerUp
{
    public override void PowerOff()
    {
        Time.timeScale = 1;

        StopCoroutine(methodName);
    }

    override public void FixedUpdate()
    {
        gameObject.transform.Rotate(Vector3.up * 1.5f);
    }
    
    override public void OnTriggerEnter(Collider other)
    {
        StartCoroutine("PowerUp");
    }

    override public IEnumerator PowerUp()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        Application.targetFrameRate = 120;

        while (Time.timeScale > 0.5f)
        {
            Time.timeScale -= Time.deltaTime / 2;
            
            yield return null;
        }
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime /= 2f;

        yield return new WaitForSeconds(2.5f + Modifier(0.2f, _UserInterface.sv.boost["TimeSlowing"].upgrade));

        while (Time.timeScale < 1)
        {
            Time.timeScale += Time.deltaTime / 2;
            yield return null;
        }
        Time.timeScale = 1;
        Time.fixedDeltaTime *= 2;

        Application.targetFrameRate = 60;

        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}
