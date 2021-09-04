using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public static string SceneName;
    public Text textProgress, loadingText;


    private void Start()
    {
        if (SceneName == null) SceneName = "MainMenu";

        StartCoroutine(AsyncLoad());
        textProgress.text = "0";

        LanguageManager.ChangeLanguage();
        loadingText.text = LanguageManager.GetLocalizedValue("Loading");
    }

    public IEnumerator AsyncLoad()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneName);
        float progress = 0;
        
        while (!operation.isDone)
        {
            progress = operation.progress / 0.9f;

            textProgress.text = string.Format("{0:0}", progress * 100);

            yield return null;
        }
    }
}
