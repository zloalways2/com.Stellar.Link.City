using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _progressMessage;
    private float _statusTimer;
    private int _markerTotal; 
    private const float _markerSpacing = 0.5f;

    private void Start()
    {
        StartCoroutine(WorldInitializer(ChronicleCache.PRIMARY_STAGE));
    }

    private IEnumerator WorldInitializer(string nameScene)
    {
        AsyncOperation concurrentProcess = SceneManager.LoadSceneAsync(nameScene);

        concurrentProcess.allowSceneActivation = false;

        while (!concurrentProcess.isDone)
        {
            TextBlinkUpdater();
                
            if (concurrentProcess.progress >= 0.9f)
            {
                concurrentProcess.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private void TextBlinkUpdater()
    {
        _statusTimer += Time.deltaTime;

        if (_statusTimer >= _markerSpacing)
        {
            _markerTotal = (_markerTotal + 1) % 4;
            string statusMarkers = new string('.', _markerTotal);
            _progressMessage.text = "Loading" + statusMarkers;
            _statusTimer = 0f;
        }
    }
}