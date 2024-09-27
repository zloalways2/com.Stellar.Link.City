using UnityEngine;

public class SymbolWeaver : MonoBehaviour
{ 
    private int _currentStage;

    private void Awake()
    {
        DimensionBuilder();
    }

    private void DimensionBuilder()
    {
        _currentStage = PlayerPrefs.GetInt(ChronicleCache.CURRENT_STAGE, 0);
    }

    public void TriumphSoundCue()
    {
        PlayerPrefs.SetInt(ChronicleCache.CURRENT_STAGE, _currentStage + 1);
        PlayerPrefs.Save();
        
        var stageCoordinator = FindObjectOfType<JourneyConductor>();
        stageCoordinator.LevelGatekeeper(_currentStage);
    }
}