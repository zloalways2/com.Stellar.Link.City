using UnityEngine;
using UnityEngine.UI;

public class MysticStageViewer : MonoBehaviour
{
    [SerializeField] private Button[] _stageSelections;

    private JourneyConductor _progressionGuide;

    private void Start()
    {
        _progressionGuide = FindObjectOfType<JourneyConductor>();
        _progressionGuide.EraScheduleManager();
        ColorBlendMatrix();
    }

    private void ColorBlendMatrix()
    {
        for (var indexIter = 0; indexIter < ChronicleCache.LEVEL_CAPACITY; indexIter++)
        {
            if (indexIter == 0 || _progressionGuide.RouteValidator(indexIter))
            {
                var levelIndex = indexIter;
                _stageSelections[indexIter].onClick.AddListener(() => _progressionGuide.AdvancementElevator(levelIndex));
            }
            else
            {
                _stageSelections[indexIter].interactable = false;
            }
        }
    }
}