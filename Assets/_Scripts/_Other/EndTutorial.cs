using UnityEngine;
using System.Collections;

public class EndTutorial : MonoBehaviour
{
    private static bool IsEnd;
    public static bool HasGetTip;
    public Transform cam;


    private void Start()
    {
        IsEnd = false;
    }

    private void OnTriggerEnter(Collider col)
    {
        _Player.CanTurn = false;
        StartCoroutine("Stopping");
    }

    private IEnumerator Stopping()
    {
        while (_Player.Speed > 1)
        {
            _Player.Speed -= 13f * Time.fixedDeltaTime;
            yield return null;
        }
        _Player.Speed = 0;
        if (IsEnd) Invoke("Win_", 1);
        else NextTutorial();
    }

    private void Win_()
    {
        if (!_UserInterface.sv.IsTutorialPassed)
        {
            _UserInterface.sv.coin += 1000;
            _UserInterface.sv.emerald += 250;
        }

        _UserInterface.sv.achievement["TutorialMan"].currentProgress = 1;
        GPGamesManager.UnlockAchievement(GPGSIds.achievement_tutorialman);

        if (!HasGetTip)
        {
            _UserInterface.sv.achievement["WhereAreTheTips"].currentProgress = 1;
            GPGamesManager.UnlockAchievement(GPGSIds.achievement_where_are_the_tips);
        }

        _UserInterface.sv.IsTutorialPassed = true;

        UI_Tutorial.Instance.winText.gameObject.SetActive(true);

        Invoke("Menu", 3f);
    }

    private void NextTutorial()
    {
        cam.GetComponent<_Camera>().enabled = false;

        cam.position = new Vector3(_Player.Instance.transform.position.x, cam.position.y + 10, _Player.Instance.transform.position.z);
        cam.rotation = Quaternion.Euler(62, 0, 0);

        cam.gameObject.AddComponent<CameraHardcore>();
        cam.GetComponent<CameraHardcore>()._target = _Player.Instance.transform.GetChild(0);

        Transform temp = UI_Tutorial.Instance.transform.root;
        temp.GetChild(0).GetComponent<TouchController>().enabled = false;
        temp.GetChild(0).gameObject.AddComponent<SwipeController>();
        temp.GetChild(0).GetComponent<SwipeController>().player = _Player.Instance.transform;
        temp.GetChild(0).GetComponent<SwipeController>()._camera = cam;

        IsEnd = true;

        UI_Tutorial.Instance.Transition(true);
    }

    private void Menu()
    {
        _UserInterface.sv.totalDistance += _Player.totalDistance;

        _UserInterface.sv.Save();

        LoadingScene.SceneName = "MainMenu";

        UI_Tutorial.Instance.loading_object.SetActive(true);
        UI_Tutorial.Instance.loadingScreen.SetActive(true);
    }
}
