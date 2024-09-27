using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalNavigator : MonoBehaviour
{
    public void HistoryLogger()
    {
        SceneManager.LoadScene(ChronicleCache.INTERIM_STAGE);
    }
    public void WorldBootstrap()
    {
        SceneManager.LoadScene(ChronicleCache.PIXEL_ADVENTURE);
    }
        
}