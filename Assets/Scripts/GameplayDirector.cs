using TMPro;
using UnityEngine;

public class GameplayDirector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _level;
        
    private void Start()
    {
        TimeFlowRestorer();
        AssembleGameElements();
    }

    private void AssembleGameElements()
    {
        _level.text = "Level " + PlayerPrefs.GetInt(ChronicleCache.CURRENT_STAGE, 0);
    }

    public void TimeFlowRestorer()
    {
        Time.timeScale = 1f;
    }
        
    public void PauseModeActivator()
    {
        Time.timeScale = 0f;
    }
}