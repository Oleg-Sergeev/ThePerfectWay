using UnityEngine;

public class Finish : MonoBehaviour
{
    public static int coinCount, emeraldCount;
    [HideInInspector]

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player") OnFinish();
    }

    public void OnFinish()
    {
        if (_UserInterface.sv.currentLevel == _UserInterface.sv.completedLevel + 1 && _UserInterface.sv.currentLevel < _UserInterface.sv.levelsCount)
        {
            _UserInterface.sv.completedLevel++;
        }
        else if (_UserInterface.sv.currentLevel == _UserInterface.sv.levelsCount)
        {
            _UserInterface.sv.completedLevel = _UserInterface.sv.levelsCount;
        }

        _UserInterface.sv.levels[_UserInterface.sv.currentLevel - 1].stars[0] = 1;
        UI_Adventure.Instance.OnStar(0);

        if (AdventureLevelsManager.Instance.allCoins[_UserInterface.sv.currentLevel].transform.childCount <= coinCount)
        {
            coinCount = 0;
            emeraldCount = 0;
        }

        EventAggregator.finishEvents.Publish(this);
        
        _UserInterface.sv.totalDistance += _Player.totalDistance;
    }
}
