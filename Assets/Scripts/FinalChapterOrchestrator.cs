using UnityEngine;

public class FinalChapterOrchestrator : MonoBehaviour
{ 
    public void JourneyFinalizer()
    {
#if UNITY_EDITOR
        
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}