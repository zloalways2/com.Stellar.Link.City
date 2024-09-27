using UnityEngine;
using UnityEngine.SceneManagement;

public class JourneyConductor : MonoBehaviour
{
    public void LevelGatekeeper(int levelIndex)
    {
        PlayerPrefs.SetInt("AdvancementLadder" + levelIndex, 1);
        PlayerPrefs.Save();
    }

    public void AdvancementElevator(int levelIndex)
    {
        PlayerPrefs.SetInt(ChronicleCache.CURRENT_STAGE, levelIndex + 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(ChronicleCache.PIXEL_ADVENTURE);
    }

    public void EraScheduleManager()
    {
        for (var iterable = 0; iterable < ChronicleCache.LEVEL_CAPACITY; iterable++)
        {
            if (!PlayerPrefs.HasKey("AdvancementLadder" + iterable))
                PlayerPrefs.SetInt("AdvancementLadder" + iterable, iterable == 0 ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    public bool RouteValidator(int levelIndex)
    {
        return PlayerPrefs.GetInt("AdvancementLadder" + levelIndex, 0) == 1;
    }
}